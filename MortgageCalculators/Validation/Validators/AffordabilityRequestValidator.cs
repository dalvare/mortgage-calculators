using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

/// <summary>
/// Validation rules for affordability calculation requests.
/// </summary>
public class AffordabilityRequestValidator : AbstractValidator<AffordabilityRequest>
{
    /// <summary>
    /// Initializes validation rules ensuring inputs are within acceptable ranges and consistent.
    /// </summary>
    public AffordabilityRequestValidator()
    {
        RuleFor(x => x.TotalMonthlyIncome)
            .MustBeValidMonthlyIncome();
        RuleFor(x => x.TotalMonthlyIncome)
            .GreaterThan(x => x.TotalMonthlyExpenses)
            .WithMessage(string.Format(ValidationMessages.GreaterThan, nameof(AffordabilityRequest.TotalMonthlyExpenses)));
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