using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Models.ViewModels;
using Bull.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BullWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartVM ShoppingCartVm { get; set; }

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
    
    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        var includeDictionaries = new List<string> { "Book" };

        ShoppingCartVm.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId,
            includeProperties: includeDictionaries);
        
        ShoppingCartVm.OrderHeader.OrderDate = DateTime.Now;
        ShoppingCartVm.OrderHeader.ApplicationUserId = userId;
        
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        

        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // it is a regular customer account and we need to capture payment
            ShoppingCartVm.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            ShoppingCartVm.OrderHeader.OrderStatus = StaticDetails.StatusPending;
        }
        else
        {
            // it is a company account
            ShoppingCartVm.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
            ShoppingCartVm.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
        }
        
        _unitOfWork.OrderHeader.Add(ShoppingCartVm.OrderHeader);
        _unitOfWork.Save();


        foreach (var cart in ShoppingCartVm.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                BookId = cart.BookId,
                OrderHeaderId = ShoppingCartVm.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // it is a regular customer account and we need to capture payment
            // stripe logic
            const string domain = "https://localhost:7289/";
            var successUrl = $"customer/cart/OrderConfirmation?id={ShoppingCartVm.OrderHeader.Id}";
            var cancelUrl = "customer/cart/index";
            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain + successUrl,
                CancelUrl = domain + cancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in ShoppingCartVm.ShoppingCartList)
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
            
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVm.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        
        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVm.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        var dictionary = new List<string> { "ApplicationUser" };
        var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, dictionary);

        if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
        {
            // it is an order by customer
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatuses(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId)
                .ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
        }
        
        return View(id);
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