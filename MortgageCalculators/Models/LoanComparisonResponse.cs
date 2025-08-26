namespace MortgageCalculators.Models;

public class LoanComparisonResponse
{
    public decimal LoanAmount { get; set; }
    public decimal TotalSavings { get; set; }
    public List<LoanComparisonResponseLoan> Loans { get; set; } = new List<LoanComparisonResponseLoan>();
}
