namespace MortgageCalculators.Models;

public class LoanComparisonResponseLoan
{
    public decimal Points { get; set; }
    public decimal OriginationFees { get; set; }
    public decimal ClosingCosts { get; set; }
    public decimal TotalClosingCosts { get; set; }
    public decimal MonthlyPrincipalAndInterest { get; set; }
    public Amortization Amortization { get; set; }
}
