using System.Linq.Expressions;
using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Bull.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Admin.Controllers
{
    [Area("admin")]
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
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                roleCondition = x => x.ApplicationUserId == userId;
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