using FluentValidation;

namespace MortgageCalculators.Validation.Extensions;

/// <summary>
/// Provides reusable FluentValidation extension methods for common mortgage validation rules.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Ensures a percentage value lies within [1, 100].
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBeValidPercent<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const int minPercent = 1;
        const int maxPercent = 100;
        return ruleBuilder
            .InclusiveBetween(minPercent, maxPercent)
            .WithMessage(string.Format(ValidationMessages.Range, minPercent, maxPercent));
    }
    
    /// <summary>
    /// Ensures a loan term in years lies within [1, 40].
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, int> MustBeValidLoanTerm<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        const int minTerm = 1;
        const int maxTerm = 40;
        return ruleBuilder
            .InclusiveBetween(minTerm, maxTerm)
            .WithMessage(string.Format(ValidationMessages.Range, minTerm, maxTerm));
    }
    
    /// <summary>
    /// Ensures a decimal value is non-negative.
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBePositive<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage(ValidationMessages.PositiveValue);
    }
    
    /// <summary>
    /// Ensures PMI is within an acceptable range of [0, 10].
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBeValidPmi<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPmi = 0;
        const decimal maxPmi = 10;
        return ruleBuilder
            .InclusiveBetween(minPmi, maxPmi)
            .WithMessage(string.Format(ValidationMessages.Range, minPmi, maxPmi));
    }
    
    /// <summary>
    /// Ensures the annual interest rate is within [1, 25].
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBeValidInterestRate<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minRate = 1;
        const decimal maxRate = 25;
        return ruleBuilder
            .InclusiveBetween(minRate, maxRate)
            .WithMessage(string.Format(ValidationMessages.Range, minRate, maxRate));
    }
    
    /// <summary>
    /// Ensures a loan amount meets the minimum threshold.
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBeValidLoanAmount<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minValue = 30000;
        return ruleBuilder
            .GreaterThanOrEqualTo(minValue)
            .WithMessage(string.Format(ValidationMessages.AtLeast, minValue));
    }
    
    /// <summary>
    /// Ensures discount points fall within [0, 3].
    /// </summary>
    /// <typeparam name="T">Validated model type.</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder.</param>
    /// <returns>Rule builder options for chaining.</returns>
    public static IRuleBuilderOptions<T, decimal> MustBeValidPoints<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPoints = 0;
        const decimal maxPoints = 3;
        return ruleBuilder
            .InclusiveBetween(minPoints, maxPoints)
            .WithMessage(string.Format(ValidationMessages.Range, minPoints, maxPoints));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidMonthlyIncome<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minIncome = 0;
        const decimal maxIncome = 200000;
        return ruleBuilder
            .InclusiveBetween(minIncome, maxIncome)
            .WithMessage(string.Format(ValidationMessages.Range, minIncome, maxIncome));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidDownPaymentPercentage<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPercentage = 0;
        const decimal maxPercentage = 95;
        return ruleBuilder
            .InclusiveBetween(minPercentage, maxPercentage)
            .WithMessage(string.Format(ValidationMessages.Range, minPercentage, maxPercentage));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidFrontRatio<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPercentage = 5;
        const decimal maxPercentage = 60;
        return ruleBuilder
            .InclusiveBetween(minPercentage, maxPercentage)
            .WithMessage(string.Format(ValidationMessages.Range, minPercentage, maxPercentage));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidBackRatio<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPercentage = 5;
        const decimal maxPercentage = 80;
        return ruleBuilder
            .InclusiveBetween(minPercentage, maxPercentage)
            .WithMessage(string.Format(ValidationMessages.Range, minPercentage, maxPercentage));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidAnnualInsurance<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minInsurance = 200;
        const decimal maxInsurance = 50000;
        return ruleBuilder
            .InclusiveBetween(minInsurance, maxInsurance)
            .WithMessage(string.Format(ValidationMessages.Range, minInsurance, maxInsurance));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidAnnualTaxes<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minTaxes = 0;
        const decimal maxTaxes = 200000;
        return ruleBuilder
            .InclusiveBetween(minTaxes, maxTaxes)
            .WithMessage(string.Format(ValidationMessages.Range, minTaxes, maxTaxes));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidHomeValue<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minValue = 25000;
        const decimal maxValue = 10000000;
        return ruleBuilder
            .InclusiveBetween(minValue, maxValue)
            .WithMessage(string.Format(ValidationMessages.Range, minValue, maxValue));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidOriginationFeesPercentage<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minPercentage = 0;
        const decimal maxPercentage = 5;
        return ruleBuilder
            .InclusiveBetween(minPercentage, maxPercentage)
            .WithMessage(string.Format(ValidationMessages.Range, minPercentage, maxPercentage));
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValidClosingCosts<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        const decimal minValue = 500;
        const decimal maxValue = 100000;
        return ruleBuilder
            .InclusiveBetween(minValue, maxValue)
            .WithMessage(string.Format(ValidationMessages.Range, minValue, maxValue));
    }
}