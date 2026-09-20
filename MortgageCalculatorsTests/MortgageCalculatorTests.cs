using MortgageCalculators;

namespace MortgageCalculatorsTests;

public class MortgageCalculatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void CalculateLoanToValue_ShouldThrow_WhenHomeValueIsNotPositive(decimal homeValue)
    {
        // Arrange
        var loanAmount = 200000m;

        // Act
        var exception = Record.Exception(() => TestCalculator.LoanToValue(loanAmount, homeValue));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void CalculateLoanToValue_ShouldReturnPercentage_WhenHomeValueIsPositive()
    {
        // Arrange
        var loanAmount = 240000m;
        var homeValue = 300000m;

        // Act
        var result = TestCalculator.LoanToValue(loanAmount, homeValue);

        // Assert
        Assert.Equal(80m, result);
    }

    private sealed class TestCalculator : MortgageCalculator
    {
        public static decimal LoanToValue(decimal loanAmount, decimal homeValue) =>
            CalculateLoanToValue(loanAmount, homeValue);
    }
}
