namespace Bull.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get;  }
    IBookRepository BookRepository { get; }

    public void Save();
}