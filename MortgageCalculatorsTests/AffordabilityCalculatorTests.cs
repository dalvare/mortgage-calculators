using MortgageCalculators;
using MortgageCalculators.Extensions;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Validators;
using FluentValidation.TestHelper;

namespace MortgageCalculatorsTests;

public class AffordabilityCalculatorTests
{
    private readonly AffordabilityRequestValidator _validator = new();
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenValidRequestProvidedWithoutPmi()
    {
        // Arrange
        var request = new AffordabilityCalculatorRequest
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
        // The qualifying payment is $1,455 (front ratio: 8000 * 28% - 225 taxes - 200 insurance), which supports
        // a $242,681.80 loan at 6% over 30 years. The loan rounds down to $242,600; home value and down payment
        // are derived from it at the requested 20% down.
        Assert.Equal(242600m, result.LoanAmount);
        Assert.Equal(60650m, result.DownPayment);
        Assert.Equal(303250m, result.HomeValue);
        Assert.Equal(1454.51m, result.MonthlyPrincipalAndInterest);
        Assert.Equal(225m, result.MonthlyTaxes);
        Assert.Equal(200m, result.MonthlyInsurance);
        Assert.Equal(0m, result.MonthlyPmi);
        Assert.Equal(1879.51m, result.MonthlyTotal);
    }

    [Fact]
    public void Calculate_ShouldReportPaymentForTheRoundedLoan()
    {
        // Arrange
        var request = ValidRequest();
        var calculator = new AffordabilityCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        Assert.Equal(0m, result.LoanAmount % 100);
        Assert.Equal(result.Amortization.PeriodicPayment.ToDollar(), result.MonthlyPrincipalAndInterest);
        Assert.Equal(result.LoanAmount, result.Amortization.Balance);
        Assert.Equal(result.HomeValue, result.LoanAmount + result.DownPayment);
        Assert.True(result.ActualFrontRatio <= request.FrontRatio);
        Assert.True(result.ActualBackRatio <= request.BackRatio);
    }

    [Fact]
    public void Calculate_ShouldPreserveDownPaymentPercentage_WhenLoanIsRounded()
    {
        // Arrange
        var request = ValidRequest();
        request.DownPayment = 20;
        request.Pmi = 1;
        var calculator = new AffordabilityCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        // Rounding the loan must not push the loan-to-value ratio past the 80% PMI threshold.
        Assert.Equal(80m, result.LoanAmount / result.HomeValue * 100);
        Assert.Equal(0m, result.MonthlyPmi);
        Assert.All(result.Amortization.Schedule, row => Assert.Equal(0m, row.Pmi));
    }

    [Fact]
    public void Calculate_ShouldReportPmiForTheRoundedLoan_WhenPmiApplies()
    {
        // Arrange
        var request = ValidRequest();
        request.DownPayment = 10;
        request.Pmi = 1;
        var calculator = new AffordabilityCalculator();

        // Act
        var result = calculator.Calculate(request);

        // Assert
        var expectedMonthlyPmi = (result.LoanAmount * request.Pmi / 100 / 12).ToDollar();
        // The home value is the rounded loan divided by 90%, so the realized ratio is 90% to within rounding.
        Assert.Equal(90m, result.LoanAmount / result.HomeValue * 100, 4);
        Assert.Equal(expectedMonthlyPmi, result.MonthlyPmi);
        Assert.Equal(expectedMonthlyPmi, result.Amortization.Schedule[0].Pmi);
        Assert.Equal(result.MonthlyPrincipalAndInterest + result.MonthlyTaxes + result.MonthlyInsurance + result.MonthlyPmi,
            result.MonthlyTotal);
    }
    
