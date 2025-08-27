namespace MortgageCalculators.Models;

/// <summary>
/// Response model summarizing loan comparison results, including total savings and per-loan details.
/// </summary>
public class LoanComparisonResponse
{
    /// <summary>
    /// The compared loan amount used across all scenarios.
    /// </summary>
    public decimal LoanAmount { get; set; }
    /// <summary>
    /// Difference in total payments between the most and least expensive loan options.
    /// </summary>
    public decimal TotalSavings { get; set; }
    /// <summary>
    /// Collection of results for each compared loan scenario.
    /// </summary>
    public List<LoanComparisonResponseLoan> Loans { get; set; } = new List<LoanComparisonResponseLoan>();
}
