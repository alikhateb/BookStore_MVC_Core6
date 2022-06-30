namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = $"{StaticDetails.Role_Admin}, {StaticDetails.Role_Employee}")]
    public class ShippingCompaniesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShippingCompany ShippingCompany { get; set; }

#pragma warning disable CS8618 // Non-nullable property 'ShippingCompany' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public ShippingCompaniesController(IUnitOfWork _unitOfWork)
#pragma warning restore CS8618 // Non-nullable property 'ShippingCompany' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        {
            this._unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            var shippingCompanies = _unitOfWork.ShippingCompany.GetAll().OrderBy(s => s.Name).ToList();

            if (shippingCompanies is null)
            {
                return NotFound("no data found");
            }

            return View(shippingCompanies);
        }

        public IActionResult Add_Update(int id)
        {
            if (id == 0)  //add
            {
                ShippingCompany = new();
                return View(ShippingCompany);
            }
            else   //update
            {
                ShippingCompany = _unitOfWork.ShippingCompany.FindObject(s => s.Id == id);
                return View(ShippingCompany);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add_Update()
        {
            if (!ModelState.IsValid)
            {
                return View(ShippingCompany);
            }

            if (ShippingCompany.Id == 0)  //add
            {
                _unitOfWork.ShippingCompany.Add(ShippingCompany);
            }
            else   //update
            {
                _unitOfWork.ShippingCompany.Update(ShippingCompany);
            }

            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                ShippingCompany = _unitOfWork.ShippingCompany.FindObject(c => c.Id == id);

                if (ShippingCompany is null)
                {
                    return NotFound("invalid id");
                }

                _unitOfWork.ShippingCompany.Remove(ShippingCompany);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("can't delete this item since it's in use");
            }
        }
    }
}
