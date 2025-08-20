using Calculators;
using Calculators.Models;
using Calculators.Validation.Validators;
using FluentValidation.TestHelper;

namespace CalculatorsTests;

public class AffordabilityCalculatorTests
{
    private readonly AffordabilityRequestValidator _validator = new();
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenValidRequestProvidedWithoutPmi()
    {
        // Arrange
        var request = new AffordabilityRequest
        {
            Pmi = 0,
            AnnualTaxes = 2700,
            AnnualInsurance = 2400,
            DownPayment = 20.0m,
            InterestRate = 6.0m,
            Term = 30,
            FrontRatio = 28.0m,
            BackRatio = 36.0m,
            TotalMonthlyIncome = 8000.0m,
            TotalMonthlyExpenses = 1000.0m
        };
        var calculator = new AffordabilityCalculator();
        
        // Act
        var result = calculator.Calculate(request);

        // Assert
        var expectedHomeValue = 303352.25m;
        Assert.Equal(expectedHomeValue, result.HomeValue);
    }
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenValidRequestProvidedWithPmi()
    {
        // Arrange
        var request = new AffordabilityRequest
        {
            DownPayment = 20,
            InterestRate = 5,
            Term = 30,
            FrontRatio = 28,
            BackRatio = 36,
            TotalMonthlyIncome = 10000,
            TotalMonthlyExpenses = 2000,
            AnnualTaxes = 12000,
            AnnualInsurance = 2400,
            Pmi = 1
        };
        var calculator = new AffordabilityCalculator();

        // Act
        var response = calculator.Calculate(request);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.HomeValue > 0);
        Assert.True(response.LoanAmount > 0);
        Assert.True(response.MonthlyPrincipalAndInterest > 0);
    }
    
    [Fact]
    public void Validate_ValidAffordabilityRequest()
    {
        var request = new AffordabilityRequest
        {
            Pmi = 0.5m,
            AnnualTaxes = 3000m,
            AnnualInsurance = 1500m,
            DownPayment = 20.0m,
            InterestRate = 5.0m,
            Term = 30,
            FrontRatio = 28.0m,
            BackRatio = 36.0m,
            TotalMonthlyIncome = 8000.0m,
            TotalMonthlyExpenses = 1000.0m
        };
        
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidAffordabilityRequest()
    {
        var request = new AffordabilityRequest
        {
            Pmi = 11.0m, // Invalid PMI
            AnnualTaxes = -100m, // Invalid taxes
            AnnualInsurance = -10m, // Invalid insurance
            DownPayment = -5.0m, // Invalid down payment
            InterestRate = 101m, // Invalid interest rate
            Term = 0, // Invalid term
            FrontRatio = 0m, // Invalid front ratio
            BackRatio = 101m, // Invalid back ratio
            TotalMonthlyIncome = 1000.0m,
            TotalMonthlyExpenses = 2000.0m // Expenses > Income
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Pmi);
        result.ShouldHaveValidationErrorFor(r => r.AnnualTaxes);
        result.ShouldHaveValidationErrorFor(r => r.AnnualInsurance);
        result.ShouldHaveValidationErrorFor(r => r.DownPayment);
        result.ShouldHaveValidationErrorFor(r => r.InterestRate);
        result.ShouldHaveValidationErrorFor(r => r.Term);
        result.ShouldHaveValidationErrorFor(r => r.FrontRatio);
        result.ShouldHaveValidationErrorFor(r => r.BackRatio);
        result.ShouldHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }
}