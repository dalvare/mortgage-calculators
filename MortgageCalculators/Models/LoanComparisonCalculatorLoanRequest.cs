namespace MortgageCalculators.Models;
/// <summary>
/// Describes a single loan scenario for comparison, including pricing and PMI.
/// </summary>
public class LoanComparisonCalculatorLoanRequest
{
    /// <summary>
    /// Annual interest rate as a percentage (e.g., 6 for 6%).
    /// </summary>
    public decimal InterestRate { get; set; }
    /// <summary>
    /// Loan term in years for this scenario.
    /// </summary>
    public int Term { get; set; }
    /// <summary>
    /// Discount points percentage applied to the loan amount.
    /// </summary>
    public decimal Points { get; set; }
    /// <summary>
    /// Origination fees percentage applied to the loan amount.
    /// </summary>
    public decimal OriginationFees { get; set; }
    /// <summary>
    /// Flat closing costs in dollars for this scenario.
    /// </summary>
    public decimal ClosingCosts { get; set; }
    /// <summary>
    /// Home value used for LTV and PMI evaluation.
    /// </summary>
    public decimal HomeValue { get; set; }
    /// <summary>
    /// Annual PMI rate as a percentage.
    /// </summary>
    public decimal Pmi { get; set; }
}
