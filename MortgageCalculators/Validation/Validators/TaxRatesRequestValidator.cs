using FluentValidation;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;

namespace MortgageCalculators.Validation.Validators;

public class TaxRatesRequestValidator : AbstractValidator<TaxRatesRequest>
{
    public TaxRatesRequestValidator()
    {
        RuleFor(x => x.StateTaxRate).MustBeValidPercent();
        RuleFor(x => x.MarginalIncomeTaxRate).MustBeValidPercent();
    }
}