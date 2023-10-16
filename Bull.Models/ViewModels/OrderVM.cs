using Bull.Models.Models;

namespace Bull.Models.ViewModels;

public class OrderVM
{
    public OrderHeader OrderHeader { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}