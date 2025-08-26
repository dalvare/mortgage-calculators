namespace MortgageCalculators.Models;

public class MonthlyPaymentRequest
{
    public decimal LoanAmount { get; set; }
    public decimal HomeValue { get; set; }
    public decimal Pmi { get; set; }
    public decimal AnnualTaxes { get; set; }
    public decimal AnnualInsurance { get; set; }
    public decimal InterestRate { get; set; }
    public int Term { get; set; }
}
