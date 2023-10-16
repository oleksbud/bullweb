using System.Linq.Expressions;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Bull.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public List<OrderVM> Orders = new();

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index(string status)
        {
            Expression<Func<OrderHeader, bool>> condition;
            switch (status)
            {
                case "pending":
                    condition = (x => x.PaymentStatus == StaticDetails.PaymentStatusPending);
                    break;
                case "inprogress":
                    condition = (x => x.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    condition = (x => x.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    condition = (x => x.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    condition = (x => true);
                    break;
            }
            
            var dictionary = new List<string> { "ApplicationUser" };
            List<OrderHeader> objHeaders = _unitOfWork.OrderHeader.GetAll(condition, dictionary).ToList();
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