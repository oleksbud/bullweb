using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bull.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public void Update(ShoppingCart shoppingCart)
    {
        _context.Update(shoppingCart);
    }
    
}