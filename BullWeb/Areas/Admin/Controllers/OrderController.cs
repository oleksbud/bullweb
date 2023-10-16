using System.Linq.Expressions;
using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Bull.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BullWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<OrderVM> Orders { get; set; }
        [BindProperty]
        public OrderVM OrderVm { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index(string status)
        {
            Expression<Func<OrderHeader, bool>> statusCondition;
            switch (status)
            {
                case "pending":
                    statusCondition = (x => x.PaymentStatus == StaticDetails.PaymentStatusPending);
                    break;
                case "inprogress":
                    statusCondition = (x => x.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    statusCondition = (x => x.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    statusCondition = (x => x.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    statusCondition = (x => true);
                    break;
            }

            Expression<Func<OrderHeader, bool>> roleCondition;
            if (User.IsInRole(StaticDetails.RoleAdmin) || User.IsInRole(StaticDetails.RoleEmployee))
            {
                roleCondition = x => true;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                if (claimsIdentity != null)
                {
                    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                    roleCondition = x => x.ApplicationUserId == userId;
                }
                else
                {
                    roleCondition = x => false;
                }
            }

            var param = Expression.Parameter(typeof(OrderHeader), "x");
            var finalConditionsBody =  Expression.AndAlso(
                Expression.Invoke(roleCondition, param), Expression.Invoke(statusCondition, param));
            var finalConditions = Expression.Lambda<Func<OrderHeader, bool>>(finalConditionsBody, param);

            var dictionary = new List<string> { "ApplicationUser" };
            List<OrderHeader> objHeaders = _unitOfWork.OrderHeader.GetAll(finalConditions, dictionary).ToList();
            Orders = new List<OrderVM>();

            foreach (var orderHeader in objHeaders)
            {
                Orders.Add(new OrderVM
                {
                    OrderHeader = orderHeader,
                    OrderDetails = new List<OrderDetail>(),
                });
            }
            
            return View(Orders);
        }

        public IActionResult Details(int id)
        {
            var dictionaryAppUser = new List<string> { "ApplicationUser" };
            var dictionaryBooks = new List<string> { "OrderHeader", "Book" };
            OrderVm = new OrderVM
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, dictionaryAppUser),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, dictionaryBooks)
            };
            return View(OrderVm);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDB = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);

            orderHeaderFromDB.Name = OrderVm.OrderHeader.Name;
            orderHeaderFromDB.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = OrderVm.OrderHeader.StreetAddress;
            orderHeaderFromDB.State = OrderVm.OrderHeader.State;
            orderHeaderFromDB.City = OrderVm.OrderHeader.City;
            orderHeaderFromDB.PostalCode = OrderVm.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVm.OrderHeader.Carrier))
            {
                orderHeaderFromDB.Carrier = OrderVm.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVm.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDB.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;
            }
            
            _unitOfWork.OrderHeader.Update(orderHeaderFromDB);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details),new { id = orderHeaderFromDB.Id});
        }
        
        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatuses(OrderVm.OrderHeader.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            
            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details),new { id = OrderVm.OrderHeader.Id});
        }
        
        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVm.OrderHeader.Carrier;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            
            TempData["Success"] = "Order shipped Successfully";

            return RedirectToAction(nameof(Details),new { id = OrderVm.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVm.OrderHeader.Id);

            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved && !string.IsNullOrEmpty(orderHeader.PaymentIntentId) )
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
                
                _unitOfWork.OrderHeader.UpdateStatuses(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatuses(orderHeader.Id, StaticDetails.StatusCancelled);
            }
            _unitOfWork.Save();
            
            TempData["Success"] = "Order cancelled Successfully";

            return RedirectToAction(nameof(Details),new { id = OrderVm.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult DetailsPayNow()
        {
            var dictionaryAppUser = new List<string> { "ApplicationUser" };
            var dictionaryBooks = new List<string> { "OrderHeader", "Book" };
            OrderVm.OrderHeader = _unitOfWork.OrderHeader
                .Get(x => x.Id == OrderVm.OrderHeader.Id, dictionaryAppUser);
            OrderVm.OrderDetails = _unitOfWork.OrderDetail
                .GetAll(x => x.OrderHeaderId == OrderVm.OrderHeader.Id, dictionaryBooks);
            
            // stripe logic
            const string domain = "https://localhost:7289/";
            var successUrl = $"admin/order/PaymentConfirmation?orderHeaderId={OrderVm.OrderHeader.Id}";
            var cancelUrl = $"admin/order/details?id={OrderVm.OrderHeader.Id}";
            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain + successUrl,
                CancelUrl = domain + cancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in OrderVm.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }
            
            var service = new SessionService();
            var session = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVm.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var dictionary = new List<string> { "ApplicationUser" };
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId, dictionary);

            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                // it is an order by company
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatuses(orderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
        
            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var dictionary = new List<string> { "ApplicationUser" };
            List<OrderHeader> objHeaders = _unitOfWork.OrderHeader.GetAll(x => true, dictionary).ToList();
            return Json(new { data = objHeaders });
        }

        #endregion
    }
}