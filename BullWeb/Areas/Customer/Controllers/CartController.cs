using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Bull.Utility;
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
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: includeDictionaries),
            OrderHeader = new()
        };

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }
        
        return View(ShoppingCartVm);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

        if (cartFromDb == null)
        {
            return NotFound();
        }
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);  
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

        if (cartFromDb == null)
        {
            return NotFound();
        }

        if (cartFromDb.Count < 1)
        {
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);  
        }
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Remove(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);

        if (cartFromDb == null)
        {
            return NotFound();
        }
        
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        var includeDictionaries = new List<string> { "Book" };

        ShoppingCartVm = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: includeDictionaries),
            OrderHeader = new()
        };

        ShoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        if (ShoppingCartVm.OrderHeader.ApplicationUser != null)
        {
            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.UserName;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVm.OrderHeader.StreetAddress = ShoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
            ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.ApplicationUser.State;
            ShoppingCartVm.OrderHeader.PostalCode = ShoppingCartVm.OrderHeader.ApplicationUser.PostalCode;
        }

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }
        return View(ShoppingCartVm);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        var wholeSaleConfig = new List<WholeSaleConfigItem>
        {
            new() { Amount = 0, Price = shoppingCart.Book.Price },
            new () { Amount = 50, Price = shoppingCart.Book.Price50 },
            new () { Amount = 100, Price = shoppingCart.Book.Price100 }
        };

        return DiscountCalculations.GetPriceBasedOnQuantity(wholeSaleConfig, shoppingCart.Count);
    }
}