﻿using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BullWeb.Areas.Admin.Controllers;
[Area("Admin")]
public class BookController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BookController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        var books = _unitOfWork.BookRepository.GetAll().ToList();
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll()
            .Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            });
        return View(books);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Book book)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.BookRepository.Add(book);
            _unitOfWork.Save();
            TempData["success"] = "Book has created successfully";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var book = _unitOfWork.BookRepository.Get(x => x.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(Book book)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.BookRepository.Update(book);
            _unitOfWork.Save();
            TempData["success"] = "Book has updated successfully";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var book = _unitOfWork.BookRepository.Get(x => x.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }
    
    [HttpPost, ActionName("Delete")]
    public IActionResult UltimateDelete(int? id)
    {
        var book = _unitOfWork.BookRepository.Get(x => x.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        _unitOfWork.BookRepository.Remove(book);
        _unitOfWork.Save();
        TempData["success"] = "Book has deleted successfully";
        return RedirectToAction("Index");
    }
}