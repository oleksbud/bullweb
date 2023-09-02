using Bull.Models.Models;
using Bull.Utility;
using Xunit;
using FluentAssertions;
namespace Bull.Test.AreaCustomer;

public class CartControllerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(49)]
    public void ShouldReturnTheProperPriceForLessThan50Items(int amount)
    {
        // Configuration
        var wholeSaleConfig = new List<WholeSaleConfigItem>
        {
            new() { Amount = 0, Price = 20.00 },
            new () { Amount = 50, Price = 15.00 },
            new () { Amount = 100, Price = 10.00 }
        };
        var expectedValue = 20;

        // Evaluation
        var actualValue = DiscountCalculations.GetPriceBasedOnQuantity(wholeSaleConfig, amount);

        // Comparison
        actualValue.Should().Be(expectedValue);
    }
    
    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    [InlineData(400)]
    public void ShouldReturnTheProperPriceForAboveThan100Items(int amount)
    {
        // Configuration
        var wholeSaleConfig = new List<WholeSaleConfigItem>
        {
            new() { Amount = 0, Price = 20.00 },
            new () { Amount = 50, Price = 15.00 },
            new () { Amount = 100, Price = 10.00 }
        };
        var expectedValue = 10;

        // Evaluation
        var actualValue = DiscountCalculations.GetPriceBasedOnQuantity(wholeSaleConfig, amount);

        // Comparison
        actualValue.Should().Be(expectedValue);
    }
}