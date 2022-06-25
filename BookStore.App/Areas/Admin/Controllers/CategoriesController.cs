namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll().OrderBy(x => x.Name);
            if (categories is null)
            {
                return NotFound("no data found");
            }
            return View(categories);
        }

        public IActionResult Create()
        {
            var model = new Category();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromForm] Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _unitOfWork.Category.Add(model);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit([FromRoute] int id)
        {
            var category = _unitOfWork.Category.FindObject(c => c.Id == id);
            if (category is null)
            {
                return NotFound("invalid id");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromForm] Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _unitOfWork.Category.Update(model);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var category = _unitOfWork.Category.FindObject(c => c.Id == id);
                if (category is null)
                {
                    return NotFound("invalid id");
                }
                _unitOfWork.Category.Remove(category);
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
