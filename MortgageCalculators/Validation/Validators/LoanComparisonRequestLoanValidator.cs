using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for individual loan scenarios in a loan comparison request.
/// </summary>
public class LoanComparisonRequestLoanValidator : AbstractValidator<LoanComparisonCalculatorLoanRequest>
{
    /// <summary>
    /// Initializes validation rules for loan pricing, LTV inputs, and PMI parameters.
    /// </summary>
    public LoanComparisonRequestLoanValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.OriginationFees).MustBeValidOriginationFeesPercentage();
        RuleFor(x => x.ClosingCosts).MustBeValidClosingCosts();
        RuleFor(x => x.Points).MustBeValidPoints();
        RuleFor(x => x.Pmi).MustBeValidPmi();
        RuleFor(x => x.HomeValue).MustBeValidHomeValue();
    }

    /// <summary>
    /// Initializes the field rules plus a check that the home value can carry the loan being compared.
    /// </summary>
    /// <param name="loanAmount">The loan amount shared by every scenario in the comparison.</param>
    /// <remarks>
    /// The shared calculator math rejects a loan-to-value ratio above 200%, so a scenario whose home value is not
    /// more than half the loan amount would fail inside the calculator rather than here.
    /// </remarks>
    public LoanComparisonRequestLoanValidator(decimal loanAmount) : this()
    {
        RuleFor(x => x.HomeValue)
            .GreaterThan(loanAmount * 0.5m)
            .WithMessage(ValidationMessages.HomeValueLessThanHalfLoanAmount);
    }
}
