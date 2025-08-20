namespace Calculators.Models;
public class AmortizationSchedule
{
    public decimal Interest { get; set; }
    public decimal Principal { get; set; }
    public decimal Balance { get; set; }
    public DateTime Date { get; set; }
    public decimal Pmi { get; set; }
}