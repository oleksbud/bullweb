using Bull.Models.Models;

namespace Bull.DataAccess.Repository.IRepository;

public interface IBookRepository: IRepository<Book>
{
    void Update(Book book);
}