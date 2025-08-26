namespace MortgageCalculators.Models;

public abstract class RefinanceLoanDetailsRequest
{
    public decimal InterestRate { get; set; }
    public int Term { get; set; }
    public decimal Pmi { get; set; }
}

public class RefinanceCurrentLoanRequest : RefinanceLoanDetailsRequest
{
    public decimal OriginalLoanAmount { get; set; }
    public int MonthsPaid { get; set; }
}

public class RefinanceRefinanceLoanRequest : RefinanceLoanDetailsRequest
{
    public decimal Points { get; set; }
    public decimal OriginationFees { get; set; }
    public decimal ClosingCosts { get; set; }
    public int YearsBeforeSale { get; set; }
}

public class TaxRatesRequest
{
    public decimal StateTaxRate { get; set; }
    public decimal MarginalIncomeTaxRate { get; set; }
}

public class RefinanceRequest
{
    public decimal HomeValue { get; set; }
    public required RefinanceCurrentLoanRequest CurrentLoan { get; set; }
    public required RefinanceRefinanceLoanRequest RefinanceLoan { get; set; }
    public required TaxRatesRequest TaxRates { get; set; }
}