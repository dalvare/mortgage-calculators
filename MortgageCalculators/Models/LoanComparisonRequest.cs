namespace MortgageCalculators.Models;

public class LoanComparisonRequest
{
    public decimal LoanAmount { get; set; }
    public List<LoanComparisonRequestLoan> Loans { get; set; } = [];
}
