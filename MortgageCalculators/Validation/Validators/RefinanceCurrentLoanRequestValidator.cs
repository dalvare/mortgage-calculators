using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

public class RefinanceCurrentLoanRequestValidator : RefinanceLoanDetailsRequestValidator<RefinanceCurrentLoanRequest>
{
    public RefinanceCurrentLoanRequestValidator()
    {
        RuleFor(x => x.OriginalLoanAmount).MustBeValidLoanAmount();
        
        const int minMonthsPaid = 0;
        const int maxMonthsPaid = 480; // 40 years
        RuleFor(x => x.MonthsPaid)
            .InclusiveBetween(minMonthsPaid, maxMonthsPaid)
            .WithMessage(string.Format(ValidationMessages.Range, minMonthsPaid, maxMonthsPaid))
            .LessThan(x => x.Term * 12);
    }
}