using Calculators.Models;
using Calculators.Validation.Extensions;
using FluentValidation;

namespace Calculators.Validation.Validators;

public class RefinanceRefinanceLoanRequestValidator : RefinanceLoanDetailsRequestValidator<RefinanceRefinanceLoanRequest>
{
    public RefinanceRefinanceLoanRequestValidator()
    {
        RuleFor(x => x.Points).MustBeValidPoints();
        RuleFor(x => x.OriginationFees).MustBePositive();
        RuleFor(x => x.ClosingCosts).MustBePositive();
        
        const int minYears = 0;
        const int maxYears = 30;
        RuleFor(x => x.YearsBeforeSale)
            .InclusiveBetween(minYears, maxYears)
            .WithMessage(string.Format(ValidationMessages.Range, minYears, maxYears));
    }
}