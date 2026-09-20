using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for affordability calculation requests.
/// </summary>
public class AffordabilityRequestValidator : AbstractValidator<AffordabilityCalculatorRequest>
{
    private readonly FieldRules _fieldRules = new();

    /// <summary>
    /// Initializes validation rules ensuring inputs are within acceptable ranges and consistent.
    /// </summary>
    public AffordabilityRequestValidator()
    {
        Include(_fieldRules);

        // The affordability solve runs the calculator's own math, which only has defined behavior for inputs the
        // field-level rules accept, so it is skipped whenever any of them fail.
        RuleFor(x => x.TotalMonthlyIncome)
            .Must((request, _) => AffordabilityCalculator.CanAffordLoan(request))
            .WithMessage(ValidationMessages.NoRoomForPrincipalAndInterest)
            .When(request => _fieldRules.Validate(request).IsValid);
    }

    /// <summary>
    /// Range and consistency rules that each apply to one field, or to a pair of fields, in isolation.
    /// </summary>
    private sealed class FieldRules : AbstractValidator<AffordabilityCalculatorRequest>
    {
        public FieldRules()
        {
            RuleFor(x => x.TotalMonthlyIncome)
                .MustBeValidMonthlyIncome();
            RuleFor(x => x.TotalMonthlyIncome)
                .GreaterThan(x => x.TotalMonthlyExpenses)
                .WithMessage(string.Format(ValidationMessages.GreaterThan, nameof(AffordabilityCalculatorRequest.TotalMonthlyExpenses)));
            RuleFor(x => x.TotalMonthlyExpenses).MustBePositive();
            RuleFor(x => x.DownPayment).MustBeValidDownPaymentPercentage();
            RuleFor(x => x.InterestRate).MustBeValidInterestRate();
            RuleFor(x => x.Term).MustBeValidLoanTerm();
            RuleFor(x => x.Pmi).MustBeValidPmi();
            RuleFor(x => x.FrontRatio).MustBeValidFrontRatio();
            RuleFor(x => x.BackRatio).MustBeValidBackRatio();
            RuleFor(x => x.AnnualTaxes).MustBeValidAnnualTaxes();
            RuleFor(x => x.AnnualInsurance).MustBeValidAnnualInsurance();
        }
    }
}
