using MortgageCalculators.Models;
using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;

namespace MortgageCalculators;

/// <summary>
/// Evaluates whether refinancing a mortgage is beneficial by comparing current and refinance scenarios.
/// </summary>
public class RefinanceCalculator : MortgageCalculator, IMortgageCalculator<RefinanceRequest, RefinanceResponse>
{
	/// <summary>
	/// Calculates the financial impact of refinancing, including payments, interest, tax effects, and closing costs.
	/// </summary>
	/// <param name="request">Refinance inputs including current and proposed loan details and tax rates.</param>
	/// <returns>A response summarizing savings, costs, and detailed amortization schedules.</returns>
	public RefinanceResponse Calculate(RefinanceRequest request)
	{
		var totalTaxRate = request.TaxRates.MarginalIncomeTaxRate + request.TaxRates.StateTaxRate;

		var currentLoanStartDate = DateTime.Now.AddMonths(-request.CurrentLoan.MonthsPaid);
		var currentMonthlyPayment = CalculatePayment(request.CurrentLoan.OriginalLoanAmount, request.CurrentLoan.InterestRate, request.CurrentLoan.Term);
		var currentTotalMonthlyPayments = currentMonthlyPayment * request.RefinanceLoan.YearsBeforeSale * 12;

		var currentAmortization = CalculateAmortization(request.CurrentLoan.OriginalLoanAmount, request.CurrentLoan.InterestRate, request.CurrentLoan.Term * 12, currentLoanStartDate, request.HomeValue, request.CurrentLoan.Pmi);

		var currentMonthsBeforeSale = (request.RefinanceLoan.YearsBeforeSale * 12) + request.CurrentLoan.MonthsPaid;
		var remainingBalance = 0m;
		var currentInterestPaid = 0m;
		var currentBalanceAtSale = 0m;
		
		if(request.CurrentLoan.MonthsPaid == 0)
		{
			remainingBalance = request.CurrentLoan.OriginalLoanAmount;
		}
		
		for (var i = 0; i < currentAmortization.Schedule.Count; i++)
		{
			if ((i + 1) == request.CurrentLoan.MonthsPaid)
			{
				remainingBalance = currentAmortization.Schedule[i].Balance;
			}

			// Calculating interest paid for original and refinanced loans for period AFTER refinance and BEFORE sale
			if ((i + 1) > request.CurrentLoan.MonthsPaid && (i + 1) <= currentMonthsBeforeSale)
			{
				currentInterestPaid += currentAmortization.Schedule[i].Interest;
			}

			if ((i + 1) == currentMonthsBeforeSale)
			{
				currentBalanceAtSale = currentAmortization.Schedule[i].Balance;
			}
		}

		var currentTaxSavings = currentInterestPaid * (totalTaxRate / 100);
		var currentPoints = remainingBalance * (request.RefinanceLoan.Points / 100);
		var currentOrigination = remainingBalance * (request.RefinanceLoan.OriginationFees / 100);

		var refiMonthlyPayment = CalculatePayment(remainingBalance, request.RefinanceLoan.InterestRate, request.RefinanceLoan.Term);
		var refiTotalMonthlyPayments = refiMonthlyPayment * request.RefinanceLoan.YearsBeforeSale * 12;

		var refiAmortization = CalculateAmortization(remainingBalance, request.RefinanceLoan.InterestRate, request.RefinanceLoan.Term * 12, DateTime.Now, request.HomeValue, request.RefinanceLoan.Pmi);

		var refiMonthsBeforeSale = request.RefinanceLoan.YearsBeforeSale * 12;
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
		var totalClosingCosts = currentPoints + currentOrigination + request.RefinanceLoan.ClosingCosts;
		var totalBenefit = monthlyPaymentSavings - totalLosses - totalClosingCosts;

		return new RefinanceResponse
		{
			CurrentLoan = new CurrentRefinanceLoanResponse
			{
				LoanAmount = request.CurrentLoan.OriginalLoanAmount.ToDollar(),
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