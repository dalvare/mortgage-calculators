using Calculators.Models;
using Calculators.Validation.Extensions;
using FluentValidation;

namespace Calculators.Validation.Validators;

public class RefinanceCurrentLoanRequestValidator : RefinanceLoanDetailsRequestValidator<RefinanceCurrentLoanRequest>
{
    public RefinanceCurrentLoanRequestValidator()
    {
        RuleFor(x => x.OriginalLoanAmount).MustBeValidLoanAmount();
        
        const int minMonthsPaid = 0;
        const int maxMonthsPaid = 480; // 40 years
        RuleFor(x => x.MonthsPaid)
            .InclusiveBetween(minMonthsPaid, maxMonthsPaid)
            .WithMessage(string.Format(ValidationMessages.Range, minMonthsPaid, maxMonthsPaid));
    }
}