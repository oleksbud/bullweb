using System.Linq.Expressions;
using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;

namespace Bull.DataAccess.Repository;

public class BookRepository : Repository<Book>, IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public void Update(Book book)
    {
        _context.Update(book);
    }
}