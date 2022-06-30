namespace BookStore.App.ViewComponents
{
    public class ShoppingCartCounterViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartCounterViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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
