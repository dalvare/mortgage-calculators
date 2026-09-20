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
        var request = new LoanComparisonCalculatorRequest
        {
            LoanAmount = 200000,
            Loans = [
                new LoanComparisonCalculatorLoanRequest
                {
                    InterestRate = 5,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 3000,
                    HomeValue = 250000,
                    Pmi = 0
                },
                new LoanComparisonCalculatorLoanRequest()
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
        Assert.Equal(2, response.Loans.Count);
    }

    [Fact]
    public void Calculate_ReturnsZeroTotalSavings_WhenLoansHaveIdenticalTotalPayments()
    {
        var request = new LoanComparisonCalculatorRequest
        {
            LoanAmount = 100000,
            Loans = [
                new LoanComparisonCalculatorLoanRequest
                {
                    InterestRate = 5,
                    Term = 30,
                    Points = 1,
                    OriginationFees = 1,
                    ClosingCosts = 2000,
                    HomeValue = 120000,
                    Pmi = 0
                },
                new LoanComparisonCalculatorLoanRequest
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

        Assert.Equal(response.Loans[0].Amortization.TotalPayment, response.Loans[1].Amortization.TotalPayment);
    }
    
    [Fact]
    public void Calculate_WhenLtvIsGreaterThan100()
    {
        var request = new LoanComparisonCalculatorRequest
        {
            LoanAmount = 400000,
            Loans = [
                new LoanComparisonCalculatorLoanRequest
                {
                    ClosingCosts = 500,
                    HomeValue = 380000,
                    InterestRate = 5.78m,
                    OriginationFees = 0,
                    Pmi = 0,
                    Points = 0,
                    Term = 30
                },
                new LoanComparisonCalculatorLoanRequest
                {
                    ClosingCosts = 500,
                    HomeValue = 400000,
                    InterestRate = 5.49m,
                    OriginationFees = 0,
                    Pmi = 0,
                    Points = 0,
                    Term = 30
                }
            ]
        };

        var calculator = new LoanComparisonCalculator();
        var response = calculator.Calculate(request);
        Assert.Equal(2, response.Loans.Count);
        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Calculate_ShouldSucceed_AtTheHighestLoanToValueTheValidatorAccepts()
    {
        // Arrange: a home value one cent above half the loan amount is the most underwater scenario allowed.
        var request = ValidRequest();
        request.LoanAmount = 250000m;
        request.Loans[0].HomeValue = 125000.01m;
        request.Loans[1].HomeValue = 125000.01m;
        var calculator = new LoanComparisonCalculator();

        // Act
        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        var response = calculator.Calculate(request);

        // Assert
        Assert.Equal(2, response.Loans.Count);
    }

    [Fact]
    public void Validate_ShouldReportError_WhenHomeValueCannotCarryTheLoanAmount()
    {
        // Arrange: a $250k loan against a $120k home is a 208% LTV, which the calculator would reject.
        var request = ValidRequest();
        request.LoanAmount = 250000m;
        request.Loans[0].HomeValue = 120000m;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Loans[0].HomeValue")
            .WithErrorMessage("Home value must be greater than half of the loan amount.");
        result.ShouldNotHaveValidationErrorFor("Loans[1].HomeValue");
    }

    [Fact]
    public void Validate_ShouldReportError_WhenHomeValueIsExactlyHalfTheLoanAmount()
    {
        // Arrange
        var request = ValidRequest();
        request.LoanAmount = 250000m;
        request.Loans[0].HomeValue = 125000m;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Loans[0].HomeValue");
    }

    [Fact]
    public void Validate_ShouldReportError_WhenLoansIsNull()
    {
        // Arrange
        var request = new LoanComparisonCalculatorRequest { LoanAmount = 300000m, Loans = null! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Loans);
    }

    [Fact]
    public void Validate_ShouldAcceptMoreThanTwoLoans()
    {
        // Arrange
        var request = ValidRequest();
        request.Loans.Add(new LoanComparisonCalculatorLoanRequest
        {
            InterestRate = 5.5m,
            Term = 20,
            Points = 0m,
            OriginationFees = 0m,
            ClosingCosts = 1000m,
            HomeValue = 250000m,
            Pmi = 0m
        });

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
        Assert.Equal(3, new LoanComparisonCalculator().Calculate(request).Loans.Count);
    }

    [Fact]
    public void Validate_ShouldReportEachInvalidLoanFieldOnce()
    {
        // Arrange
        var loan = new LoanComparisonCalculatorLoanRequest
        {
            InterestRate = 5.0m,
            Term = 30,
            Points = 1.0m,
            OriginationFees = 10.0m, // Invalid
            ClosingCosts = 10.0m, // Invalid
            HomeValue = 250000.0m,
            Pmi = 0.5m
        };
        var validator = new LoanComparisonRequestLoanValidator();

        // Act
        var result = validator.TestValidate(loan);

        // Assert
        Assert.Equal(2, result.Errors.Count);
        Assert.Single(result.Errors, e => e.PropertyName == nameof(LoanComparisonCalculatorLoanRequest.OriginationFees));
        Assert.Single(result.Errors, e => e.PropertyName == nameof(LoanComparisonCalculatorLoanRequest.ClosingCosts));
    }

    private static LoanComparisonCalculatorRequest ValidRequest() => new()
    {
        LoanAmount = 200000,
        Loans = [
            new LoanComparisonCalculatorLoanRequest
            {
                InterestRate = 5,
                Term = 30,
                Points = 1,
                OriginationFees = 1,
                ClosingCosts = 3000,
                HomeValue = 250000,
                Pmi = 0.5m
            },
            new LoanComparisonCalculatorLoanRequest
            {
                InterestRate = 4,
                Term = 30,
                Points = 1,
                OriginationFees = 1,
                ClosingCosts = 3000,
                HomeValue = 250000,
                Pmi = 0.5m
            }
        ]
    };
    
    [Fact]
    public void Validate_ValidLoanComparisonRequestLoan()
    {
        var loan = new LoanComparisonCalculatorLoanRequest
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
        var loan = new LoanComparisonCalculatorLoanRequest
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
        var request = new LoanComparisonCalculatorRequest
        {
            LoanAmount = 150000,
            Loans = [
                new LoanComparisonCalculatorLoanRequest
                {
                    InterestRate = 4.5m,
                    Term = 30,
                    Points = 1.0m,
                    OriginationFees = 1.0m,
                    ClosingCosts = 2000.0m,
                    HomeValue = 200000.0m,
                    Pmi = 0.5m
                },
                new LoanComparisonCalculatorLoanRequest
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
        var request = new LoanComparisonCalculatorRequest
        {
            LoanAmount = 100000,
            Loans = [
                new LoanComparisonCalculatorLoanRequest
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