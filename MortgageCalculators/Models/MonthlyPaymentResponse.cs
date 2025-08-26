namespace MortgageCalculators.Models;

public class MonthlyPaymentResponse
{
    public decimal MonthlyPrincipalAndInterest { get; set; }
    public decimal MonthlyTaxes { get; set; }
    public decimal MonthlyInsurance { get; set; }
    public decimal LoanToValue { get; set; }
    public decimal MonthlyPmi { get; set; }
    public decimal MonthlyPayment { get; set; }
    public Amortization Amortization { get; set; }
}
