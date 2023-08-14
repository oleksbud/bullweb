using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Bull.Models.ViewModels;

namespace BullWeb.Areas.Admin.Controllers;
[Area("Admin")]
public class BookController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
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
            if (file != null)
            {
                var pathFromRoot = @"images\books";
                SaveFile(file, pathFromRoot, out var fileName);
                model.Book.ImageUrl = pathFromRoot + @"\" + fileName;
            }
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

    private void SaveFile(IFormFile file, string pathFromRoot, out string fileName)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var path = Path.Combine(wwwRootPath, pathFromRoot);

        using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
        {
            file.CopyTo(fileStream);
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