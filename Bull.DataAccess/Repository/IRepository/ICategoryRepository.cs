using Bull.Models.Models;

namespace Bull.DataAccess.Repository.IRepository;

public interface ICategoryRepository: IRepository<Category>
{
    void Update(Category category);
    void Save();
}