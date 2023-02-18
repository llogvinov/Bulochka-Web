using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulochkaWeb.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitofwork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitofwork.Product.GetAll();

            return View(productList);
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

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                ProductId = productId,
                Product = _unitofwork.Product.GetFirstOrDefault(p => p.Id == productId),
                Count = 1
            };

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _unitofwork.ShoppingCart.GetFirstOrDefault(
                c => c.ApplicationUserId == claim.Value 
                && c.ProductId == shoppingCart.ProductId);
            
            if (cartFromDb == null)
            {
                _unitofwork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitofwork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            }

            _unitofwork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}