using MortgageCalculators;
using MortgageCalculators.Extensions;
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

    [Theory]
    [InlineData(8339.58, 8300)]
    [InlineData(303352.25, 303300)]
    [InlineData(199.99, 100)]
    [InlineData(1250.5, 1200)]
    [InlineData(100, 100)]
    [InlineData(0.37, 0)]
    [InlineData(0, 0)]
    [InlineData(-50, -100)]
    public void RoundDownToNearestHundred_ShouldReturnNearestHundredAtOrBelow(decimal amount, decimal expected)
    {
        // Act
        var result = TestCalculator.RoundDown(amount);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1_000_000, 30)]
    [InlineData(6, 100_000)]
    public void CalculateLoanAmount_ShouldThrowArgumentException_WhenRateAndTermAreTooLargeToSolve(decimal interestRate, int termInYears)
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.LoanAmount(1000m, interestRate, termInYears));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void CalculateLoanAmount_ShouldThrowArgumentException_WhenTermOverflowsThePeriodCount()
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.LoanAmount(1000m, 6m, int.MaxValue));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void CalculatePayment_ShouldThrowArgumentException_WhenTermOverflowsThePeriodCount()
    {
        // Act
        var exception = Record.Exception(() => TestCalculator.Payment(200000m, 6m, int.MaxValue));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [InlineData(26, 12, 552.69)]     // biweekly payments, monthly compounding
    [InlineData(52, 12, 276.19)]     // weekly payments, monthly compounding
    [InlineData(12, 2, 1189.65)]     // monthly payments, semi-annual compounding
    [InlineData(12, 365, 1200.97)]   // monthly payments, daily compounding
    [InlineData(12, 12, 1199.10)]    // the default
    public void CalculatePayment_ShouldHonorPaymentAndCompoundingFrequencies(int annualPayments, int annualCompounds, decimal expected)
    {
        // Act
        var result = TestCalculator.Payment(200000m, 6m, 30, annualPayments, annualCompounds);

        // Assert
        Assert.Equal(expected, result, 2);
    }

    [Theory]
    [InlineData(250000, 6, 360)]
    [InlineData(320000, 7, 360)]
    [InlineData(30000, 25, 12)]
    [InlineData(10000000, 1, 480)]
    [InlineData(100000.126, 4.5, 180)]
    public void CalculateAmortization_ShouldReconcileToTheCent(decimal principal, decimal rate, int periods)
    {
        // Act
        var result = TestCalculator.Schedule(principal, rate, periods, principal * 2);

        // Assert
        Assert.Equal(principal.ToDollar(), result.Balance);
        Assert.Equal(result.Balance, result.Schedule.Sum(row => row.Principal));
        Assert.Equal(0m, result.Schedule.Last().Balance);
        Assert.Equal(result.Schedule.Sum(row => row.Interest), result.TotalInterest);
        Assert.Equal(result.Balance + result.TotalInterest, result.TotalPayment);
        Assert.Equal(result.PeriodicPayment, result.PeriodicPayment.ToDollar());
        Assert.All(result.Schedule, row =>
        {
            Assert.Equal(row.Interest, row.Interest.ToDollar());
            Assert.Equal(row.Principal, row.Principal.ToDollar());
            Assert.Equal(row.Balance, row.Balance.ToDollar());
        });
    }

    [Fact]
    public void CalculateAmortization_ShouldOnlyAdjustTheFinalRow()
    {
        // Arrange
        var principal = 250000m;

        // Act
        var result = TestCalculator.Schedule(principal, 6m, 360, 500000m);

        // Assert
        // Every row but the last is principal = payment - interest to the cent.
        Assert.All(result.Schedule.Take(359), row => Assert.Equal(result.PeriodicPayment, row.Principal + row.Interest));
        // The displayed payment is rounded, so the final payment differs by at most half a cent per period.
        var tolerance = 0.005m * 360 + 0.01m;
        var last = result.Schedule.Last();
        Assert.InRange(last.Principal + last.Interest, result.PeriodicPayment - tolerance, result.PeriodicPayment + tolerance);
    }

    private sealed class TestCalculator : MortgageCalculator
    {
        public static decimal Payment(decimal loanAmount, decimal interest, int termInYears) =>
            CalculatePayment(loanAmount, interest, termInYears);

        public static decimal Payment(decimal loanAmount, decimal interest, int termInYears, int annualPayments, int annualCompounds) =>
            CalculatePayment(loanAmount, interest, termInYears, annualPayments, annualCompounds);

        public static decimal LoanAmount(decimal periodPayment, decimal interestRate, int termInYears) =>
            CalculateLoanAmount(periodPayment, interestRate, termInYears);

        public static decimal LoanToValue(decimal loanAmount, decimal homeValue) =>
            CalculateLoanToValue(loanAmount, homeValue);

        public static decimal RoundDown(decimal amount) =>
            RoundDownToNearestHundred(amount);

        public static Amortization Schedule(decimal principal, decimal rate, int periods, decimal homeValue) =>
            CalculateAmortization(principal, rate, periods, DateTime.Now, homeValue);
    }
}
