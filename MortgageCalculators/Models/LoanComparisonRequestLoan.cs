namespace MortgageCalculators.Models;
public class LoanComparisonRequestLoan
{
    public decimal InterestRate { get; set; }
    public int Term { get; set; }
    public decimal Points { get; set; }
    public decimal OriginationFees { get; set; }
    public decimal ClosingCosts { get; set; }
    public decimal HomeValue { get; set; }
    public decimal Pmi { get; set; }
}
