using BookStore.DataAccess.UnitOfWork;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.App.Areas.Customer.Controllers
{
    [Area("customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartItem ShoppingCartItem { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(x => x.Category, x => x.CoverType).OrderBy(x => x.Title);
            if (products is null)
            {
                return NotFound("no data found");
            }
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCartItem = new()
            {
                Product = _unitOfWork.Product.FindObject(x => x.Id == productId, x => x.Category, x => x.CoverType),
                ProductId = productId,
                Count = 1
            };

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
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartItem.AppUserId = claim.Value;

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

            _unitOfWork.Save();

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