using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for individual loan scenarios in a loan comparison request.
/// </summary>
public class LoanComparisonRequestLoanValidator : AbstractValidator<LoanComparisonRequestLoan>
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
        RuleFor(x => x.OriginationFees).MustBeValidOriginationFeesPercentage();
        RuleFor(x => x.ClosingCosts).MustBeValidClosingCosts();
    }
}