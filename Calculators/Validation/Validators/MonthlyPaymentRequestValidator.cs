using Calculators.Models;
using Calculators.Validation.Extensions;
using FluentValidation;

namespace Calculators.Validation.Validators;

public class MonthlyPaymentRequestValidator : AbstractValidator<MonthlyPaymentRequest>
{
    public MonthlyPaymentRequestValidator()
    {
        RuleFor(x => x.LoanAmount).MustBeValidLoanAmount();
        RuleFor(x => x.HomeValue)
            .GreaterThan(0).WithMessage(ValidationMessages.GreaterThanZero);
        RuleFor(x => x.HomeValue)
            .GreaterThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.GreaterThan, nameof(MonthlyPaymentRequest.LoanAmount)));
        RuleFor(x => x.InterestRate).MustBeValidInterestRate();
        RuleFor(x => x.Term).MustBeValidLoanTerm();
        RuleFor(x => x.AnnualTaxes).MustBePositive();
        RuleFor(x => x.AnnualTaxes)
            .LessThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.LessThan, nameof(MonthlyPaymentRequest.LoanAmount)));
        RuleFor(x => x.AnnualInsurance).MustBePositive();
        RuleFor(x => x.AnnualInsurance)
            .LessThan(x => x.LoanAmount)
            .WithMessage(string.Format(ValidationMessages.LessThan, nameof(MonthlyPaymentRequest.LoanAmount)));
        RuleFor(x => x.Pmi).MustBeValidPmi();
    }
}