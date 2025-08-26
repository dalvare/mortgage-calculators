using FluentValidation;

namespace MortgageCalculators.Validation.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal> MustBeValidPercent<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const int minPercent = 1;
        const int maxPercent = 100;
        return ruleBuilder
            .InclusiveBetween(minPercent, maxPercent)
            .WithMessage(string.Format(ValidationMessages.Range, minPercent, maxPercent));
    }
    
    public static IRuleBuilderOptions<T, int> MustBeValidLoanTerm<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        const int minTerm = 1;
        const int maxTerm = 40;
        return ruleBuilder
            .InclusiveBetween(minTerm, maxTerm)
            .WithMessage(string.Format(ValidationMessages.Range, minTerm, maxTerm));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBePositive<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage(ValidationMessages.PositiveValue);
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidPmi<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPmi = 0;
        const decimal maxPmi = 10;
        return ruleBuilder
            .InclusiveBetween(minPmi, maxPmi)
            .WithMessage(string.Format(ValidationMessages.Range, minPmi, maxPmi));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidInterestRate<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minRate = 1;
        const decimal maxRate = 25;
        return ruleBuilder
            .InclusiveBetween(minRate, maxRate)
            .WithMessage(string.Format(ValidationMessages.Range, minRate, maxRate));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidLoanAmount<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minValue = 30000;
        return ruleBuilder
            .GreaterThanOrEqualTo(minValue)
            .WithMessage(string.Format(ValidationMessages.AtLeast, minValue));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidPoints<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPoints = 0;
        const decimal maxPoints = 3;
        return ruleBuilder
            .InclusiveBetween(minPoints, maxPoints)
            .WithMessage(string.Format(ValidationMessages.Range, minPoints, maxPoints));
    }
}