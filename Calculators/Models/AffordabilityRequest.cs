namespace Calculators.Models;

public class AffordabilityRequest
{
    public decimal TotalMonthlyIncome { get; set; } // dollars
    public decimal TotalMonthlyExpenses { get; set; } // dollars
    public decimal DownPayment { get; set; } // dollars
    public decimal InterestRate { get; set; } // percentage e.g. 3.5%
    public int Term { get; set; } // years
    public decimal Pmi { get; set; } // percentage e.g. 0.5%
    public decimal FrontRatio { get; set; } // percentage e.g. 28%
    public decimal BackRatio { get; set; } // percentage e.g. 36%
    public decimal AnnualTaxes { get; set; } // dollars
    public decimal AnnualInsurance { get; set; } // dollars
}
