using FluentValidation;
using Calculators.Models;
using Calculators.Validation.Extensions;

namespace Calculators.Validation.Validators;

public class TaxRatesRequestValidator : AbstractValidator<TaxRatesRequest>
{
    public TaxRatesRequestValidator()
    {
        RuleFor(x => x.State).MustBeValidPercent();
        RuleFor(x => x.Federal).MustBeValidPercent();
    }
}