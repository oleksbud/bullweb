using Bull.Models.Models;

namespace Bull.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository: IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
} 