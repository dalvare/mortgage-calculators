using FluentValidation;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;

namespace MortgageCalculators.Validation.Validators;

public abstract class RefinanceLoanDetailsRequestValidator<T> : AbstractValidator<T> where T : RefinanceLoanDetailsRequest
{
    protected RefinanceLoanDetailsRequestValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}