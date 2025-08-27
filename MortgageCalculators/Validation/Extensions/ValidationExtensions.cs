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
}