using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Controllers;

public class CategoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}