namespace MortgageCalculators.Models;

public class RefinanceResponse
{
    public required RefinanceLoanResponse CurrentLoan { get; set; }
    public required RefinanceLoanResponse RefinanceLoan { get; set; }
    public required decimal MonthlyPaymentSavings { get; set; }
    public required decimal TaxSavingsLosses { get; set; }
    public required decimal BalanceLosses { get; set; }
    public required decimal TotalLosses { get; set; }
    public required decimal TotalClosingCosts { get; set; }
    public required decimal TotalBenefit { get; set; }
}

public class RefinanceLoanResponse {
    public decimal LoanAmount { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalMonthlyPayments { get; set; }
    public decimal BalanceAtSale { get; set; }
    public decimal InterestPaid { get; set; }
    public decimal TaxSavings { get; set; }
    public decimal Points { get; set; }
    public required Amortization Amortization { get; set; }
}

public class CurrentRefinanceLoanResponse : RefinanceLoanResponse
{
    public decimal RemainingBalance { get; set; }
}


