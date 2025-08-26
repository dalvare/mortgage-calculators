using MortgageCalculators;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Validators;
using FluentValidation.TestHelper;

namespace MortgageCalculatorsTests;

public class LoanComparisonCalculatorTests
{
    private readonly LoanComparisonRequestValidator _validator = new();
    
    [Fact]
    public void Calculate_ReturnsCorrectTotalSavings_WhenSecondLoanIsCheaper()
    {
        var request = new LoanComparisonRequest
        {
            LoanAmount = 200000,
            Loans = [
                new LoanComparisonRequestLoan
                {
                    InterestRate = 5,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 3000,
                    HomeValue = 250000,
                    Pmi = 0
                },
                new LoanComparisonRequestLoan()
                {
                    InterestRate = 4,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 3000,
                    HomeValue = 250000,
                    Pmi = 0
                }
            ]
        };
        var calculator = new LoanComparisonCalculator();

        var response = calculator.Calculate(request);

        Assert.Equal(request.LoanAmount, response.LoanAmount);
        Assert.True(response.TotalSavings > 0);
        Assert.Equal(2, response.Loans.Count);
    }

    [Fact]
    public void Calculate_ReturnsZeroTotalSavings_WhenLoansHaveIdenticalTotalPayments()
    {
        var request = new LoanComparisonRequest
        {
            LoanAmount = 100000,
            Loans = [
                new LoanComparisonRequestLoan
                {
                    InterestRate = 5,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 2000,
                    HomeValue = 120000,
                    Pmi = 0
                },
                new LoanComparisonRequestLoan
                {
                    InterestRate = 5,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 2000,
                    HomeValue = 120000,
                    Pmi = 0
                }
            ]
        };
        var calculator = new LoanComparisonCalculator();

        var response = calculator.Calculate(request);

        Assert.Equal(0, response.TotalSavings);
    }
    
    [Fact]
    public void Validate_ValidLoanComparisonRequestLoan()
    {
        var loan = new LoanComparisonRequestLoan
        {
            InterestRate = 5.0m,
            Term = 30,
            Points = 1.0m,
            OriginationFees = 1.0m,
            ClosingCosts = 3000.0m,
            HomeValue = 250000.0m,
            Pmi = 0.5m
        };
        var validator = new LoanComparisonRequestLoanValidator();

        var result = validator.TestValidate(loan);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidLoanComparisonRequestLoan()
    {
        var loan = new LoanComparisonRequestLoan
        {
            InterestRate = 0.0m, // Invalid
            Term = 0, // Invalid
            Points = 1.0m,
            OriginationFees = -1.0m, // Invalid
            ClosingCosts = -100.0m, // Invalid
            HomeValue = 250000.0m,
            Pmi = 11.0m // Invalid
        };
        var validator = new LoanComparisonRequestLoanValidator();

        var result = validator.TestValidate(loan);
        result.ShouldHaveValidationErrorFor(l => l.InterestRate);
        result.ShouldHaveValidationErrorFor(l => l.Term);
        result.ShouldHaveValidationErrorFor(l => l.OriginationFees);
        result.ShouldHaveValidationErrorFor(l => l.ClosingCosts);
        result.ShouldHaveValidationErrorFor(l => l.Pmi);
    }
    
    [Fact]
    public void Validate_ValidLoanComparisonRequest_WithTwoValidLoans()
    {
        var request = new LoanComparisonRequest
        {
            LoanAmount = 150000,
            Loans = [
                new LoanComparisonRequestLoan
                {
                    InterestRate = 4.5m,
                    Term = 30,
                    Points = 1.0m,
                    OriginationFees = 1.0m,
                    ClosingCosts = 2000.0m,
                    HomeValue = 200000.0m,
                    Pmi = 0.5m
                },
                new LoanComparisonRequestLoan
                {
                    InterestRate = 5.0m,
                    Term = 15,
                    Points = 0.5m,
                    OriginationFees = 0.5m,
                    ClosingCosts = 1500.0m,
                    HomeValue = 200000.0m,
                    Pmi = 0.0m
                }
            ]
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidLoanComparisonRequest_WithSingleValidLoan()
    {
        var request = new LoanComparisonRequest
        {
            LoanAmount = 100000,
            Loans = [
                new LoanComparisonRequestLoan
                {
                    InterestRate = 3.75m,
                    Term = 20,
                    Points = 0.0m,
                    OriginationFees = 0.0m,
                    ClosingCosts = 1000.0m,
                    HomeValue = 120000.0m,
                    Pmi = 0.0m
                }
            ]
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(l => l.Loans);
    }
}