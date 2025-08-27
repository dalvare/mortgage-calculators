using FluentValidation;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Base validator for refinance loan detail models applying shared rate, term, and PMI rules.
/// </summary>
public abstract class RefinanceLoanDetailsRequestValidator<T> : AbstractValidator<T> where T : RefinanceLoanDetailsRequest
{
    /// <summary>
    /// Initializes common validation rules for refinance loan detail requests.
    /// </summary>
    protected RefinanceLoanDetailsRequestValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}