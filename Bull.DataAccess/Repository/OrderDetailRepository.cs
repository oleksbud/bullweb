using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;

namespace Bull.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _context;

    public OrderDetailRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public void Update(OrderDetail orderDetail)
    {
        _context.OrderDetails.Update(orderDetail);
    }
}