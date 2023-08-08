using BullRazor.Data;
using BullRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BullRazor.Pages.Categories;

public class Create : PageModel
{
    private readonly ApplicationDbContext _context;
    [BindProperty]
    public Category Category { get; set; }

    public Create(ApplicationDbContext context)
    {
        _context = context;
    }
    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        _context.Categories.Add(Category);
        _context.SaveChanges();
        TempData["success"] = "Category has created successfully";
        return RedirectToPage("Index");
    }
}