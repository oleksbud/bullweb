using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Bull.Models.Models;

namespace Bull.DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderHeaderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public void Update(OrderHeader orderHeader)
    {
        _context.OrderHeaders.Update(orderHeader);
    }

    public void UpdateStatuses(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);

        if (orderFromDb != null)
        {
            orderFromDb.OrderStatus = orderStatus;
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIdentId)
    {
        var orderFromDb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);

        if (orderFromDb != null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIdentId))
            {
                orderFromDb.PaymentIntentId = paymentIdentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}