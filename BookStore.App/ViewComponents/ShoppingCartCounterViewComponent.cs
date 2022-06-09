using BookStore.DataAccess.UnitOfWork;
using BookStore.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.App.ViewComponents
{
    public class ShoppingCartCounterViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartCounterViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                if (HttpContext.Session.GetInt32(StaticDetails.SessionCart).GetValueOrDefault() != 0)
                {
                    return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart).GetValueOrDefault());
                }
                else  //sign in or close and reopen the browser and browser does not remember any session
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                        _unitOfWork.ShoppingCartItem.GetAll(s => s.AppUserId == claim.Value).Count());

                    return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart).GetValueOrDefault());
                }
            }
            else  //logout or unregistered user
            {
                HttpContext.Session.Clear();

                return View(0);
            }
        }
    }
}
