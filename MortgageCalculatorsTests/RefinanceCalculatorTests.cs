using MortgageCalculators;
using MortgageCalculators.Extensions;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Validators;
using FluentValidation.TestHelper;

namespace MortgageCalculatorsTests;

public class RefinanceCalculatorTests
{
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenValidRequestProvided()
    {
        // Arrange
        var request = new RefinanceCalculatorRequest
        {
            HomeValue = 400000m,
            CurrentLoan = new RefinanceCurrentLoanRequest
            {
                OriginalLoanAmount = 320000m,
                InterestRate = 7m,
                Term = 30,
                Pmi = 0.5m,
                MonthsPaid = 24
            },
            RefinanceLoan = new RefinanceRefinanceLoanRequest
            {
                InterestRate = 5.750m,
                Term = 15,
                Points = 1.0m,
                OriginationFees = 0m,
                ClosingCosts = 1200m,
                YearsBeforeSale = 5,
            },
            TaxRates = new TaxRatesRequest
            {
                MarginalIncomeTaxRate = 2.0m , 
                StateTaxRate = 5.0m
            },
        };
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        // The five-year analysis window sits inside both terms, so payments run for all 60 months.
        Assert.Equal(313263.83m, result.CurrentLoan.RemainingBalance);
        Assert.Equal(2128.97m, result.CurrentLoan.MonthlyPayment);
        Assert.Equal(127738.08m, result.CurrentLoan.TotalMonthlyPayments);
        Assert.Equal(291671.73m, result.CurrentLoan.BalanceAtSale);
        Assert.Equal(313263.83m, result.RefinanceLoan.LoanAmount);
        Assert.Equal(2601.37m, result.RefinanceLoan.MonthlyPayment);
        Assert.Equal(156082.47m, result.RefinanceLoan.TotalMonthlyPayments);
        Assert.Equal(236985.78m, result.RefinanceLoan.BalanceAtSale);
        Assert.Equal(-28344.39m, result.MonthlyPaymentSavings);
        Assert.Equal(1843.91m, result.TaxSavingsLosses);
        Assert.Equal(-54685.95m, result.BalanceLosses);
        Assert.Equal(4332.64m, result.TotalClosingCosts);
        Assert.Equal(20165.02m, result.TotalBenefit);
    }
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenZeroYearsBeforeSaleProvided()
    {
        // Arrange
        var request = new RefinanceCalculatorRequest
        {
            HomeValue = 400000m,
            CurrentLoan = new RefinanceCurrentLoanRequest
            {
                OriginalLoanAmount = 320000m,
                InterestRate = 7m,
                Term = 30,
                Pmi = 0.5m,
                MonthsPaid = 24
            },
            RefinanceLoan = new RefinanceRefinanceLoanRequest
            {
                InterestRate = 5.750m,
                Term = 15,
                Points = 1.0m,
                OriginationFees = 0m,
                ClosingCosts = 1200m,
                YearsBeforeSale = 0,
            },
            TaxRates = new TaxRatesRequest
            {
                MarginalIncomeTaxRate = 36.0m , 
                StateTaxRate = 5.0m
            },
        };
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        AssertZeroHoldingPeriod(result);
    }
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenZeroMonthsPaidProvided()
    {
        // Arrange
        var request = new RefinanceCalculatorRequest
        {
            HomeValue = 400000m,
            CurrentLoan = new RefinanceCurrentLoanRequest
            {
                OriginalLoanAmount = 320000m,
                InterestRate = 7m,
                Term = 30,
                Pmi = 0.5m,
                MonthsPaid = 0
            },
            RefinanceLoan = new RefinanceRefinanceLoanRequest
            {
                InterestRate = 5.750m,
                Term = 15,
                Points = 1.0m,
                OriginationFees = 0m,
                ClosingCosts = 1200m,
                YearsBeforeSale = 0,
            },
            TaxRates = new TaxRatesRequest
            {
                MarginalIncomeTaxRate = 28.0m , 
                StateTaxRate = 5.0m
            },
        };
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        AssertZeroHoldingPeriod(result);
    }
    
    [Fact]
    public void Calculate_WithLTVOver100()
    {
        // Arrange
        var request = new RefinanceCalculatorRequest
        {
            HomeValue = 400000m,
            CurrentLoan = new RefinanceCurrentLoanRequest
            {
                InterestRate = 6.25m,
                MonthsPaid = 4,
                OriginalLoanAmount = 420000m,
                Pmi = 4m,
                Term = 30
            },
            RefinanceLoan = new RefinanceRefinanceLoanRequest
            {
                ClosingCosts = 500m,
                InterestRate = 5.25m,
                OriginationFees = 0m,
                Pmi = 4m,
                Points = 1.5m,
                Term = 30,
                YearsBeforeSale = 0
            },
            TaxRates = new TaxRatesRequest
            {
                MarginalIncomeTaxRate = 5.25m,
                StateTaxRate = 5.3m
            }
        };
        var calculator = new RefinanceCalculator();
        var validator = new RefinanceRequestValidator();
        
        // Act
        var result = calculator.Calculate(request);
        var validationResult = validator.TestValidate(request);
        
        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        AssertZeroHoldingPeriod(result);
    }
    
