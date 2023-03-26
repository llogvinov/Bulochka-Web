using Bulochka.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulochkaWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderHistory : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public OrderHistory(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var ordersList = _unitofwork.OrderHeader.GetAll();

            return View(ordersList);
        }

    }
}
