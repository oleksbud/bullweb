using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
                {
                    var userCart = _unitOfWork.ShoppingCart.GetAll(
                        x => x.ApplicationUserId == claim.Value).ToList();
                    var totalItems = userCart.Count;
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart, totalItems);
                }

                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart).Value);
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        } 
    }
}

