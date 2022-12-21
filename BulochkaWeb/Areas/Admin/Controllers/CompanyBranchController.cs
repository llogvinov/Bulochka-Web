using Bulochka.DataAccess;
using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulochkaWeb.Controllers
{
    [Area("Admin")]
    public class CompanyBranchController : Controller
    {
        private string _tempDataMessage;

        private readonly IUnitOfWork _unitOfWork;

        public CompanyBranchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tempDataMessage = "";
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            CompanyBranch branch = new();

            if (id == null || id == 0)
            {
                //create
                return View(branch);
            }
            else
            {
                //update
                branch = _unitOfWork.CompanyBranch.GetFirstOrDefault(p => p.Id == id);
                return View(branch);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompanyBranch branch, IFormFile? file)
        {
            _tempDataMessage = "";
            if (ModelState.IsValid)
            {
                if (branch.Id == 0)
                {
                    _unitOfWork.CompanyBranch.Add(branch);
                    _tempDataMessage = "Филиал успешно добавлен";
                }
                else
                {
                    _unitOfWork.CompanyBranch.Update(branch);
                    _tempDataMessage = "Филиал успешно изменен";
                }

                _unitOfWork.Save();
                TempData["success"] = _tempDataMessage;
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyBranchList = _unitOfWork.CompanyBranch.GetAll();
            return Json(new { data = companyBranchList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var branch = _unitOfWork.CompanyBranch.GetFirstOrDefault(p => p.Id == id);
            if (branch == null)
            {
                return Json(new { success = false, message = "Ошибка во время удаления" });
            }

            _unitOfWork.CompanyBranch.Remove(branch);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удалено успешно" });
        }

        #endregion

    }
}
