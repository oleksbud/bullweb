using System.Linq.Expressions;
using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Microsoft.EntityFrameworkCore;

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
        var storedBook = _context.Books.FirstOrDefault(x => x.Id == book.Id);
        if (storedBook != null)
        {
            storedBook.Title = book.Title;
            storedBook.Isbn = book.Isbn;
            storedBook.Author = book.Author;
            storedBook.Description = book.Description;
            storedBook.CategoryId = book.CategoryId;
            storedBook.ListPrice = book.ListPrice;
            storedBook.Price = book.Price;
            storedBook.Price50 = book.Price50;
            storedBook.Price100 = book.Price100;
            
            if (book.ImageUrl != null)
            {
                storedBook.ImageUrl = book.ImageUrl;
            }
        }
    }
}