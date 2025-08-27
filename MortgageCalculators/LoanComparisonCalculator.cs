using MortgageCalculators.Extensions;
using MortgageCalculators.Models;
using MortgageCalculators.Interfaces;

namespace MortgageCalculators;

/// <summary>
/// Compares multiple loan scenarios, calculating points, fees, and amortization to identify savings.
/// </summary>
public class LoanComparisonCalculator : MortgageCalculator, IMortgageCalculator<LoanComparisonCalculatorRequest, LoanComparisonCalculatorResponse>
{
	/// <summary>
	/// Calculates comparison metrics for each loan scenario and summarizes savings across options.
	/// </summary>
	/// <param name="calculatorRequest">The loan comparison request containing loan options and base inputs.</param>
	/// <returns>A response containing per-loan metrics and overall savings.</returns>
	public LoanComparisonCalculatorResponse Calculate(LoanComparisonCalculatorRequest calculatorRequest)
	{
		var loans = new List<LoanComparisonCalculatorLoanResponse>();
		
		foreach (var loanComparisonRequestLoan in calculatorRequest.Loans)
		{
			var points = CalculatePoints(calculatorRequest.LoanAmount, loanComparisonRequestLoan.Points);
			var originationFees = CalculateOriginationFees(calculatorRequest.LoanAmount, loanComparisonRequestLoan.OriginationFees);
			var totalClosingCosts = loanComparisonRequestLoan.ClosingCosts + points + originationFees;
			var monthlyPrincipalAndInterest = CalculatePayment(calculatorRequest.LoanAmount, loanComparisonRequestLoan.InterestRate, loanComparisonRequestLoan.Term);

			var amortization = CalculateAmortization(calculatorRequest.LoanAmount, loanComparisonRequestLoan.InterestRate, loanComparisonRequestLoan.Term * 12, DateTime.Now, loanComparisonRequestLoan.HomeValue, loanComparisonRequestLoan.Pmi);

			var loan = new LoanComparisonCalculatorLoanResponse
			{
				Points = points.ToDollar(),
				OriginationFees = originationFees.ToDollar(),
				ClosingCosts = loanComparisonRequestLoan.ClosingCosts.ToDollar(),
				TotalClosingCosts = totalClosingCosts.ToDollar(),
				MonthlyPrincipalAndInterest = monthlyPrincipalAndInterest.ToDollar(),
				Amortization = amortization
			};

			loans.Add(loan);
		}
		
		return new LoanComparisonCalculatorResponse
		{
			LoanAmount = calculatorRequest.LoanAmount.ToDollar(),
			Loans = loans
		};
	}
}