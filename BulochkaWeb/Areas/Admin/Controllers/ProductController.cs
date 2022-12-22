using Bulochka.DataAccess;
using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Bulochka.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulochkaWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private const string ImagesFolder = @"\images\products\";
        private string _tempDataMessage;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _tempDataMessage = "";
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Product product = new();

            if (id == null || id == 0)
            {
                //create
                return View(product);
            }
            else
            {
                //update
                product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                return View(product);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product, IFormFile? file)
        {
            _tempDataMessage = "";
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (product.ImageUrl != null) DeleteOldImage(product);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    product.ImageUrl = ImagesFolder + fileName + extension;
                }

                if (product.Id == 0)
                {
                    _unitOfWork.Product.Add(product);
                    _tempDataMessage = "Продукт успешно добавлен";
                }
                else
                {
                    _unitOfWork.Product.Update(product);
                    _tempDataMessage = "Продукт успешно изменен";
                }

                _unitOfWork.Save();
                TempData["success"] = _tempDataMessage;
                return RedirectToAction("Index");
            }

            return View(product);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll();
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Ошибка во время удаления" });
            }

            DeleteOldImage(product);

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Удалено успешно" });
        }

        #endregion

        private void DeleteOldImage(Product product)
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }
    }
}
