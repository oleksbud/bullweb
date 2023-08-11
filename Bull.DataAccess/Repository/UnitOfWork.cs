using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;

namespace Bull.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ICategoryRepository CategoryRepository { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        CategoryRepository = new CategoryRepository(_context);
    }
    
    public void Save()
    {
        _context.SaveChanges();
    }
}