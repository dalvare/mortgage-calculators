namespace MortgageCalculators.Models;

/// <summary>
/// Represents a loan amortization summary and detailed payment schedule.
/// </summary>
public class Amortization
{
	/// <summary>
	/// Initial loan balance used to create the schedule.
	/// </summary>
	public decimal Balance { get; set; }
	/// <summary>
	/// Periodic interest rate per month expressed as a decimal (e.g., 0.005 for 0.5%).
	/// </summary>
	public decimal PeriodicInterest { get; set; }
	/// <summary>
	/// Total number of payment periods (months) in the schedule.
	/// </summary>
	public int Periods { get; set; }
	/// <summary>
	/// Constant periodic principal and interest payment.
	/// </summary>
	public decimal PeriodicPayment { get; set; }
	/// <summary>
	/// Total interest paid over the schedule.
	/// </summary>
	public decimal TotalInterest { get; set; }
	/// <summary>
	/// Total of all payments (principal + interest) over the schedule.
	/// </summary>
	public decimal TotalPayment { get; set; }
	/// <summary>
	/// The start date of the amortization schedule.
	/// </summary>
	public DateTime StartDate { get; set; }
	/// <summary>
	/// The end date of the amortization schedule.
	/// </summary>
	public DateTime? EndDate { get; set; }
	/// <summary>
	/// The list of monthly schedule entries including balance, principal, interest, and PMI.
	/// </summary>
	public List<AmortizationSchedule> Schedule { get; set; } = new();
	
	/// <summary>
	/// Number of months in the schedule that include a positive PMI amount.
	/// </summary>
	public int MonthsWithPmi => Schedule.Count(s => s.Pmi > 0);
}
