using BullRazor.Data;
using BullRazor.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BullRazor.Pages.Categories;

public class Create : PageModel
{
    private readonly ApplicationDbContext _context;
    public Category Category { get; set; }

    public Create(ApplicationDbContext context)
    {
        _context = context;
    }
    public void OnGet()
    {
        
    }
}