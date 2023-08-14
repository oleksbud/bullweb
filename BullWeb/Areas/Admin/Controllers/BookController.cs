﻿using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Bull.Models.ViewModels;

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
        return View(books);
    }

    public IActionResult UpdateOrCreate(int? id)
    {
        var model = new BookViewModel()
        {
            CategoryList = _unitOfWork.CategoryRepository.GetSelectOptions()
        };
        if (id == null || id == 0)
        {
            model.Book = new Book();
            return View(model);
        }
        else
        {
            var book = _unitOfWork.BookRepository.Get(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            model.Book = book;
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult UpdateOrCreate(BookViewModel model, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.BookRepository.Add(model.Book);
            _unitOfWork.Save();
            TempData["success"] = "Book has created successfully";
            return RedirectToAction("Index");
        }
        else
        {
            model.CategoryList = _unitOfWork.CategoryRepository.GetSelectOptions();
            return View(model);
        }
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