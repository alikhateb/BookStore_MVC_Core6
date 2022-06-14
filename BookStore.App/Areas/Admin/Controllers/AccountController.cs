using BookStore.Models.ApplicationUser;
using BookStore.Models.IdentityVM;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            //List<UserVM> users = _userManager.Users.Select(user => new UserVM
            //{
            //    Id = user.Id,
            //    UserName = user.UserName,
            //    Email = user.Email,
            //    Phone = user.PhoneNumber,
            //    Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().ToList(),
            //}).ToListAsync().GetAwaiter().GetResult();

            //if (users is null)
            //{
            //    return NotFound("no data found");
            //}

            //if (User.IsInRole(StaticDetails.Role_Admin))
            //{
            //    switch (status)
            //    {
            //        case StaticDetails.Role_Admin:
            //            users = users.Where(u => u.Roles.Contains(StaticDetails.Role_Admin)).ToList();
            //            break;
            //        case StaticDetails.Role_Employee:
            //            users = users.Where(u => u.Roles.Contains(StaticDetails.Role_Employee)).ToList();
            //            break;
            //        case StaticDetails.Role_User_Comp:
            //            users = users.Where(u => u.Roles.Contains(StaticDetails.Role_User_Comp)).ToList();
            //            break;
            //        case StaticDetails.Role_User_Indi:
            //            users = users.Where(u => u.Roles.Contains(StaticDetails.Role_User_Indi)).ToList();
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //return View(users);
            return View();
        }


        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            List<UserVM> users = _userManager.Users.Select(user => new UserVM
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().ToList(),
            }).ToListAsync().GetAwaiter().GetResult();

            if (users is null)
            {
                return NotFound("no data found");
            }

            if (User.IsInRole(StaticDetails.Role_Admin))
            {
                switch (status)
                {
                    case StaticDetails.Role_Admin:
                        users = users.Where(u => u.Roles.Contains(StaticDetails.Role_Admin)).ToList();
                        break;
                    case StaticDetails.Role_Employee:
                        users = users.Where(u => u.Roles.Contains(StaticDetails.Role_Employee)).ToList();
                        break;
                    case StaticDetails.Role_User_Comp:
                        users = users.Where(u => u.Roles.Contains(StaticDetails.Role_User_Comp)).ToList();
                        break;
                    case StaticDetails.Role_User_Indi:
                        users = users.Where(u => u.Roles.Contains(StaticDetails.Role_User_Indi)).ToList();
                        break;
                    default:
                        break;
                }
            }
            return Json(new { data = users });
        }

        #endregion


        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
            {
                return NotFound("no user found");
            }

            var roles = await _roleManager.Roles.ToListAsync();

            var model = new UserRolesVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RolesVM
                {
                    Name = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).GetAwaiter().GetResult()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(UserRolesVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null)
            {
                return NotFound("no user found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.Name) && !role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
