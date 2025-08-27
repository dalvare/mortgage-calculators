using MortgageCalculators.Extensions;
using MortgageCalculators.Models;

namespace MortgageCalculators;

/// <summary>
/// Provides common mortgage calculation utilities and shared algorithms used by concrete calculators.
/// </summary>
public abstract class MortgageCalculator
{
	private const int PmiLtvThreshold = 80;
	/// <summary>
	/// Converts an annual interest rate into an effective periodic interest rate for mortgage calculations.
	/// </summary>
	/// <param name="interestRate">Annual nominal interest rate as a percentage (e.g., 6 for 6%).</param>
	/// <param name="annualPayments">Number of payments made each year (typically 12 for monthly).</param>
	/// <param name="annualCompounds">Number of compounding periods per year (typically 12 for monthly compounding).</param>
	/// <returns>The periodic interest rate as a decimal per payment period.</returns>
	private static double CalculateMonthlyInterestRate(decimal interestRate, int annualPayments, int annualCompounds)
	{
		var adjustedInterest = interestRate / 100 / annualCompounds;
		return Math.Pow((double)(1 + adjustedInterest), (annualCompounds / annualPayments)) - 1;
	}
	
	/// <summary>
	/// Calculates the constant periodic mortgage payment amount (principal and interest) for a loan.
	/// </summary>
	/// <param name="loanAmount">The principal loan amount.</param>
	/// <param name="interest">Annual interest rate as a percentage (e.g., 6 for 6%).</param>
	/// <param name="termInYears">The loan term in years.</param>
	/// <param name="annualPayments">Number of payments per year. Defaults to 12.</param>
	/// <param name="annualCompounds">Number of compounding periods per year. Defaults to 12.</param>
	/// <returns>The periodic payment amount covering principal and interest.</returns>
	protected static decimal CalculatePayment(decimal loanAmount, decimal interest, int termInYears, int annualPayments = 12, int annualCompounds = 12)
	{
		var monthlyInterestRate = (decimal)CalculateMonthlyInterestRate(interest, annualPayments, annualCompounds);
		return loanAmount * (monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -annualPayments * termInYears)));
	}
	
	/// <summary>
	/// Calculates the loan principal that corresponds to a given periodic payment.
	/// </summary>
	/// <param name="periodPayment">The periodic payment amount (principal and interest).</param>
	/// <param name="interestRate">Annual interest rate as a percentage (e.g., 6 for 6%).</param>
	/// <param name="termInYears">The loan term in years.</param>
	/// <param name="numOfAnnualPayments">Number of payments per year. Defaults to 12.</param>
	/// <param name="annualCompounds">Number of compounding periods per year. Defaults to 12.</param>
	/// <returns>The calculated principal amount.</returns>
	/// <exception cref="ArgumentException">Thrown when any frequency parameter is not a positive value.</exception>
	protected static decimal CalculateLoanAmount(decimal periodPayment, decimal interestRate, int termInYears, int numOfAnnualPayments = 12, int annualCompounds = 12)
	{
		if (termInYears <= 0 || numOfAnnualPayments <= 0 || annualCompounds <= 0)
			throw new ArgumentException("Term, payment frequency, and compounding frequency must be positive values.");
		
		var payment = (double)periodPayment;
		var totalNumberOfPayments = termInYears * numOfAnnualPayments;
		var paymentPeriodInterestRate = CalculateMonthlyInterestRate(interestRate, numOfAnnualPayments, annualCompounds);

		var loanAmount = payment * (Math.Pow(1 + paymentPeriodInterestRate, totalNumberOfPayments) - 1) / (paymentPeriodInterestRate * Math.Pow(1 + paymentPeriodInterestRate, totalNumberOfPayments));

		return (decimal)loanAmount;
	}
	
	/// <summary>
	/// Builds a full amortization schedule for the loan including optional PMI line-items.
	/// </summary>
	/// <param name="principal">Initial loan principal.</param>
	/// <param name="rate">Annual interest rate as a percentage.</param>
	/// <param name="periods">Total number of payment periods (months).</param>
	/// <param name="startDate">Date of the first payment period.</param>
	/// <param name="homeValue">Original home value used for LTV and PMI determination.</param>
	/// <param name="annualPmi">Annual PMI rate as a percentage. Zero disables PMI.</param>
	/// <returns>An amortization object with schedule, totals, and metadata.</returns>
	protected static Amortization CalculateAmortization(decimal principal, decimal rate, int periods, DateTime startDate, decimal homeValue, decimal annualPmi = 0)
	{
		var amortization = new Amortization
		{
			Balance = principal,
			PeriodicInterest = (rate / 100) / 12,
			Periods = periods,
			PeriodicPayment = CalculatePayment(principal, rate, periods / 12),
			TotalInterest = 0,
			TotalPayment = 0,
			StartDate = startDate,
			EndDate = null,
			Schedule = []
		};

		var balance = principal;
		var hasPmi = DoesLoanHavePmi(CalculateLoanToValue(principal, homeValue), annualPmi);
		var monthlyPmi = hasPmi ? CalculatePmiAnnualAmount(principal, annualPmi) / 12 : 0;
		monthlyPmi = monthlyPmi.ToDollar();
		for (var i = 0; i < periods; i++)
		{
			var interestAmount = balance * amortization.PeriodicInterest;
			balance += interestAmount;
			var principalAmount = amortization.PeriodicPayment - interestAmount;
			balance -= amortization.PeriodicPayment;

			var date = startDate.AddMonths(i);

			amortization.TotalInterest += interestAmount;
			amortization.TotalPayment += amortization.PeriodicPayment;

			var paymentHasPmi = false;
			if (monthlyPmi > 0 && balance > 0)
			{
				var ltv = CalculateLoanToValue(balance, homeValue);
				if (DoesLoanHavePmi(ltv, annualPmi))
				{
					paymentHasPmi = true;
				}
			}
			
			var data = new AmortizationSchedule
			{
				Interest = interestAmount.ToDollar(),
				Principal = principalAmount.ToDollar(),
				Balance = balance.ToDollar(),
				Date = date,
				Pmi = paymentHasPmi ? monthlyPmi : 0
			};

			amortization.Schedule.Add(data);
		}

		amortization.EndDate = amortization.Schedule.Last().Date;
		return amortization;
	}
	
	/// <summary>
	/// Calculates the loan-to-value (LTV) ratio as a percentage.
	/// </summary>
	/// <param name="loanAmount">Current or initial loan balance.</param>
	/// <param name="homeValue">Home value used as denominator.</param>
	/// <returns>LTV as a percentage in the range [0, 100].</returns>
	protected static decimal CalculateLoanToValue(decimal loanAmount, decimal homeValue)
	{
		return (loanAmount / homeValue) * 100;
	}
	
	/// <summary>
	/// Calculates the annual PMI amount (in dollars) given a loan balance and PMI rate.
	/// </summary>
	/// <param name="loanAmount">Loan balance to use for PMI calculation.</param>
	/// <param name="annualPmi">PMI rate as a percentage (e.g., 0.5 for 0.5%).</param>
	/// <returns>Annual PMI amount in dollars. Returns 0 when PMI rate is 0.</returns>
	protected static decimal CalculatePmiAnnualAmount(decimal loanAmount, decimal annualPmi)
	{
		return annualPmi > 0 ? ((loanAmount * annualPmi) / 100) : 0;
	}
	
	/// <summary>
	/// Determines whether PMI should be applied based on LTV threshold and PMI presence.
	/// </summary>
	/// <param name="loanToValue">Current loan-to-value percentage.</param>
	/// <param name="annualPmi">Annual PMI rate percentage.</param>
	/// <returns>True if PMI applies; otherwise false.</returns>
	protected static bool DoesLoanHavePmi(decimal loanToValue, decimal annualPmi)
	{
		ValidatePercentage(loanToValue);
		return loanToValue > PmiLtvThreshold && annualPmi > 0;
	}
	
	/// <summary>
	/// Rounds a monetary value down to the nearest hundred.
	/// </summary>
	/// <param name="amount">The amount to round down.</param>
	/// <returns>The rounded value.</returns>
	protected static decimal RoundDownToNearestHundred(decimal amount)
	{
		return (amount / 100) * 100;
	}
	
	private static bool IsValidPercentage(decimal percentage)
	{
		return percentage is (>= 0 and <= 100);
	}
	
	/// <summary>
	/// Validates that a percentage is within the inclusive range [0, 100].
	/// </summary>
	/// <param name="percentage">The percentage to validate.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the percentage is outside [0,100].</exception>
	private static void ValidatePercentage(decimal percentage)
	{
		if (!IsValidPercentage(percentage))
			throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");
	}
	
	/// <summary>
	/// Calculates discount points cost based on a percentage of the loan amount.
	/// </summary>
	/// <param name="loanAmount">Loan amount the points are applied to.</param>
	/// <param name="pointsPercentage">Points percentage (e.g., 1 for 1%).</param>
	/// <returns>Dollar cost of points.</returns>
	protected static decimal CalculatePoints(decimal loanAmount, decimal pointsPercentage)
	{
		ValidatePercentage(pointsPercentage);
		return (loanAmount * pointsPercentage) / 100;
	}
	
	/// <summary>
	/// Calculates origination fees based on a percentage of the loan amount.
	/// </summary>
	/// <param name="loanAmount">Loan amount the fee is applied to.</param>
	/// <param name="originationFeesPercentage">Origination fee percentage.</param>
	/// <returns>Dollar cost of origination fees.</returns>
	protected static decimal CalculateOriginationFees(decimal loanAmount, decimal originationFeesPercentage)
	{
		ValidatePercentage(originationFeesPercentage);
		return (loanAmount * originationFeesPercentage) / 100;
	}
}