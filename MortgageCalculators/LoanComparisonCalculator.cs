using MortgageCalculators.Extensions;
using MortgageCalculators.Models;
using MortgageCalculators.Interfaces;

namespace MortgageCalculators;

/// <summary>
/// Compares multiple loan scenarios, calculating points, fees, and amortization to identify savings.
/// </summary>
public class LoanComparisonCalculator : MortgageCalculator, IMortgageCalculator<LoanComparisonRequest, LoanComparisonResponse>
{
	/// <summary>
	/// Calculates comparison metrics for each loan scenario and summarizes savings across options.
	/// </summary>
	/// <param name="request">The loan comparison request containing loan options and base inputs.</param>
	/// <returns>A response containing per-loan metrics and overall savings.</returns>
	public LoanComparisonResponse Calculate(LoanComparisonRequest request)
	{
		var loans = new List<LoanComparisonResponseLoan>();
		
		foreach (var loanComparisonRequestLoan in request.Loans)
		{
			var points = CalculatePoints(request.LoanAmount, loanComparisonRequestLoan.Points);
			var originationFees = CalculateOriginationFees(request.LoanAmount, loanComparisonRequestLoan.OriginationFees);
			var totalClosingCosts = loanComparisonRequestLoan.ClosingCosts + points + originationFees;
			var monthlyPrincipalAndInterest = CalculatePayment(request.LoanAmount, loanComparisonRequestLoan.InterestRate, loanComparisonRequestLoan.Term);

			var amortization = CalculateAmortization(request.LoanAmount, loanComparisonRequestLoan.InterestRate, loanComparisonRequestLoan.Term * 12, DateTime.Now, loanComparisonRequestLoan.HomeValue, loanComparisonRequestLoan.Pmi);

			var loan = new LoanComparisonResponseLoan
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

		var (leastExpensiveLoan, mostExpensiveLoan) = loans.Aggregate(
			(Min: loans[0], Max: loans[0]),
			(acc, loan) => (
				loan.Amortization.TotalPayment < acc.Min.Amortization.TotalPayment ? loan : acc.Min,
				loan.Amortization.TotalPayment > acc.Max.Amortization.TotalPayment ? loan : acc.Max
			)
		);
		
		var totalSavings = mostExpensiveLoan.Amortization.TotalPayment - leastExpensiveLoan.Amortization.TotalPayment;

		return new LoanComparisonResponse
		{
			LoanAmount = request.LoanAmount.ToDollar(),
			TotalSavings = totalSavings.ToDollar(),
			Loans = loans
		};
	}
}