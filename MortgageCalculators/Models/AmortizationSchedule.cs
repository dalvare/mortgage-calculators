namespace MortgageCalculators.Models;
/// <summary>
/// A single amortization period entry describing components of the payment and resulting balance.
/// </summary>
public class AmortizationSchedule
{
	/// <summary>
	/// Interest portion of the payment for the period.
	/// </summary>
	public decimal Interest { get; set; }
	/// <summary>
	/// Principal portion of the payment for the period.
	/// </summary>
	public decimal Principal { get; set; }
	/// <summary>
	/// Remaining loan balance after the payment is applied.
	/// </summary>
	public decimal Balance { get; set; }
	/// <summary>
	/// Date corresponding to the payment period.
	/// </summary>
	public DateTime Date { get; set; }
	/// <summary>
	/// Private mortgage insurance amount for the period, if applicable.
	/// </summary>
	public decimal Pmi { get; set; }
}