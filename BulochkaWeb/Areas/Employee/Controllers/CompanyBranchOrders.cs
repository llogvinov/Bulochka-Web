using Bulochka.DataAccess.Repository.IRepository;
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

            return View(orderList);
        }
    }
}
