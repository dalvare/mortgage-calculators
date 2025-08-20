using FluentValidation;
using Calculators.Models;
using Calculators.Validation.Extensions;

namespace Calculators.Validation.Validators;

public abstract class RefinanceLoanDetailsRequestValidator<T> : AbstractValidator<T> where T : RefinanceLoanDetailsRequest
{
    protected RefinanceLoanDetailsRequestValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}