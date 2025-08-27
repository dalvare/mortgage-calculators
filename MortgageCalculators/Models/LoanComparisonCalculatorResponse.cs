namespace MortgageCalculators.Models;

/// <summary>
/// Response model summarizing loan comparison results, including total savings and per-loan details.
/// </summary>
public class LoanComparisonCalculatorResponse
{
    /// <summary>
    /// The compared loan amount used across all scenarios.
    /// </summary>
    public decimal LoanAmount { get; set; }
    /// <summary>
    /// Collection of results for each compared loan scenario.
    /// </summary>
    public List<LoanComparisonCalculatorLoanResponse> Loans { get; set; } = [];
}
