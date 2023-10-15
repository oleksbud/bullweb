using Bull.Models.Models;

namespace Bull.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository: IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
    void UpdateStatuses(int id, string orderStatus, string? paymentStatus = null);
    void UpdateStripePaymentId(int id, string sessionId, string paymentIdentId);
}