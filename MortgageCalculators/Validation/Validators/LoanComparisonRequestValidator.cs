using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for the loan comparison request, including required loan scenarios.
/// </summary>
public class LoanComparisonRequestValidator :  AbstractValidator<LoanComparisonCalculatorRequest>
{
    private const int MinimumLoans = 2;

    /// <summary>
    /// Initializes validation rules ensuring a base loan amount and at least two comparable loan options.
    /// </summary>
    public LoanComparisonRequestValidator()
    {
        RuleFor(x => x.LoanAmount).MustBeValidLoanAmount();
        RuleFor(x => x.Loans)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(loans => loans.Count >= MinimumLoans)
            .WithMessage(ValidationMessages.TwoLoansRequired);
        RuleForEach(x => x.Loans)
            .SetValidator(request => new LoanComparisonRequestLoanValidator(request.LoanAmount));
    }
}
