using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;

namespace Bull.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ICategoryRepository Category { get; private set; }
    public IBookRepository Book { get; private set; }
    public ICompanyRepository Company { get; private set;}
    public IShoppingCartRepository ShoppingCart { get; private set;}
    public IApplicationUserRepository ApplicationUser { get; }
    public IOrderHeaderRepository OrderHeader { get; }
    public IOrderDetailRepository OrderDetail { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Category = new CategoryRepository(_context);
        Book = new BookRepository(_context);
        Company = new CompanyRepository(_context);
        ShoppingCart = new ShoppingCartRepository(_context);
        ApplicationUser = new ApplicationUserRepository(_context);
        OrderHeader = new OrderHeaderRepository(_context);
        OrderDetail = new OrderDetailRepository(_context);
    }
    
    public void Save()
    {
        _context.SaveChanges();
    }
}