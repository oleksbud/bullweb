namespace Bull.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get;  }
    IBookRepository Book { get; }
    ICompanyRepository Company { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }

    public void Save();
}