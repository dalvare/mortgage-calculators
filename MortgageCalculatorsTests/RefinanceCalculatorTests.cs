using MortgageCalculators;
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
        Assert.NotNull(result);
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
        Assert.True(result.RefinanceLoan.LoanAmount > 0);
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
        Assert.True(result.RefinanceLoan.LoanAmount > 0);
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

        // Act
        var result = calculator.Calculate(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RefinanceLoan.LoanAmount > 0);
    }
    
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