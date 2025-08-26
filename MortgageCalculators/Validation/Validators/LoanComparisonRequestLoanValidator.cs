using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

public class LoanComparisonRequestLoanValidator : AbstractValidator<LoanComparisonRequestLoan>
{
    public LoanComparisonRequestLoanValidator()
    {
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.OriginationFees).MustBePositive();
        RuleFor(x => x.ClosingCosts).MustBePositive();
        RuleFor(x => x.Points).MustBePositive();
        RuleFor(x => x.Pmi).MustBeValidPmi();
        RuleFor(x => x.HomeValue).MustBePositive();
        RuleFor(x => x.OriginationFees).MustBeValidPercent();
        RuleFor(x => x.ClosingCosts).MustBePositive();
    }
}