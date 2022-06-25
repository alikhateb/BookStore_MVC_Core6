namespace WebApplication1.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolesVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), _roleManager.Roles.ToList());
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if (roleExists)
            {
                ModelState.AddModelError("Name", "this role already exists.");
                return View(nameof(Index), _roleManager.Roles.ToList());
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                await _roleManager.DeleteAsync(role);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("can't delete this item since it's in use");
            }
        }
    }
}
