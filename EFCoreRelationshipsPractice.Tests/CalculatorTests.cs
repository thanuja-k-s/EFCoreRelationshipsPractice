using EFCoreRelationshipsPractice;

namespace EFCoreRelationshipsPractice.Tests;

public class CalculatorTests
{
    //[Fact]
    //public void Add_TwoNumbers_ReturnsSum()
    //{
    //    var calculator = new Calculator();

    //    int result = calculator.Add(10, 20);

    //    Assert.Equal(30, result);
    //}

    [Theory]
    [InlineData(10, 20, 30)]
    [InlineData(5, 5, 10)]
    [InlineData(0, 10, 10)]
    [InlineData(-5, 5, 0)]
    [InlineData(100, 200, 300)]
    public void Add_TwoNumbers_ReturnsSum(
        int a,
        int b,
        int expected)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        int result = calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}