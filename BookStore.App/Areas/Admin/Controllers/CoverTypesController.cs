namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public class CoverTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = _unitOfWork.CoverType.GetAll().OrderBy(x => x.Name);
            if (coverTypes is null)
            {
                return NotFound("no data found");
            }
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            var model = new CoverType();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromForm] CoverType model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _unitOfWork.CoverType.Add(model);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit([FromRoute] int id)
        {
            var coverType = _unitOfWork.CoverType.FindObject(c => c.Id == id);
            if (coverType is null)
            {
                return NotFound("invalid id");
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromForm] CoverType model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _unitOfWork.CoverType.Update(model);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var coverType = _unitOfWork.CoverType.FindObject(c => c.Id == id);
                if (coverType is null)
                {
                    return NotFound("invalid id");
                }
                _unitOfWork.CoverType.Remove(coverType);
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
