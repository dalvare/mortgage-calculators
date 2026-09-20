using FluentValidation;
using FluentValidation.TestHelper;
using MortgageCalculators.Validation.Extensions;

namespace MortgageCalculatorsTests;

public class ValidationExtensionsTests
{
    private sealed record Amount(decimal Value);

    private sealed class NonNegativeAmountValidator : AbstractValidator<Amount>
    {
        public NonNegativeAmountValidator() => RuleFor(x => x.Value).MustBePositive();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1000)]
    public void MustBePositive_ShouldAcceptZeroAndAbove(decimal value)
    {
        var result = new NonNegativeAmountValidator().TestValidate(new Amount(value));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void MustBePositive_ShouldRejectNegativeValues_WithAMessageThatMatchesTheRule()
    {
        var result = new NonNegativeAmountValidator().TestValidate(new Amount(-0.01m));

        result.ShouldHaveValidationErrorFor(x => x.Value)
            .WithErrorMessage("The value must be zero or greater.");
    }
}
