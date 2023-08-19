using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = StaticDetails.RoleAdmin)]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        var categoryList = _unitOfWork.CategoryRepository.GetAll().ToList();
        return View(categoryList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Save();
            TempData["success"] = "Category has created successfully";
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

        var category = _unitOfWork.CategoryRepository.Get(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CategoryRepository.Update(category);
            _unitOfWork.Save();
            TempData["success"] = "Category has updated successfully";
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

        var category = _unitOfWork.CategoryRepository.Get(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }
    
    [HttpPost, ActionName("Delete")]
    public IActionResult UltimateDelete(int? id)
    {
        var category = _unitOfWork.CategoryRepository.Get(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        _unitOfWork.CategoryRepository.Remove(category);
        _unitOfWork.Save();
        TempData["success"] = "Category has deleted successfully";
        return RedirectToAction("Index");
    }
}