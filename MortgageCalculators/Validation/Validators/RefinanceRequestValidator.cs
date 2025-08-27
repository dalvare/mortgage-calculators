using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for the refinance request, delegating to sub-validators.
/// </summary>
public class RefinanceRequestValidator : AbstractValidator<RefinanceRequest>
{
    /// <summary>
    /// Initializes validation ensuring valid home value and nested models are validated.
    /// </summary>
    public RefinanceRequestValidator()
    {
        RuleFor(x => x.HomeValue).MustBeValidHomeValue();
        RuleFor(x => x.CurrentLoan).SetValidator(new RefinanceCurrentLoanRequestValidator());
        RuleFor(x => x.RefinanceLoan).SetValidator(new RefinanceRefinanceLoanRequestValidator());
        RuleFor(x => x.TaxRates).SetValidator(new TaxRatesRequestValidator());
    }
}