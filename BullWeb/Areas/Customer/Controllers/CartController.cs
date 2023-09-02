using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartVM ShoppingCartVm;

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        var includeDictionaries = new List<string> { "Book" };

        ShoppingCartVm = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: includeDictionaries)
        };

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.Total += (cart.Price * cart.Count);
        }
        
        return View(ShoppingCartVm);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count < 50)
        {
            return shoppingCart.Book.Price;
        }
        
        if (shoppingCart.Count < 100)
        {
            return shoppingCart.Book.Price50;
        }
        
        return shoppingCart.Book.Price100;
    }
}