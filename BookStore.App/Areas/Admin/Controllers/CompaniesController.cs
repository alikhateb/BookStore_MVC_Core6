using BookStore.DataAccess.UnitOfWork;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.App.Areas.Admin.Controllers
{
    [Area("admin")]
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompaniesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll().OrderBy(x => x.Name);
            if (companies is null)
            {
                return NotFound("no data found");
            }
            return Json(new { data = companies });
        }
        #endregion

        public IActionResult Upsert([FromRoute] int id)
        {
            if (id == 0)
            {
                //create new company
                var company = new Company();
                return View(company);
            }
            else
            {
                //edit company
                var company = _unitOfWork.Company.FindObject(x => x.Id == id);
                if (company is null)
                {
                    return NotFound("invalid id");
                }
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert([FromForm] Company model)
        {
             if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == 0)
            {
                _unitOfWork.Company.Add(model);
            }
            else
            {
                _unitOfWork.Company.Update(model);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var company = _unitOfWork.Company.FindObject(c => c.Id == id);
                if (company is null)
                {
                    return NotFound("invalid id");
                }
                _unitOfWork.Company.Remove(company);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("can't delete this item since it's in use");
            }
        }
    }
}
