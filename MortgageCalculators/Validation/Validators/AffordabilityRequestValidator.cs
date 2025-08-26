using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

public class AffordabilityRequestValidator : AbstractValidator<AffordabilityRequest>
{
    public AffordabilityRequestValidator()
    {
        RuleFor(x => x.TotalMonthlyIncome)
            .MustBePositive() ;
        RuleFor(x => x.TotalMonthlyIncome)
            .GreaterThan(x => x.TotalMonthlyExpenses)
            .WithMessage(string.Format(ValidationMessages.GreaterThan, nameof(AffordabilityRequest.TotalMonthlyExpenses)));
        RuleFor(x => x.TotalMonthlyExpenses).MustBePositive();
        RuleFor(x => x.DownPayment).MustBePositive();
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.Pmi).MustBeValidPmi();
        RuleFor(x => x.FrontRatio).MustBeValidPercent();
        RuleFor(x => x.BackRatio).MustBeValidPercent();
        RuleFor(x => x.AnnualTaxes).MustBePositive();
        RuleFor(x => x.AnnualInsurance).MustBePositive();
    }
}