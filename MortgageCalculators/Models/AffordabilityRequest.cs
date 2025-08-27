namespace MortgageCalculators.Models;

/// <summary>
/// Input model for calculating mortgage affordability based on income, expenses, ratios, and costs.
/// </summary>
public class AffordabilityRequest
{
	/// <summary>
	/// Borrower's gross total monthly income in dollars.
	/// </summary>
	public decimal TotalMonthlyIncome { get; set; } // dollars
	/// <summary>
	/// Total recurring non-housing monthly expenses in dollars.
	/// </summary>
	public decimal TotalMonthlyExpenses { get; set; } // dollars
	/// <summary>
	/// Planned down payment amount in dollars.
	/// </summary>
	public decimal DownPayment { get; set; } // dollars
	/// <summary>
	/// Annual interest rate as a percentage (e.g., 6 for 6%).
	/// </summary>
	public decimal InterestRate { get; set; } // percentage e.g. 3.5%
	/// <summary>
	/// Loan term in years.
	/// </summary>
	public int Term { get; set; } // years
	/// <summary>
	/// Annual PMI rate as a percentage (e.g., 0.5 for 0.5%).
	/// </summary>
	public decimal Pmi { get; set; } // percentage e.g. 0.5%
	/// <summary>
	/// Front-end ratio as a percentage limit for housing payment share of income.
	/// </summary>
	public decimal FrontRatio { get; set; } // percentage e.g. 28%
	/// <summary>
	/// Back-end ratio as a percentage limit for total debt payment share of income.
	/// </summary>
	public decimal BackRatio { get; set; } // percentage e.g. 36%
	/// <summary>
	/// Annual property taxes in dollars.
	/// </summary>
	public decimal AnnualTaxes { get; set; } // dollars
	/// <summary>
	/// Annual homeowners insurance in dollars.
	/// </summary>
	public decimal AnnualInsurance { get; set; } // dollars
}
