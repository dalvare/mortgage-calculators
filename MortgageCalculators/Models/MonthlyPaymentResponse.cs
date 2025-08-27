namespace MortgageCalculators.Models;

/// <summary>
/// Response model containing monthly payment components and generated amortization.
/// </summary>
public class MonthlyPaymentResponse
{
    /// <summary>
    /// Monthly principal and interest payment amount.
    /// </summary>
    public decimal MonthlyPrincipalAndInterest { get; set; }
    /// <summary>
    /// Monthly property taxes amount.
    /// </summary>
    public decimal MonthlyTaxes { get; set; }
    /// <summary>
    /// Monthly homeowners insurance amount.
    /// </summary>
    public decimal MonthlyInsurance { get; set; }
    /// <summary>
    /// Loan-to-value percentage used for PMI determination.
    /// </summary>
    public decimal LoanToValue { get; set; }
    /// <summary>
    /// Monthly private mortgage insurance amount.
    /// </summary>
    public decimal MonthlyPmi { get; set; }
    /// <summary>
    /// Total monthly payment (P&amp;I + taxes + insurance + PMI).
    /// </summary>
    public decimal MonthlyPayment { get; set; }
    /// <summary>
    /// Full amortization schedule and totals.
    /// </summary>
    public Amortization Amortization { get; set; }
}
