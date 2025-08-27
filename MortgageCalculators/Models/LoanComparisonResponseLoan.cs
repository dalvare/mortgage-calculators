namespace MortgageCalculators.Models;

/// <summary>
/// Per-loan comparison result including pricing components and amortization.
/// </summary>
public class LoanComparisonResponseLoan
{
    /// <summary>
    /// Dollar amount of discount points paid.
    /// </summary>
    public decimal Points { get; set; }
    /// <summary>
    /// Dollar amount of origination fees paid.
    /// </summary>
    public decimal OriginationFees { get; set; }
    /// <summary>
    /// Flat closing costs in dollars.
    /// </summary>
    public decimal ClosingCosts { get; set; }
    /// <summary>
    /// Total of all closing costs including points and origination fees.
    /// </summary>
    public decimal TotalClosingCosts { get; set; }
    /// <summary>
    /// Monthly principal and interest payment amount.
    /// </summary>
    public decimal MonthlyPrincipalAndInterest { get; set; }
    /// <summary>
    /// Full amortization schedule and totals for this loan scenario.
    /// </summary>
    public required Amortization Amortization { get; set; }
}
