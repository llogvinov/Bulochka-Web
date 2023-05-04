using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Bulochka.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulochkaWeb.Areas.Employee
{
    [Area("Employee")]
    [Authorize]
    public class CompanyBranchOrders : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public Dictionary<OrderHeader, string> Orders { get; set; }

        public CompanyBranchOrders(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var employee = _unitofwork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            var companyBranchId = employee.CompanyBranchId;

            var orderList = _unitofwork.OrderHeader.GetAll(o => o.PickUpPlaceId == companyBranchId);

            Orders = new Dictionary<OrderHeader, string>();
            foreach (var order in orderList)
            {
                var orderDetails = _unitofwork.OrderDetail.GetAll(d => d.OrderId == order.Id);
                string details = "";
                foreach (var detail in orderDetails)
                {
                    var product = _unitofwork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                    details += product.Title + " \tX" + detail.Count + "\n";
                }
                Orders.Add(order, details);
            }

            return View(Orders);
        }
    }
}
