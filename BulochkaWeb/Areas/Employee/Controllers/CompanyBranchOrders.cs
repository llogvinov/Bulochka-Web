using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Bulochka.Models.ViewModels;
using Bulochka.Utility;
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
        public Dictionary<OrderHeader, IEnumerable<OrderDetail>> OrdersNew { get; set; }

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
            var companyBranch = _unitofwork.CompanyBranch.GetFirstOrDefault(c => c.Id == companyBranchId);
            
            var orderList = GetInCafeOrders(companyBranchId);
            var deliveryOrders = GetDeliveryOrders(companyBranch);
            
            var allOrders = new List<OrderHeader> ();
            foreach (var order in orderList) allOrders.Add(order);
            foreach (var order in deliveryOrders) allOrders.Add(order);

            /*Orders = new Dictionary<OrderHeader, string>();
            foreach (var order in allOrders)
            {
                var orderDetails = _unitofwork.OrderDetail.GetAll(d => d.OrderId == order.Id);
                string details = "";
                foreach (var detail in orderDetails)
                {
                    var product = _unitofwork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                    details += $"{detail.Count}X\t {product.Title}\n";
                }
                Orders.TryAdd(order, details);
            }*/

            OrdersNew = new Dictionary<OrderHeader, IEnumerable<OrderDetail>>();
            foreach (var order in allOrders)
            {
                var orderDetails = _unitofwork.OrderDetail.GetAll(d => d.OrderId == order.Id);
                foreach (var detail in orderDetails)
                {
                    var product = _unitofwork.Product.GetFirstOrDefault(p => p.Id == detail.ProductId);
                    detail.Product = product;
                }
                OrdersNew.TryAdd(order, orderDetails);
            }

            return View(OrdersNew);
        }

        private IEnumerable<OrderHeader> GetInCafeOrders(int? companyBranchId) 
            => _unitofwork.OrderHeader.GetAll(o => o.PickUpPlaceId == companyBranchId);

        private IEnumerable<OrderHeader> GetDeliveryOrders(CompanyBranch companyBranch) 
            => _unitofwork.OrderHeader.GetAll(o => o.PickUpPlaceId == null && o.City == companyBranch.City);

        private OrderHeader GetOrderHeader(int orderId) 
            => _unitofwork.OrderHeader.GetFirstOrDefault(o => o.Id == orderId);

        public IActionResult PaymentComplete(int orderId)
        {
            GetOrderHeader(orderId).PaymentStatus = SD.PaymentStatusApproved;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PaymentRejected(int orderId)
        {
            GetOrderHeader(orderId).PaymentStatus = SD.PaymentStatusRejected;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderInProgress(int orderId)
        {
            GetOrderHeader(orderId).OrderStatus = SD.StatusInProgress;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderApproved(int orderId)
        {
            GetOrderHeader(orderId).OrderStatus = SD.StatusApproved;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderFinished(int orderId)
        {
            GetOrderHeader(orderId).OrderStatus = SD.StatusFinished;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderCanceled(int orderId)
        {
            GetOrderHeader(orderId).OrderStatus = SD.StatusCanceled;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
