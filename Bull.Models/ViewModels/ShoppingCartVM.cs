using Bull.Models.Models;

namespace Bull.Models.ViewModels;

public class ShoppingCartVM
{
    public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
    public double Total { get; set; }
}