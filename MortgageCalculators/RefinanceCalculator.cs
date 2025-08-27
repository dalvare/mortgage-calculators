using MortgageCalculators.Models;
using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;

namespace MortgageCalculators;

/// <summary>
/// Evaluates whether refinancing a mortgage is beneficial by comparing current and refinance scenarios.
/// </summary>
public class RefinanceCalculator : MortgageCalculator, IMortgageCalculator<RefinanceCalculatorRequest, RefinanceCalculatorResponse>
{
	/// <summary>
	/// Calculates the financial impact of refinancing, including payments, interest, tax effects, and closing costs.
	/// </summary>
	/// <param name="calculatorRequest">Refinance inputs including current and proposed loan details and tax rates.</param>
	/// <returns>A response summarizing savings, costs, and detailed amortization schedules.</returns>
	public RefinanceCalculatorResponse Calculate(RefinanceCalculatorRequest calculatorRequest)
	{
		var totalTaxRate = calculatorRequest.TaxRates.MarginalIncomeTaxRate + calculatorRequest.TaxRates.StateTaxRate;

		var currentLoanStartDate = DateTime.Now.AddMonths(-calculatorRequest.CurrentLoan.MonthsPaid);
		var currentMonthlyPayment = CalculatePayment(calculatorRequest.CurrentLoan.OriginalLoanAmount, calculatorRequest.CurrentLoan.InterestRate, calculatorRequest.CurrentLoan.Term);
		var currentTotalMonthlyPayments = currentMonthlyPayment * calculatorRequest.RefinanceLoan.YearsBeforeSale * 12;

		var currentAmortization = CalculateAmortization(calculatorRequest.CurrentLoan.OriginalLoanAmount, calculatorRequest.CurrentLoan.InterestRate, calculatorRequest.CurrentLoan.Term * 12, currentLoanStartDate, calculatorRequest.HomeValue, calculatorRequest.CurrentLoan.Pmi);

		var currentMonthsBeforeSale = (calculatorRequest.RefinanceLoan.YearsBeforeSale * 12) + calculatorRequest.CurrentLoan.MonthsPaid;
		var remainingBalance = 0m;
		var currentInterestPaid = 0m;
		var currentBalanceAtSale = 0m;
		
		if(calculatorRequest.CurrentLoan.MonthsPaid == 0)
		{
			remainingBalance = calculatorRequest.CurrentLoan.OriginalLoanAmount;
		}
		
		for (var i = 0; i < currentAmortization.Schedule.Count; i++)
		{
			if ((i + 1) == calculatorRequest.CurrentLoan.MonthsPaid)
			{
				remainingBalance = currentAmortization.Schedule[i].Balance;
			}

			// Calculating interest paid for original and refinanced loans for period AFTER refinance and BEFORE sale
			if ((i + 1) > calculatorRequest.CurrentLoan.MonthsPaid && (i + 1) <= currentMonthsBeforeSale)
			{
				currentInterestPaid += currentAmortization.Schedule[i].Interest;
			}

			if ((i + 1) == currentMonthsBeforeSale)
			{
				currentBalanceAtSale = currentAmortization.Schedule[i].Balance;
			}
		}

		var currentTaxSavings = currentInterestPaid * (totalTaxRate / 100);
		var currentPoints = remainingBalance * (calculatorRequest.RefinanceLoan.Points / 100);
		var currentOrigination = remainingBalance * (calculatorRequest.RefinanceLoan.OriginationFees / 100);

		var refiMonthlyPayment = CalculatePayment(remainingBalance, calculatorRequest.RefinanceLoan.InterestRate, calculatorRequest.RefinanceLoan.Term);
		var refiTotalMonthlyPayments = refiMonthlyPayment * calculatorRequest.RefinanceLoan.YearsBeforeSale * 12;

		var refiAmortization = CalculateAmortization(remainingBalance, calculatorRequest.RefinanceLoan.InterestRate, calculatorRequest.RefinanceLoan.Term * 12, DateTime.Now, calculatorRequest.HomeValue, calculatorRequest.RefinanceLoan.Pmi);

		var refiMonthsBeforeSale = calculatorRequest.RefinanceLoan.YearsBeforeSale * 12;
		var refiInterestPaid = 0m;
		var refiBalanceAtSale = 0m;
		
		for (var i = 0; i < refiAmortization.Schedule.Count; i++)
		{
			// Calculating interest paid for original and refinanced loans for period AFTER refinance and BEFORE sale
			if ((i + 1) <= refiMonthsBeforeSale)
			{
				refiInterestPaid += refiAmortization.Schedule[i].Interest;
			}

			if ((i + 1) == refiMonthsBeforeSale)
			{
				refiBalanceAtSale = refiAmortization.Schedule[i].Balance;
			}
		}

		var refiTaxSavings = refiInterestPaid * (totalTaxRate / 100);

		var monthlyPaymentSavings = currentTotalMonthlyPayments - refiTotalMonthlyPayments;
		var taxSavingsLosses = currentTaxSavings - refiTaxSavings;
		var balanceLosses = refiBalanceAtSale - currentBalanceAtSale;
		var totalLosses = balanceLosses + taxSavingsLosses;
		var totalClosingCosts = currentPoints + currentOrigination + calculatorRequest.RefinanceLoan.ClosingCosts;
		var totalBenefit = monthlyPaymentSavings - totalLosses - totalClosingCosts;

		return new RefinanceCalculatorResponse
		{
			CurrentLoan = new CurrentRefinanceLoanResponse
			{
				LoanAmount = calculatorRequest.CurrentLoan.OriginalLoanAmount.ToDollar(),
				MonthlyPayment = currentMonthlyPayment.ToDollar(),
				TotalMonthlyPayments = currentTotalMonthlyPayments.ToDollar(),
				RemainingBalance = remainingBalance.ToDollar(),
				InterestPaid = currentInterestPaid.ToDollar(),
				TaxSavings = currentTaxSavings.ToDollar(),
				Points = currentPoints,
				BalanceAtSale = currentBalanceAtSale.ToDollar(),
				Amortization = currentAmortization
			},
			RefinanceLoan = new RefinanceLoanResponse
			{
				LoanAmount = remainingBalance.ToDollar(),
				MonthlyPayment = refiMonthlyPayment.ToDollar(),
				TotalMonthlyPayments = refiTotalMonthlyPayments.ToDollar(),
				InterestPaid = refiInterestPaid.ToDollar(),
				TaxSavings = refiTaxSavings.ToDollar(),
				BalanceAtSale = refiBalanceAtSale.ToDollar(),
				Amortization = refiAmortization
			},
			MonthlyPaymentSavings = monthlyPaymentSavings.ToDollar(),
			TaxSavingsLosses = taxSavingsLosses.ToDollar(),
			BalanceLosses = balanceLosses.ToDollar(),
			TotalLosses = totalLosses.ToDollar(),
			TotalClosingCosts = totalClosingCosts.ToDollar(),
			TotalBenefit = totalBenefit.ToDollar()
		};
	}
}