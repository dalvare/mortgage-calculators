namespace Calculators.Models;
    
public class AffordabilityResponse
{
    public decimal MonthlyPrincipalAndInterest { get; set; }
    public decimal MonthlyTaxes { get; set; }
    public decimal MonthlyInsurance { get; set; }
    public decimal MonthlyPmi { get; set; }
    public decimal MonthlyTotal { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal ActualFrontRatio { get; set; }
    public decimal ActualBackRatio { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal DownPayment { get; set; }
    public decimal HomeValue { get; set; }
    public Amortization Amortization { get; set; }
}