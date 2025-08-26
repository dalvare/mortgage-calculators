using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

public class LoanComparisonRequestValidator :  AbstractValidator<LoanComparisonRequest>
{
    public LoanComparisonRequestValidator()
    {
        RuleFor(x => x.LoanAmount).MustBePositive();
        RuleForEach(x => x.Loans)
            .SetValidator(new LoanComparisonRequestLoanValidator());
        RuleFor(x => x.Loans)
            .NotNull()
            .Must(loans => loans.Count == 2)
            .WithMessage(ValidationMessages.TwoLoansRequired);
    }
}