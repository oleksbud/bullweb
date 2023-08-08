﻿using BullRazor.Data;
using BullRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BullRazor.Pages.Categories;

public class Edit : PageModel
{
    private readonly ApplicationDbContext _context;
    [BindProperty]
    public Category? Category { get; set; }

    public Edit(ApplicationDbContext context)
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
        if (ModelState.IsValid && Category != null)
        {
            _context.Categories.Update(Category);
            _context.SaveChanges(); 
            TempData["success"] = "Category has updated successfully";
            return RedirectToPage("Index");
        }

        return Page();
    }
}