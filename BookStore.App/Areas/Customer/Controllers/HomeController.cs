namespace BookStore.App.Areas.Customer.Controllers
{
    [Area("customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartItem ShoppingCartItem { get; set; }

#pragma warning disable CS8618 // Non-nullable property 'ShoppingCartItem' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
#pragma warning restore CS8618 // Non-nullable property 'ShoppingCartItem' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8603 // Possible null reference return.
            var products = _unitOfWork.Product.GetAll(x => x.Category, x => x.CoverType).OrderBy(x => x.Title);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8603 // Possible null reference return.
            if (products is null)
            {
                return NotFound("no data found");
            }
            return View(products);
        }

        public IActionResult Details(int productId)
        {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8603 // Possible null reference return.
            ShoppingCartItem = new()
            {
                Product = _unitOfWork.Product.FindObject(x => x.Id == productId, x => x.Category, x => x.CoverType),
                ProductId = productId,
                Count = 1
            };
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8603 // Possible null reference return.

            if (ShoppingCartItem.Product is null)
            {
                return NotFound("no data found");
            }

            return View(ShoppingCartItem);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ShoppingCartItem.AppUserId = claim.Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (ShoppingCartItem.Count > 1000)
            {
                ShoppingCartItem.Count = 1000;
            }
            if (ShoppingCartItem.Count < 1)
            {
                ShoppingCartItem.Count = 1;
            }

            var existingShoppingCart = _unitOfWork.ShoppingCartItem
                .FindObject(x => x.AppUserId == claim.Value && x.ProductId == ShoppingCartItem.ProductId);

            if (existingShoppingCart is null)
            {
                _unitOfWork.ShoppingCartItem.Add(ShoppingCartItem);
            }
            else
            {
                existingShoppingCart.Count = ShoppingCartItem.Count;
                _unitOfWork.ShoppingCartItem.Update(existingShoppingCart);
            }

            _unitOfWork.SaveChanges();

            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                _unitOfWork.ShoppingCartItem.GetAll(s => s.AppUserId == claim.Value).Count());

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}