    [Fact]
    public void Calculate_ShouldReturnCorrectResponse_WhenValidRequestProvidedWithPmi()
    {
        // Arrange
        var request = new AffordabilityCalculatorRequest
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
        var request = new AffordabilityCalculatorRequest
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
        var request = new AffordabilityCalculatorRequest
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

    [Fact]
    public void Validate_ShouldReportError_WhenTaxesAndInsuranceConsumeTheEntirePayment()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TotalMonthlyIncome)
            .WithErrorMessage(
                "Monthly income and qualifying ratios must leave room for a principal and interest payment after taxes, insurance, and PMI.");
    }

    [Fact]
    public void Validate_ShouldReportError_WhenTaxesAndInsuranceExceedThePayment()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();
        request.AnnualInsurance = 1200;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }

    [Fact]
    public void Validate_ShouldReportError_WhenPmiConsumesTheEntirePayment()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();
        request.AnnualInsurance = 200;
        request.DownPayment = 5;
        request.InterestRate = 1;
        request.Pmi = 10;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }

    [Fact]
    public void Validate_ShouldReportError_WhenAffordableLoanRoundsToZero()
    {
        // Arrange
        var request = SubHundredDollarLoanRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }

    [Theory]
    [InlineData(1_000_000)]
    [InlineData(10_000_000_000)]
    public void Validate_ShouldReportOnlyTheFieldError_WhenInterestRateIsOutOfRange(decimal interestRate)
    {
        // Arrange
        var request = ValidRequest();
        request.InterestRate = interestRate;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.InterestRate);
        result.ShouldNotHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }

    [Theory]
    [InlineData(100_000)]
    [InlineData(int.MaxValue)]
    public void Validate_ShouldReportOnlyTheFieldError_WhenTermIsOutOfRange(int term)
    {
        // Arrange
        var request = ValidRequest();
        request.Term = term;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Term);
        result.ShouldNotHaveValidationErrorFor(r => r.TotalMonthlyIncome);
    }

    [Fact]
    public void Calculate_ShouldThrow_WhenTaxesAndInsuranceConsumeTheEntirePayment()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();
        var calculator = new AffordabilityCalculator();

        // Act
        var exception = Record.Exception(() => calculator.Calculate(request));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Calculate_ShouldThrow_WhenTaxesAndInsuranceExceedThePayment()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();
        request.AnnualInsurance = 1200;
        var calculator = new AffordabilityCalculator();

        // Act
        var exception = Record.Exception(() => calculator.Calculate(request));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Calculate_ShouldThrow_WhenAffordableLoanRoundsToZero()
    {
        // Arrange
        var request = SubHundredDollarLoanRequest();
        var calculator = new AffordabilityCalculator();

        // Act
        var exception = Record.Exception(() => calculator.Calculate(request));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Calculate_ShouldThrow_WhenDownPaymentCoversTheEntireHomeValue()
    {
        // Arrange
        var request = FullyConsumedPaymentRequest();
        request.AnnualInsurance = 200;
        request.DownPayment = 100;
        var calculator = new AffordabilityCalculator();

        // Act
        var exception = Record.Exception(() => calculator.Calculate(request));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    private static AffordabilityCalculatorRequest ValidRequest() => new()
    {
        TotalMonthlyIncome = 10000,
        TotalMonthlyExpenses = 2000,
        DownPayment = 20,
        InterestRate = 5,
        Term = 30,
        Pmi = 0,
        FrontRatio = 28,
        BackRatio = 36,
        AnnualTaxes = 12000,
        AnnualInsurance = 2400
    };

    /// <summary>
    /// The front-ratio allowance is 1000 * 5% = $50/mo and monthly insurance is 600 / 12 = $50, so the
    /// affordable principal and interest payment lands on exactly zero.
    /// </summary>
    private static AffordabilityCalculatorRequest FullyConsumedPaymentRequest() => new()
    {
        TotalMonthlyIncome = 1000,
        TotalMonthlyExpenses = 0,
        DownPayment = 20,
        InterestRate = 6,
        Term = 30,
        Pmi = 0,
        FrontRatio = 5,
        BackRatio = 5,
        AnnualTaxes = 0,
        AnnualInsurance = 600
    };

    /// <summary>
    /// Monthly insurance of 594 / 12 = $49.50 leaves $0.50/mo for principal and interest, which supports a
    /// loan of about $83 at 6% over 30 years: positive, but it rounds down to zero.
    /// </summary>
    private static AffordabilityCalculatorRequest SubHundredDollarLoanRequest()
    {
        var request = FullyConsumedPaymentRequest();
        request.AnnualInsurance = 594;
        return request;
    }
}
