using MortgageCalculators;
using MortgageCalculators.Models;

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

    [Fact]
    public void CalculatePayment_ShouldThrow_WhenTermIsZero()
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.Payment(200000m, 6m, 0));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void CalculatePayment_ShouldThrow_WhenInterestRateIsNegative()
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.Payment(200000m, -1m, 30));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void CalculatePayment_ShouldSpreadPrincipalEvenly_WhenInterestRateIsZero()
    {
        // Arrange
        var loanAmount = 240000m;
        var termInYears = 10;

        // Act
        var result = TestCalculator.Payment(loanAmount, 0m, termInYears);

        // Assert
        Assert.Equal(2000m, result);
    }

    [Fact]
    public void CalculateLoanAmount_ShouldReturnTotalOfPayments_WhenInterestRateIsZero()
    {
        // Arrange
        var monthlyPayment = 2000m;
        var termInYears = 10;

        // Act
        var result = TestCalculator.LoanAmount(monthlyPayment, 0m, termInYears);

        // Assert
        Assert.Equal(240000m, result);
    }

    [Fact]
    public void CalculateAmortization_ShouldThrow_WhenPeriodCountIsZero()
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.Schedule(200000m, 6m, 0, 250000m));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void CalculateAmortization_ShouldPayOffTheLoan_WhenTermIsShorterThanOneYear()
    {
        // Arrange
        var principal = 10000m;
        var periods = 6;

        // Act
        var result = TestCalculator.Schedule(principal, 6m, periods, 250000m);

        // Assert
        Assert.Equal(periods, result.Schedule.Count);
        Assert.True(result.PeriodicPayment > 0);
        Assert.Equal(0m, result.Schedule.Last().Balance);
    }

    private sealed class TestCalculator : MortgageCalculator
    {
        public static decimal Payment(decimal loanAmount, decimal interest, int termInYears) =>
            CalculatePayment(loanAmount, interest, termInYears);

        public static decimal LoanAmount(decimal periodPayment, decimal interestRate, int termInYears) =>
            CalculateLoanAmount(periodPayment, interestRate, termInYears);

        public static decimal LoanToValue(decimal loanAmount, decimal homeValue) =>
            CalculateLoanToValue(loanAmount, homeValue);

        public static Amortization Schedule(decimal principal, decimal rate, int periods, decimal homeValue) =>
            CalculateAmortization(principal, rate, periods, DateTime.Now, homeValue);
    }
}
