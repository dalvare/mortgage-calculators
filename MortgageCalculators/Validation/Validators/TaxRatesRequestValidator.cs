using FluentValidation;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for tax rate inputs used in refinance analysis.
/// </summary>
public class TaxRatesRequestValidator : AbstractValidator<TaxRatesRequest>
{
    /// <summary>
    /// Initializes validation ensuring each tax rate is a valid percentage.
    /// </summary>
    public TaxRatesRequestValidator()
    {
        RuleFor(x => x.StateTaxRate).MustBeValidPercent();
        RuleFor(x => x.MarginalIncomeTaxRate).MustBeValidPercent();
    }
}