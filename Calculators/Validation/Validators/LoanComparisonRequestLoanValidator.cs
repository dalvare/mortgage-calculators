using Calculators.Models;
using Calculators.Validation.Extensions;
using FluentValidation;

namespace Calculators.Validation.Validators;

public class LoanComparisonRequestLoanValidator : AbstractValidator<LoanComparisonRequestLoan>
{
    public LoanComparisonRequestLoanValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.OriginationFees).MustBePositive();
        RuleFor(x => x.ClosingCosts).MustBePositive();
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}