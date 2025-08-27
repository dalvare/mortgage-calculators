using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for monthly payment calculation requests.
/// </summary>
public class MonthlyPaymentRequestValidator : AbstractValidator<MonthlyPaymentCalculatorRequest>
{
    /// <summary>
    /// Initializes validation ensuring amounts, rates, term, and LTV-related constraints.
    /// </summary>
    public MonthlyPaymentRequestValidator()
    {
        RuleFor(x => x.LoanAmount).MustBeValidLoanAmount();
        RuleFor(x => x.HomeValue).MustBeValidHomeValue();
        RuleFor(x => x.HomeValue)
            .GreaterThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.GreaterThan, nameof(MonthlyPaymentCalculatorRequest.LoanAmount)));
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.AnnualTaxes).MustBeValidAnnualTaxes();
        RuleFor(x => x.AnnualTaxes)
            .LessThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.LessThan, nameof(MonthlyPaymentCalculatorRequest.LoanAmount)));
        RuleFor(x => x.AnnualInsurance).MustBeValidAnnualInsurance();
        RuleFor(x => x.AnnualInsurance)
            .LessThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.LessThan, nameof(MonthlyPaymentCalculatorRequest.LoanAmount)));
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}