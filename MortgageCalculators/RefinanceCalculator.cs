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
	/// <remarks>
	/// The analysis period runs from the refinance date to the sale, <c>YearsBeforeSale</c> years later. Payments,
	/// interest, and balances for each loan are taken over that window only, and stop once a loan is paid off.
	/// A zero-year holding period therefore yields no savings or losses and a benefit equal to the negated closing costs.
	/// </remarks>
	/// <param name="calculatorRequest">Refinance inputs including current and proposed loan details and tax rates.</param>
	/// <returns>A response summarizing savings, costs, and detailed amortization schedules.</returns>
	public RefinanceCalculatorResponse Calculate(RefinanceCalculatorRequest calculatorRequest)
	{
		var current = calculatorRequest.CurrentLoan;
		var refinance = calculatorRequest.RefinanceLoan;
		var totalTaxRate = calculatorRequest.TaxRates.MarginalIncomeTaxRate + calculatorRequest.TaxRates.StateTaxRate;
		var monthsBeforeSale = refinance.YearsBeforeSale * 12;

		// Current loan: the analysis window starts after the payments already made.
		var currentLoanStartDate = DateTime.Now.AddMonths(-current.MonthsPaid);
		var currentMonthlyPayment = CalculatePayment(current.OriginalLoanAmount, current.InterestRate, current.Term);
		var currentAmortization = CalculateAmortization(current.OriginalLoanAmount, current.InterestRate, current.Term * 12, currentLoanStartDate, calculatorRequest.HomeValue, current.Pmi);

		var remainingBalance = BalanceAfterPayments(currentAmortization, current.MonthsPaid);
		var currentPaymentsBeforeSale = PaymentsInWindow(currentAmortization, current.MonthsPaid, monthsBeforeSale);
		var currentTotalMonthlyPayments = currentMonthlyPayment * currentPaymentsBeforeSale;
		var currentInterestPaid = InterestPaidInWindow(currentAmortization, current.MonthsPaid, monthsBeforeSale);
		var currentBalanceAtSale = BalanceAfterPayments(currentAmortization, current.MonthsPaid + monthsBeforeSale);
		var currentTaxSavings = currentInterestPaid * (totalTaxRate / 100);

		// Refinance loan: replaces the remaining balance, and the analysis window starts at its first payment.
		var points = CalculatePoints(remainingBalance, refinance.Points);
		var originationFees = CalculateOriginationFees(remainingBalance, refinance.OriginationFees);

		var refiMonthlyPayment = CalculatePayment(remainingBalance, refinance.InterestRate, refinance.Term);
		var refiAmortization = CalculateAmortization(remainingBalance, refinance.InterestRate, refinance.Term * 12, DateTime.Now, calculatorRequest.HomeValue, refinance.Pmi);

		var refiPaymentsBeforeSale = PaymentsInWindow(refiAmortization, 0, monthsBeforeSale);
		var refiTotalMonthlyPayments = refiMonthlyPayment * refiPaymentsBeforeSale;
		var refiInterestPaid = InterestPaidInWindow(refiAmortization, 0, monthsBeforeSale);
		var refiBalanceAtSale = BalanceAfterPayments(refiAmortization, monthsBeforeSale);
		var refiTaxSavings = refiInterestPaid * (totalTaxRate / 100);

		var monthlyPaymentSavings = currentTotalMonthlyPayments - refiTotalMonthlyPayments;
		var taxSavingsLosses = currentTaxSavings - refiTaxSavings;
		var balanceLosses = refiBalanceAtSale - currentBalanceAtSale;
		var totalLosses = balanceLosses + taxSavingsLosses;
		var totalClosingCosts = points + originationFees + refinance.ClosingCosts;
		var totalBenefit = monthlyPaymentSavings - totalLosses - totalClosingCosts;

		return new RefinanceCalculatorResponse
		{
			CurrentLoan = new CurrentRefinanceLoanResponse
			{
				LoanAmount = current.OriginalLoanAmount.ToDollar(),
				MonthlyPayment = currentMonthlyPayment.ToDollar(),
				TotalMonthlyPayments = currentTotalMonthlyPayments.ToDollar(),
				RemainingBalance = remainingBalance.ToDollar(),
				InterestPaid = currentInterestPaid.ToDollar(),
				TaxSavings = currentTaxSavings.ToDollar(),
				BalanceAtSale = currentBalanceAtSale.ToDollar(),
				Amortization = currentAmortization
			},
			RefinanceLoan = new RefinanceRefinanceLoanResponse
			{
				LoanAmount = remainingBalance.ToDollar(),
				MonthlyPayment = refiMonthlyPayment.ToDollar(),
				TotalMonthlyPayments = refiTotalMonthlyPayments.ToDollar(),
				InterestPaid = refiInterestPaid.ToDollar(),
				TaxSavings = refiTaxSavings.ToDollar(),
				BalanceAtSale = refiBalanceAtSale.ToDollar(),
				Points = points.ToDollar(),
				OriginationFees = originationFees.ToDollar(),
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

	/// <summary>
	/// Returns the balance remaining after the given number of payments: the full principal before any payment
	/// is made, and zero once the schedule is exhausted.
	/// </summary>
	private static decimal BalanceAfterPayments(Amortization amortization, int paymentsMade)
	{
		if (paymentsMade <= 0)
			return amortization.Balance;
		if (paymentsMade >= amortization.Schedule.Count)
			return 0;

		return amortization.Schedule[paymentsMade - 1].Balance;
	}

	/// <summary>
	/// Returns the number of payments that fall inside an analysis window, which is cut short when the loan is
	/// paid off before the window ends.
	/// </summary>
	private static int PaymentsInWindow(Amortization amortization, int paymentsAlreadyMade, int windowLength)
	{
		var paymentsRemaining = Math.Max(amortization.Schedule.Count - paymentsAlreadyMade, 0);
		return Math.Min(windowLength, paymentsRemaining);
	}

	/// <summary>
	/// Sums the interest charged by the payments that fall inside an analysis window.
	/// </summary>
	private static decimal InterestPaidInWindow(Amortization amortization, int paymentsAlreadyMade, int windowLength)
	{
		return amortization.Schedule.Skip(paymentsAlreadyMade).Take(windowLength).Sum(row => row.Interest);
	}
}
