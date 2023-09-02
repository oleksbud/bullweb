﻿using System.Security.Claims;
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
        var wholeSaleConfig = new List<WholeSaleConfigItem>
        {
            new() { Amount = 0, Price = shoppingCart.Book.Price },
            new () { Amount = 50, Price = shoppingCart.Book.Price50 },
            new () { Amount = 100, Price = shoppingCart.Book.Price100 }
        };

        return DiscountCalculations.GetPriceBasedOnQuantity(wholeSaleConfig, shoppingCart.Count);
    }
}