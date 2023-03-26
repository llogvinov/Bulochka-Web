using Bulochka.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulochkaWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
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

            var ordersList = _unitofwork.OrderHeader.GetAll(o => o.ApplicationUserId == claim.Value);

            return View(ordersList);
        }

    }
}