    [Fact]
    public void Calculate_ShouldStopCountingPayments_WhenRefinanceLoanIsPaidOffBeforeSale()
    {
        // Arrange: a 15-year refinance held for 30 years is paid off halfway through the analysis window.
        var request = ValidRequest();
        request.RefinanceLoan.Term = 15;
        request.RefinanceLoan.YearsBeforeSale = 30;
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        Assert.Equal(result.RefinanceLoan.Amortization.TotalPayment.ToDollar(), result.RefinanceLoan.TotalMonthlyPayments);
        Assert.Equal(0m, result.RefinanceLoan.BalanceAtSale);
        Assert.Equal(result.RefinanceLoan.Amortization.TotalInterest.ToDollar(), result.RefinanceLoan.InterestPaid);
        // The current loan has 336 payments left, so it too stops short of the 360-month window.
        AssertWithinADollar(result.CurrentLoan.MonthlyPayment * 336, result.CurrentLoan.TotalMonthlyPayments);
        Assert.Equal(0m, result.CurrentLoan.BalanceAtSale);
        Assert.True(result.MonthlyPaymentSavings > 0);
    }

    [Fact]
    public void Calculate_ShouldStopCountingPayments_WhenCurrentLoanIsPaidOffBeforeSale()
    {
        // Arrange: 24 payments remain on the current loan, but the analysis window is 60 months.
        var request = ValidRequest();
        request.CurrentLoan.Term = 10;
        request.CurrentLoan.MonthsPaid = 96;
        request.RefinanceLoan.YearsBeforeSale = 5;
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        AssertWithinADollar(result.CurrentLoan.MonthlyPayment * 24, result.CurrentLoan.TotalMonthlyPayments);
        Assert.Equal(0m, result.CurrentLoan.BalanceAtSale);
        AssertWithinADollar(result.RefinanceLoan.MonthlyPayment * 60, result.RefinanceLoan.TotalMonthlyPayments);
        Assert.True(result.RefinanceLoan.BalanceAtSale > 0);
    }

    [Fact]
    public void Calculate_ShouldReportPointsAndOriginationFees_OnTheRefinanceLoan()
    {
        // Arrange
        var request = ValidRequest();
        request.RefinanceLoan.Points = 1.0m;
        request.RefinanceLoan.OriginationFees = 1.0m;
        request.RefinanceLoan.ClosingCosts = 1200m;
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        var remainingBalance = result.CurrentLoan.RemainingBalance;
        Assert.Equal((remainingBalance * 0.01m).ToDollar(), result.RefinanceLoan.Points);
        Assert.Equal((remainingBalance * 0.01m).ToDollar(), result.RefinanceLoan.OriginationFees);
        Assert.Equal(result.RefinanceLoan.Points + result.RefinanceLoan.OriginationFees + 1200m, result.TotalClosingCosts);
    }

    [Fact]
    public void Calculate_ShouldUseTheOriginalLoanAmount_WhenNoPaymentsHaveBeenMade()
    {
        // Arrange
        var request = ValidRequest();
        request.CurrentLoan.MonthsPaid = 0;
        var calculator = new RefinanceCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        Assert.Equal(request.CurrentLoan.OriginalLoanAmount, result.CurrentLoan.RemainingBalance);
        Assert.Equal(request.CurrentLoan.OriginalLoanAmount, result.RefinanceLoan.LoanAmount);
    }

    /// <summary>
    /// Totals are built from the unrounded payment, so a product of the rounded payment lands within a dollar.
    /// </summary>
    private static void AssertWithinADollar(decimal expected, decimal actual)
    {
        Assert.InRange(actual, expected - 1m, expected + 1m);
    }

