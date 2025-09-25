namespace MortgageCalculators.Validation;

/// <summary>
/// Centralized validation message templates used by FluentValidation rules.
/// </summary>
internal static class ValidationMessages
{
    /// <summary>
    /// Message template for range constraints, expects min and max placeholders.
    /// </summary>
    public const string Range = "The value must be between {0} and {1}.";
    /// <summary>
    /// Message for values that must be positive.
    /// </summary>
    public const string PositiveValue = "The value must be greater than zero.";
    /// <summary>
    /// Message alias indicating the value must be strictly greater than zero.
    /// </summary>
    public const string GreaterThanZero = "The value must be greater than zero.";
    /// <summary>
    /// Message template indicating the value must be at least a minimum.
    /// </summary>
    public const string AtLeast = "The value must be at least {0}.";
    /// <summary>
    /// Message template indicating the value must be greater than a reference value.
    /// </summary>
    public const string GreaterThan = "The value must be greater than {0}.";
    /// <summary>
    /// Message template indicating the value must be less than a reference value.
    /// </summary>
    public const string LessThan = "The value must be less than {0}.";
    /// <summary>
    /// Message indicating that at least two loans are required for comparison.
    /// </summary>
    public const string TwoLoansRequired = "At least 2 loans are required for comparison.";
    /// <summary>
    /// Message indicating that the home value must be greater than or equal to the loan amount.
    /// </summary>
    public const string HomeValueGreaterThanLoanAmount = "Home value must be greater than or equal to the refinance loan amount.";
}