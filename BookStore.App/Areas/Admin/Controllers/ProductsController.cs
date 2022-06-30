namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            //var products = _unitOfWork.Product.GetAll(x => x.Category, p => p.CoverType).OrderBy(x => x.Title);
            var products = _unitOfWork.Product.GetAll().OrderBy(x => x.Title).ToList();
            if (products is null)
            {
                return NotFound("no data found");
            }
            return Json(new { data = products });
        }

        #endregion

        public IActionResult Upsert([FromRoute] int id)
        {
            var Categories = _unitOfWork.Category.GetAll().OrderBy(x => x.Name);
            var CoverTypes = _unitOfWork.CoverType.GetAll().OrderBy(x => x.Name);

            if (id == 0)
            {
                //create new product
                var productVM = new ProductVM
                {
                    Product = new Product(),
                    CoverTypes = CoverTypes,
                    Categories = Categories
                };
                return View(productVM);
            }
            else
            {
                //edit product
                var productVM = new ProductVM
                {
                    Product = _unitOfWork.Product.FindObject(c => c.Id == id),
                    CoverTypes = CoverTypes,
                    Categories = Categories
                };
                if (productVM.Product is null)
                {
                    return NotFound("invalid id");
                }
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert([FromForm] ProductVM model, [FromForm] IFormFile? file)
        {
            model.Categories = _unitOfWork.Category.GetAll().OrderBy(x => x.Name);
            model.CoverTypes = _unitOfWork.CoverType.GetAll().OrderBy(x => x.Name);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (file is not null)
            {
                var extentions = new List<string> { ".jpg", ".png" };

                if (!extentions.Contains(Path.GetExtension(file.FileName).ToLower()))    //check file extention
                {
                    ModelState.AddModelError("imagProfilePicturee", "only .jpg, .png image are allowed");
                    return View(model);
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (model.Product.ImageUrl is not null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, model.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                //string uploadPath = Path.Combine(wwwRootPath, @"assets\products");
                string uploadPath = $@"{wwwRootPath}\assets\products";
                string fileName = Guid.NewGuid().ToString();
                //string extension = Path.GetExtension(model.File.FileName);

                //using var fileStreem = new FileStream(Path.Combine(uploadPath, $"{fileName}{extension}"), FileMode.Create);
                using var fileStreem = new FileStream($@"{uploadPath}\{fileName}_{file.FileName}", FileMode.Create);
                file.CopyTo(fileStreem);

                model.Product.ImageUrl = $@"\assets\products\{fileName}_{file.FileName}";
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (model.Product.Id == 0)
            {
                _unitOfWork.Product.Add(model.Product);
            }
            else
            {
                _unitOfWork.Product.Update(model.Product);
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var product = _unitOfWork.Product.FindObject(c => c.Id == id);
                if (product is null)
                {
                    return NotFound("invalid id");
                }
                if (product.ImageUrl is not null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Product.Remove(product);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                //ModelState.AddModelError(null, exception.InnerException.Message);
                return BadRequest("can't delete this item since it's in use");
            }
        }
    }
}