    /// <summary>
    /// Selling immediately after refinancing yields no savings or losses, only the closing costs.
    /// </summary>
    private static void AssertZeroHoldingPeriod(RefinanceCalculatorResponse result)
    {
        Assert.True(result.RefinanceLoan.LoanAmount > 0);
        Assert.Equal(result.CurrentLoan.RemainingBalance, result.RefinanceLoan.LoanAmount);
        Assert.Equal(0m, result.CurrentLoan.TotalMonthlyPayments);
        Assert.Equal(0m, result.RefinanceLoan.TotalMonthlyPayments);
        Assert.Equal(0m, result.CurrentLoan.InterestPaid);
        Assert.Equal(0m, result.RefinanceLoan.InterestPaid);
        Assert.Equal(result.CurrentLoan.RemainingBalance, result.CurrentLoan.BalanceAtSale);
        Assert.Equal(result.RefinanceLoan.LoanAmount, result.RefinanceLoan.BalanceAtSale);
        Assert.Equal(0m, result.MonthlyPaymentSavings);
        Assert.Equal(0m, result.TaxSavingsLosses);
        Assert.Equal(0m, result.BalanceLosses);
        Assert.Equal(0m, result.TotalLosses);
        Assert.True(result.TotalClosingCosts > 0);
        Assert.Equal(-result.TotalClosingCosts, result.TotalBenefit);
    }

    private static RefinanceCalculatorRequest ValidRequest() => new()
    {
        HomeValue = 400000m,
        CurrentLoan = new RefinanceCurrentLoanRequest
        {
            OriginalLoanAmount = 320000m,
            InterestRate = 7m,
            Term = 30,
            Pmi = 0.5m,
            MonthsPaid = 24
        },
        RefinanceLoan = new RefinanceRefinanceLoanRequest
        {
            InterestRate = 5.750m,
            Term = 15,
            Points = 1.0m,
            OriginationFees = 0m,
            ClosingCosts = 1200m,
            YearsBeforeSale = 5,
        },
        TaxRates = new TaxRatesRequest
        {
            MarginalIncomeTaxRate = 2.0m,
            StateTaxRate = 5.0m
        },
    };

    [Fact]
    public void Validate_ValidRefinanceCurrentLoanRequest()
    {
        var request = new RefinanceCurrentLoanRequest
        {
            InterestRate = 5.0m,
            Term = 30,
            Pmi = 0.5m,
            OriginalLoanAmount = 200000m,
            MonthsPaid = 24
        };
        var validator = new RefinanceCurrentLoanRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidRefinanceCurrentLoanRequest()
    {
        var request = new RefinanceCurrentLoanRequest
        {
            InterestRate = 0.0m, // Invalid
            Term = 0, // Invalid
            Pmi = 11.0m, // Invalid
            OriginalLoanAmount = -1000m, // Invalid
            MonthsPaid = 500 // Invalid (over 480)
        };
        var validator = new RefinanceCurrentLoanRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.InterestRate);
        result.ShouldHaveValidationErrorFor(r => r.Term);
        result.ShouldHaveValidationErrorFor(r => r.Pmi);
        result.ShouldHaveValidationErrorFor(r => r.OriginalLoanAmount);
        result.ShouldHaveValidationErrorFor(r => r.MonthsPaid);
    }

    [Fact]
    public void Validate_ValidRefinanceRefinanceLoanRequest()
    {
        var request = new RefinanceRefinanceLoanRequest
        {
            InterestRate = 4.5m,
            Term = 30,
            Pmi = 0.5m,
            Points = 1.0m,
            OriginationFees = 1.1m,
            ClosingCosts = 3000m,
            YearsBeforeSale = 5
        };
        var validator = new RefinanceRefinanceLoanRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidRefinanceRefinanceLoanRequest()
    {
        var request = new RefinanceRefinanceLoanRequest
        {
            InterestRate = -2.0m, // Invalid
            Term = 0, // Invalid
            Pmi = 12.0m, // Invalid
            Points = -1.0m, // Invalid
            OriginationFees = -100m, // Invalid
            ClosingCosts = -500m, // Invalid
            YearsBeforeSale = 40 // Invalid (over 30)
        };
        var validator = new RefinanceRefinanceLoanRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.InterestRate);
        result.ShouldHaveValidationErrorFor(r => r.Term);
        result.ShouldHaveValidationErrorFor(r => r.Pmi);
        result.ShouldHaveValidationErrorFor(r => r.Points);
        result.ShouldHaveValidationErrorFor(r => r.OriginationFees);
        result.ShouldHaveValidationErrorFor(r => r.ClosingCosts);
        result.ShouldHaveValidationErrorFor(r => r.YearsBeforeSale);
    }
    
    [Fact]
    public void Validate_ValidRefinanceTaxRatesRequest()
    {
        var request = new TaxRatesRequest()
        {
            MarginalIncomeTaxRate = 36.0m,
            StateTaxRate = 5.0m
        };
        var validator = new TaxRatesRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void Validate_InvalidRefinanceTaxRatesRequest()
    {
        var request = new TaxRatesRequest()
        {
            MarginalIncomeTaxRate = 51.0m,
            StateTaxRate = 20.0m
        };
        var validator = new TaxRatesRequestValidator();

        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.MarginalIncomeTaxRate);
        result.ShouldHaveValidationErrorFor(r => r.StateTaxRate);
    }
}