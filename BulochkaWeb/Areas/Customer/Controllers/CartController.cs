using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using Bulochka.Models.ViewModels;
using Bulochka.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulochkaWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult SummaryDelivery()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("SummaryDelivery")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryDeliveryPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) throw new ArgumentNullException(nameof(claim));
            
            SetListCart(shoppingCartVM, claim);
            SetUserInfo(shoppingCartVM, claim);
            SetOrderHeaderAddress(shoppingCartVM,
                shoppingCartVM.OrderHeader.ApplicationUser.City,
                shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress,
                shoppingCartVM.OrderHeader.ApplicationUser.PostalCode);

            SetDefaultInformation(shoppingCartVM);
            CalculateTotal(shoppingCartVM);

            AddOrderHeaderInfoToDb(shoppingCartVM);
            AddOrderDetailInfoToDb(shoppingCartVM);

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SummaryInCafe()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                OrderHeader = new(),
                CompanyBranchList = _unitOfWork.CompanyBranch.GetAll().Select(
                    b => new SelectListItem
                    {
                        Text = b.City + ", " + b.StreetAddress,
                        Value = b.Id.ToString()
                    })
            };
            ShoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("SummaryInCafe")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOSTInCafe(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) throw new ArgumentNullException(nameof(claim));

            var companyBranch = _unitOfWork.CompanyBranch.GetFirstOrDefault(
                b => b.Id == shoppingCartVM.OrderHeader.PickUpPlaceId);

            SetListCart(shoppingCartVM, claim);
            SetUserInfo(shoppingCartVM, claim);
            SetOrderHeaderAddress(shoppingCartVM,
                companyBranch.City,
                companyBranch.StreetAddress,
                companyBranch.PostalCode);

            SetDefaultInformation(shoppingCartVM);
            CalculateTotal(shoppingCartVM);

            AddOrderHeaderInfoToDb(shoppingCartVM);
            AddOrderDetailInfoToDb(shoppingCartVM);

            #region Stripe

            var domain = "https://localhost:44324/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in shoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "rub",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentId(shoppingCartVM.OrderHeader.Id,
                session.Id,
                session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            #endregion

            //_unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);

            SessionService service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == SD.Paid)
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View();
        }

        #region SummaryPOST common methods

        private static void SetOrderHeaderAddress(ShoppingCartVM ShoppingCartVM, string city, string address, string postalCode)
        {
            ShoppingCartVM.OrderHeader.City = city;
            ShoppingCartVM.OrderHeader.StreetAddress = address;
            ShoppingCartVM.OrderHeader.PostalCode = postalCode;
        }

        private static void SetDefaultInformation(ShoppingCartVM shoppingCartVM)
        {
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        }

        private void AddOrderDetailInfoToDb(ShoppingCartVM shoppingCartVM)
        {
            foreach (var cart in shoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Count,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
        }

        private static void CalculateTotal(ShoppingCartVM shoppingCartVM)
        {
            foreach (var cart in shoppingCartVM.ListCart)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }
        }

        private void SetUserInfo(ShoppingCartVM shoppingCartVM, Claim? claim)
        {
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
        }

        private void AddOrderHeaderInfoToDb(ShoppingCartVM shoppingCartVM)
        {
            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();
        }

        private void SetListCart(ShoppingCartVM shoppingCartVM, Claim? claim)
        {
            shoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product");
        }

        #endregion

        #region Cart methods

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        } 

        #endregion

    }
}
