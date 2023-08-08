using BullRazor.Data;
using BullRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BullRazor.Pages.Categories;

public class Delete : PageModel
{
    private readonly ApplicationDbContext _context;
    [BindProperty]
    public Category? Category { get; set; }

    public Delete(ApplicationDbContext context)
    {
        _context = context;
    }
    public void OnGet(int? id)
    {
        if (id != null && id != 0)
        {
            Category = _context.Categories.Find(id);
        }
    }

    public IActionResult OnPost()
    {
        var category = _context.Categories.FirstOrDefault(x => x.Id == Category.Id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
        TempData["success"] = "Category has deleted successfully";
        return RedirectToPage("Index");
    }
}