using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;
using MortgageCalculators.Extensions;

namespace MortgageCalculators;

/// <summary>
/// Calculates a detailed monthly mortgage payment breakdown, including taxes, insurance, and PMI.
/// </summary>
public class MonthlyPaymentCalculator : MortgageCalculator, IMortgageCalculator<MonthlyPaymentCalculatorRequest, MonthlyPaymentCalculatorResponse>
{
	/// <summary>
	/// Computes monthly payment components and builds an amortization schedule with PMI tracking.
	/// </summary>
	/// <param name="calculatorRequest">Monthly payment inputs such as loan amount, rate, term, and escrow amounts.</param>
	/// <returns>A response containing monthly totals and an amortization schedule.</returns>
	public MonthlyPaymentCalculatorResponse Calculate(MonthlyPaymentCalculatorRequest calculatorRequest)
	{
		var loanToValue =  CalculateLoanToValue(calculatorRequest.LoanAmount, calculatorRequest.HomeValue);
		var pmi = DoesLoanHavePmi(loanToValue, calculatorRequest.Pmi) ? CalculatePmiAnnualAmount(calculatorRequest.LoanAmount, calculatorRequest.Pmi) : 0;
		var monthlyPmi = pmi / 12;
		var monthlyTaxes = calculatorRequest.AnnualTaxes / 12;
		var monthlyInsurance = calculatorRequest.AnnualInsurance / 12;
		var monthlyPrincipalAndInterest = CalculatePayment(calculatorRequest.LoanAmount, calculatorRequest.InterestRate, calculatorRequest.Term);
		decimal[] payments = [ monthlyPrincipalAndInterest, monthlyTaxes, monthlyInsurance, monthlyPmi ];
		var monthlyPayment = payments.Sum();
		var monthsWithPmi = 0;

		var amortization = CalculateAmortization(calculatorRequest.LoanAmount, calculatorRequest.InterestRate, calculatorRequest.Term * 12, DateTime.Now, calculatorRequest.HomeValue, calculatorRequest.Pmi);

		foreach (var (schedule, i) in amortization.Schedule.Select((s, i) => (s, i)))
		{
			var ltv = CalculateLoanToValue(schedule.Balance, calculatorRequest.HomeValue);
			if (ltv >= 80)
			{
				amortization.Schedule[i].Pmi = monthlyPmi;
				monthsWithPmi++;
			}
			else
			{
				amortization.Schedule[i].Pmi = 0;
			}
		}

		return new MonthlyPaymentCalculatorResponse
		{
			MonthlyPayment = monthlyPayment.ToDollar(),
			MonthlyPrincipalAndInterest = monthlyPrincipalAndInterest.ToDollar(),
			MonthlyTaxes = monthlyTaxes.ToDollar(),
			MonthlyInsurance = monthlyInsurance.ToDollar(),
			LoanToValue = loanToValue,
			MonthlyPmi = monthlyPmi.ToDollar(),
			Amortization = amortization
		};
	}
}