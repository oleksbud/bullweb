using System.Diagnostics;
using System.Security.Claims;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var dictionary = new List<string> { "Category" };
        IEnumerable<Book> books = _unitOfWork.BookRepository.GetAll(dictionary);
        return View(books);
    }

    public IActionResult Details(int id)
    {
        var dictionary = new List<string> { "Category" };
        var book = _unitOfWork.BookRepository.Get(x => x.Id == id, dictionary);
        if (book == null)
        {
            return NotFound();
        }
        _logger.Log(LogLevel.Information, "Book requested: {Title}",book.Title);
       
        ShoppingCart cart = new()
        {
           BookId = id,
           Book = book,
           Count = 1,
        };
        
        return View(cart);
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart cart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        cart.ApplicationUserId = userId;
        
        _unitOfWork.ShoppingCartRepository.Add(cart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}