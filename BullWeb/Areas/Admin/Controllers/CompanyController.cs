using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Bull.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.RoleAdmin)] 
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        var company = _unitOfWork.CompanyRepository.GetAll().ToList();
        return View(company);
    }
    
    public IActionResult UpdateOrCreate(int? id)
    {
        if (id == null || id == 0)
        {
            return View(new Company());
        }
        else
        {
            var company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            
            return View(company);
        }
    }
    
    [HttpPost]
    public IActionResult UpdateOrCreate(Company company)
    {
        if (ModelState.IsValid)
        {

            if (company.Id == 0)
            {
                _unitOfWork.CompanyRepository.Add(company);
            }
            else
            {
                _unitOfWork.CompanyRepository.Update(company);
            }

            _unitOfWork.Save();
            TempData["success"] = "Company has created successfully";
            return RedirectToAction("Index");
        }
        else
        {
            return View(company);
        }
    }
    
    /*public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        return View(company);
    }*/
    
    [HttpPost, ActionName("Delete")]
    public IActionResult UltimateDelete(int? id)
    {
        var company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        _unitOfWork.CompanyRepository.Remove(company);
        _unitOfWork.Save();
        TempData["success"] = "Company has deleted successfully";
        return RedirectToAction("Index");
    }

    #region ApiEndpoints

    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.CompanyRepository.GetAll().ToList();
        
        return Json(new { data = companyList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);

        if (company == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        return Json(new { success = true });
    }

    #endregion

}