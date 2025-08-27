using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for the loan comparison request, including required loan scenarios.
/// </summary>
public class LoanComparisonRequestValidator :  AbstractValidator<LoanComparisonRequest>
{
    /// <summary>
    /// Initializes validation rules ensuring base loan amount and two comparable loan options.
    /// </summary>
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