namespace MortgageCalculators.Models;

/// <summary>
/// Base request details shared by refinance loan sub-models.
/// </summary>
public abstract class RefinanceLoanDetailsRequest
{
    /// <summary>
    /// Annual interest rate as a percentage.
    /// </summary>
    public decimal InterestRate { get; set; }
    /// <summary>
    /// Loan term in years.
    /// </summary>
    public int Term { get; set; }
    /// <summary>
    /// Annual PMI rate as a percentage.
    /// </summary>
    public decimal Pmi { get; set; }
}

/// <summary>
/// Details of the current (original) loan before refinancing.
/// </summary>
public class RefinanceCurrentLoanRequest : RefinanceLoanDetailsRequest
{
    /// <summary>
    /// Original loan amount in dollars.
    /// </summary>
    public decimal OriginalLoanAmount { get; set; }
    /// <summary>
    /// Number of months already paid on the current loan.
    /// </summary>
    public int MonthsPaid { get; set; }
}

/// <summary>
/// Details of the proposed refinance loan and associated pricing.
/// </summary>
public class RefinanceRefinanceLoanRequest : RefinanceLoanDetailsRequest
{
    /// <summary>
    /// Discount points as a percentage of the refinanced amount.
    /// </summary>
    public decimal Points { get; set; }
    /// <summary>
    /// Origination fees as a percentage of the refinanced amount.
    /// </summary>
    public decimal OriginationFees { get; set; }
    /// <summary>
    /// Flat closing costs in dollars.
    /// </summary>
    public decimal ClosingCosts { get; set; }
    /// <summary>
    /// Planned holding period before property sale (years).
    /// </summary>
    public int YearsBeforeSale { get; set; }
}

/// <summary>
/// Tax rates used to estimate tax savings from interest deductions.
/// </summary>
public class TaxRatesRequest
{
    /// <summary>
    /// State income tax rate as a percentage.
    /// </summary>
    public decimal StateTaxRate { get; set; }
    /// <summary>
    /// Marginal federal income tax rate as a percentage.
    /// </summary>
    public decimal MarginalIncomeTaxRate { get; set; }
}

/// <summary>
/// Request model for evaluating a mortgage refinance scenario.
/// </summary>
public class RefinanceCalculatorRequest
{
    /// <summary>
    /// Current home value in dollars.
    /// </summary>
    public decimal HomeValue { get; set; }
    /// <summary>
    /// Details of the current loan being refinanced.
    /// </summary>
    public required RefinanceCurrentLoanRequest CurrentLoan { get; set; }
    /// <summary>
    /// Details of the proposed refinance loan.
    /// </summary>
    public required RefinanceRefinanceLoanRequest RefinanceLoan { get; set; }
    /// <summary>
    /// Applicable tax rates for estimating tax impacts.
    /// </summary>
    public required TaxRatesRequest TaxRates { get; set; }
}