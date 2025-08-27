namespace MortgageCalculators.Models;

/// <summary>
/// Response model summarizing the financial impact of refinancing.
/// </summary>
public class RefinanceCalculatorResponse
{
    /// <summary>
    /// Metrics and amortization for the current (original) loan.
    /// </summary>
    public required RefinanceLoanResponse CurrentLoan { get; set; }
    /// <summary>
    /// Metrics and amortization for the proposed refinance loan.
    /// </summary>
    public required RefinanceLoanResponse RefinanceLoan { get; set; }
    /// <summary>
    /// Savings from lower monthly payments over the analysis period.
    /// </summary>
    public required decimal MonthlyPaymentSavings { get; set; }
    /// <summary>
    /// Net change in tax savings due to different interest paid.
    /// </summary>
    public required decimal TaxSavingsLosses { get; set; }
    /// <summary>
    /// Difference in balances at time of sale (refi minus current).
    /// </summary>
    public required decimal BalanceLosses { get; set; }
    /// <summary>
    /// Total of losses (e.g., balance and tax) over the analysis period.
    /// </summary>
    public required decimal TotalLosses { get; set; }
    /// <summary>
    /// Total closing costs incurred to refinance.
    /// </summary>
    public required decimal TotalClosingCosts { get; set; }
    /// <summary>
    /// Net financial benefit of refinancing (savings minus losses and costs).
    /// </summary>
    public required decimal TotalBenefit { get; set; }
}

/// <summary>
/// Common response details for a loan, including amortization and cost totals.
/// </summary>
public class RefinanceLoanResponse {
    /// <summary>
    /// Principal loan amount.
    /// </summary>
    public decimal LoanAmount { get; set; }
    /// <summary>
    /// Monthly principal and interest payment amount.
    /// </summary>
    public decimal MonthlyPayment { get; set; }
    /// <summary>
    /// Total of all monthly payments made during the analysis period.
    /// </summary>
    public decimal TotalMonthlyPayments { get; set; }
    /// <summary>
    /// Remaining balance at the time of sale.
    /// </summary>
    public decimal BalanceAtSale { get; set; }
    /// <summary>
    /// Total interest paid during the analysis period.
    /// </summary>
    public decimal InterestPaid { get; set; }
    /// <summary>
    /// Estimated tax savings resulting from interest paid.
    /// </summary>
    public decimal TaxSavings { get; set; }
    /// <summary>
    /// Discount points paid at closing.
    /// </summary>
    public decimal Points { get; set; }
    /// <summary>
    /// Complete amortization schedule and totals for this loan.
    /// </summary>
    public required Amortization Amortization { get; set; }
}

/// <summary>
/// Specific response details for the current loan, including remaining balance.
/// </summary>
public class CurrentRefinanceLoanResponse : RefinanceLoanResponse
{
    /// <summary>
    /// Remaining balance on the current loan at evaluation time.
    /// </summary>
    public decimal RemainingBalance { get; set; }
}


