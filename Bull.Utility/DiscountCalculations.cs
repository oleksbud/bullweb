using Bull.Models.Models;

namespace Bull.Utility;

public static class DiscountCalculations
{
    public static double GetPriceBasedOnQuantity(List<WholeSaleConfigItem> wholeSaleConfig, int amount)
    {
        if (wholeSaleConfig.Count == 0 || amount == 0)
        {
            return 0;
        }

        var orderedWholeSalePrices = wholeSaleConfig
            .OrderByDescending(x => x.Amount).ToList();

        foreach (var priceItem in orderedWholeSalePrices)
        {
            if (priceItem.Amount < amount)
            {
                return priceItem.Price;
            }
        }

        return orderedWholeSalePrices.First().Price;
    }
}