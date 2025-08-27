namespace MortgageCalculators.Models;
    
/// <summary>
/// Output model providing an affordability summary including payments, ratios, values, and amortization.
/// </summary>
public class AffordabilityResponse
{
	/// <summary>
	/// Monthly principal and interest (P&I) payment amount.
	/// </summary>
	public decimal MonthlyPrincipalAndInterest { get; set; }
	/// <summary>
	/// Monthly property tax amount.
	/// </summary>
	public decimal MonthlyTaxes { get; set; }
	/// <summary>
	/// Monthly homeowners insurance amount.
	/// </summary>
	public decimal MonthlyInsurance { get; set; }
	/// <summary>
	/// Monthly private mortgage insurance amount.
	/// </summary>
	public decimal MonthlyPmi { get; set; }
	/// <summary>
	/// Total monthly housing payment (P&I + taxes + insurance + PMI).
	/// </summary>
	public decimal MonthlyTotal { get; set; }
	/// <summary>
	/// Gross monthly income used in the calculation.
	/// </summary>
	public decimal MonthlyIncome { get; set; }
	/// <summary>
	/// Other monthly expenses used in the back-end ratio.
	/// </summary>
	public decimal MonthlyExpenses { get; set; }
	/// <summary>
	/// Realized front-end ratio percentage.
	/// </summary>
	public decimal ActualFrontRatio { get; set; }
	/// <summary>
	/// Realized back-end ratio percentage.
	/// </summary>
	public decimal ActualBackRatio { get; set; }
	/// <summary>
	/// Calculated loan amount (home value minus down payment).
	/// </summary>
	public decimal LoanAmount { get; set; }
	/// <summary>
	/// Down payment amount used in the calculation.
	/// </summary>
	public decimal DownPayment { get; set; }
	/// <summary>
	/// Estimated affordable home value.
	/// </summary>
	public decimal HomeValue { get; set; }
	/// <summary>
	/// Complete amortization including PMI tracking and totals.
	/// </summary>
	public Amortization Amortization { get; set; }
}