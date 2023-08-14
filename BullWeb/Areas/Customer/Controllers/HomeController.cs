using System.Diagnostics;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
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
       
        return View(book);
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