using Bull.Models.Models;

namespace Bull.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository: IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
}