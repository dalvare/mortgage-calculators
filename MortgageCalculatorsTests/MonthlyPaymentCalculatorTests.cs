using MortgageCalculators;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Validators;
using FluentValidation.TestHelper;

namespace MortgageCalculatorsTests;

public class MonthlyPaymentCalculatorTests
{
    private readonly MonthlyPaymentRequestValidator _validator = new();

    [Fact]
    public void Calculate_ReturnsCorrectMonthlyPayment_NoPmi()
    {
        // Arrange
        var request = new MonthlyPaymentRequest
        {
            LoanAmount = 280000m,
            HomeValue = 350000m, 
            InterestRate = 6.5m,
            Term = 30,
            AnnualTaxes = 3000m, 
            AnnualInsurance = 1500m,
            Pmi = 1.0m
        };
        var calculator = new MonthlyPaymentCalculator();
        
        // Act
        var result = calculator.Calculate(request);

        // Assert
        const decimal expectedMonthlyPayment = 2144.79m;
        Assert.Equal(expectedMonthlyPayment, result.MonthlyPayment, 2);
        Assert.Equal(0, result.Amortization.MonthsWithPmi);
        Assert.Equal(0, result.MonthlyPmi);
        Assert.True(result.LoanToValue <= 80);
    }
    
    [Fact]
    public void Calculate_ReturnsCorrectMonthlyPayment_WithPmi()
    {
        // Arrange
        var request = new MonthlyPaymentRequest
        {
            LoanAmount = 300000m,
            HomeValue = 350000m,
            InterestRate = 6.5m,
            Term = 30,
            AnnualTaxes = 3000m,
            AnnualInsurance = 1500m,
            Pmi = 1.0m
        };
        var calculator = new MonthlyPaymentCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        const decimal expectedMonthlyPayment = 2521.20m;
        const decimal expectedMonthlyPmi = 250.00m;
        const int expectedMonthsWithPmi = 62;

        Assert.Equal(expectedMonthlyPayment, result.MonthlyPayment, 2);
        Assert.Equal(expectedMonthsWithPmi, result.Amortization.MonthsWithPmi);
        Assert.Equal(expectedMonthlyPmi, result.MonthlyPmi, 2);
        Assert.True(result.LoanToValue > 80);
    }
    
    [Fact]
    public void Validate_ValidRequest()
    {
        var request = new MonthlyPaymentRequest
        {
            LoanAmount = 280000m,
            HomeValue = 350000m, 
            InterestRate = 6.5m,
            Term = 30,
            AnnualTaxes = 3000m, 
            AnnualInsurance = 1500m,
            Pmi = 1.0m
        };
        
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void Validate_InvalidRequest()
    {
        var request = new MonthlyPaymentRequest
        {
            LoanAmount = 280000m,
            HomeValue = 350000m, 
            InterestRate = 6.5m,
            Term = 30,
            AnnualTaxes = 3000m, 
            AnnualInsurance = 1500m,
            Pmi = 11.0m
        };
        
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.LoanAmount);
        result.ShouldHaveValidationErrorFor(r => r.Pmi);
    }
}