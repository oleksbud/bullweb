using BullRazor.Data;
using BullRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BullRazor.Pages.Categories;

public class Index : PageModel
{
    private readonly ApplicationDbContext _context;
    public List<Category> CategoryList { get; set; }

    public Index(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        CategoryList = _context.Categories.ToList();
    }
}