namespace MortgageCalculators.Models;

/// <summary>
/// Request model for comparing multiple loan scenarios against a single loan amount.
/// </summary>
public class LoanComparisonRequest
{
    /// <summary>
    /// The base loan amount used across all compared loan scenarios.
    /// </summary>
    public decimal LoanAmount { get; set; }
    /// <summary>
    /// The list of loan options to compare, each with its own rate, term, and fees.
    /// </summary>
    public List<LoanComparisonRequestLoan> Loans { get; set; } = [];
}
