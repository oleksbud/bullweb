using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bull.DataAccess.Repository.IRepository;

public interface ICategoryRepository: IRepository<Category>
{
    void Update(Category category);
    IEnumerable<SelectListItem> GetSelectOptions();
}