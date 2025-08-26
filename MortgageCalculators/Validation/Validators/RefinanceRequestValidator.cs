using MortgageCalculators.Models;
using MortgageCalculators.Validation.Extensions;
using FluentValidation;

namespace MortgageCalculators.Validation.Validators;

public class RefinanceRequestValidator : AbstractValidator<RefinanceRequest>
{
    public RefinanceRequestValidator()
    {
        RuleFor(x => x.HomeValue).MustBePositive();
        RuleFor(x => x.CurrentLoan).SetValidator(new RefinanceCurrentLoanRequestValidator());
        RuleFor(x => x.RefinanceLoan).SetValidator(new RefinanceRefinanceLoanRequestValidator());
        RuleFor(x => x.TaxRates).SetValidator(new TaxRatesRequestValidator());
    }
}