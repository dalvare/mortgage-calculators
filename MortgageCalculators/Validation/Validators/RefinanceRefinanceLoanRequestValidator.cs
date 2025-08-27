using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for the proposed refinance loan details and pricing inputs.
/// </summary>
public class RefinanceRefinanceLoanRequestValidator : RefinanceLoanDetailsRequestValidator<RefinanceRefinanceLoanRequest>
{
    /// <summary>
    /// Initializes validation for points, origination, closing costs, and holding period.
    /// </summary>
    public RefinanceRefinanceLoanRequestValidator()
    {
        RuleFor(x => x.Points).MustBeValidPoints();
        RuleFor(x => x.OriginationFees).MustBeValidOriginationFeesPercentage();
        RuleFor(x => x.ClosingCosts).MustBeValidClosingCosts();
        
        const int minYears = 0;
        const int maxYears = 30;
        RuleFor(x => x.YearsBeforeSale)
            .InclusiveBetween(minYears, maxYears)
            .WithMessage(string.Format(ValidationMessages.Range, minYears, maxYears));
    }
}