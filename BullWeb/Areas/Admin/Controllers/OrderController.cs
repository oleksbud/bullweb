using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
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
        
        public IActionResult Index()
        {
            var dictionary = new List<string> { "ApplicationUser" };
            List<OrderHeader> objHeaders = _unitOfWork.OrderHeader.GetAll(x => true, dictionary).ToList();
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