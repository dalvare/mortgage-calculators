namespace MortgageCalculators.Models;

/// <summary>
/// Request model for computing a monthly mortgage payment breakdown.
/// </summary>
public class MonthlyPaymentCalculatorRequest
{
    /// <summary>
    /// Loan principal amount.
    /// </summary>
    public decimal LoanAmount { get; set; }
    /// <summary>
    /// Home value used to evaluate LTV and PMI.
    /// </summary>
    public decimal HomeValue { get; set; }
    /// <summary>
    /// Annual PMI rate as a percentage.
    /// </summary>
    public decimal Pmi { get; set; }
    /// <summary>
    /// Annual property taxes in dollars.
    /// </summary>
    public decimal AnnualTaxes { get; set; }
    /// <summary>
    /// Annual homeowners insurance in dollars.
    /// </summary>
    public decimal AnnualInsurance { get; set; }
    /// <summary>
    /// Annual interest rate as a percentage (e.g., 6 for 6%).
    /// </summary>
    public decimal InterestRate { get; set; }
    /// <summary>
    /// Loan term in years.
    /// </summary>
    public int Term { get; set; }
}
