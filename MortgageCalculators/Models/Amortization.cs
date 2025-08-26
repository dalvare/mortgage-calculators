namespace MortgageCalculators.Models;

public class Amortization
{
    public decimal Balance { get; set; }
    public decimal PeriodicInterest { get; set; }
    public int Periods { get; set; }
    public decimal PeriodicPayment { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalPayment { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<AmortizationSchedule> Schedule { get; set; } = new();
    
    public int MonthsWithPmi => Schedule.Count(s => s.Pmi > 0);
}
