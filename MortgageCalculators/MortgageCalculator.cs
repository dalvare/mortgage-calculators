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
	/// Multiplies a term in years by a payment frequency, rejecting products that overflow a 32-bit period count.
	/// </summary>
	/// <param name="termInYears">The loan term in years.</param>
	/// <param name="annualPayments">Number of payments per year.</param>
	/// <returns>The total number of payment periods.</returns>
	/// <exception cref="ArgumentException">Thrown when the term and payment frequency overflow the period count.</exception>
	private static int TotalPeriods(int termInYears, int annualPayments)
	{
		var totalPeriods = (long)termInYears * annualPayments;
		if (totalPeriods > int.MaxValue)
			throw new ArgumentException("The term and payment frequency produce more payment periods than can be counted.");

		return (int)totalPeriods;
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
	/// <exception cref="ArgumentException">Thrown when the term or any frequency parameter is not a positive value, or when they overflow the period count.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the interest rate is negative.</exception>
	protected static decimal CalculatePayment(decimal loanAmount, decimal interest, int termInYears, int annualPayments = 12, int annualCompounds = 12)
	{
		if (termInYears <= 0 || annualPayments <= 0 || annualCompounds <= 0)
			throw new ArgumentException("Term, payment frequency, and compounding frequency must be positive values.");

		return CalculatePaymentForPeriods(loanAmount, interest, TotalPeriods(termInYears, annualPayments), annualPayments, annualCompounds);
	}

	/// <summary>
	/// Calculates the constant periodic payment for an explicit number of payment periods.
	/// </summary>
	/// <param name="loanAmount">The principal loan amount.</param>
	/// <param name="interest">Annual interest rate as a percentage (e.g., 6 for 6%).</param>
	/// <param name="totalPeriods">Total number of payment periods over the life of the loan.</param>
	/// <param name="annualPayments">Number of payments per year.</param>
	/// <param name="annualCompounds">Number of compounding periods per year.</param>
	/// <returns>The periodic payment amount covering principal and interest.</returns>
	/// <exception cref="ArgumentException">Thrown when the period count or any frequency parameter is not a positive value.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the interest rate is negative.</exception>
	private static decimal CalculatePaymentForPeriods(decimal loanAmount, decimal interest, int totalPeriods, int annualPayments = 12, int annualCompounds = 12)
	{
		if (totalPeriods <= 0 || annualPayments <= 0 || annualCompounds <= 0)
			throw new ArgumentException("Period count, payment frequency, and compounding frequency must be positive values.");
		if (interest < 0)
			throw new ArgumentOutOfRangeException(nameof(interest), "Interest rate cannot be negative.");

		var periodicInterestRate = (decimal)CalculateMonthlyInterestRate(interest, annualPayments, annualCompounds);

		// An interest-free loan amortizes in equal principal-only installments; the annuity formula
		// divides by zero at a zero rate.
		if (periodicInterestRate == 0)
			return loanAmount / totalPeriods;

		return loanAmount * (periodicInterestRate / (1 - (decimal)Math.Pow((double)(1 + periodicInterestRate), -totalPeriods)));
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
	/// <exception cref="ArgumentException">Thrown when the term or any frequency parameter is not a positive value, when they overflow the period count, or when the rate and term are too large to solve.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the interest rate is negative.</exception>
	protected static decimal CalculateLoanAmount(decimal periodPayment, decimal interestRate, int termInYears, int numOfAnnualPayments = 12, int annualCompounds = 12)
	{
		if (termInYears <= 0 || numOfAnnualPayments <= 0 || annualCompounds <= 0)
			throw new ArgumentException("Term, payment frequency, and compounding frequency must be positive values.");
		if (interestRate < 0)
			throw new ArgumentOutOfRangeException(nameof(interestRate), "Interest rate cannot be negative.");

		var payment = (double)periodPayment;
		var totalNumberOfPayments = TotalPeriods(termInYears, numOfAnnualPayments);
		var paymentPeriodInterestRate = CalculateMonthlyInterestRate(interestRate, numOfAnnualPayments, annualCompounds);

		// At a zero rate every installment is pure principal, and the annuity formula collapses to 0/0.
		if (paymentPeriodInterestRate == 0)
			return periodPayment * totalNumberOfPayments;

		var loanAmount = payment * (Math.Pow(1 + paymentPeriodInterestRate, totalNumberOfPayments) - 1) / (paymentPeriodInterestRate * Math.Pow(1 + paymentPeriodInterestRate, totalNumberOfPayments));

		// A very large rate or term overflows the double math to infinity, and infinity / infinity is NaN;
		// either would surface as an OverflowException from the decimal cast below.
		if (!double.IsFinite(loanAmount) || Math.Abs(loanAmount) > (double)decimal.MaxValue)
			throw new ArgumentException("The interest rate and term are too large to solve for a loan amount.");

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
	/// <exception cref="ArgumentException">Thrown when the period count is not a positive value.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the home value is not greater than zero.</exception>
	protected static Amortization CalculateAmortization(decimal principal, decimal rate, int periods, DateTime startDate, decimal homeValue, decimal annualPmi = 0)
	{
		var amortization = new Amortization
		{
			Balance = principal,
			PeriodicInterest = (rate / 100) / 12,
			Periods = periods,
			PeriodicPayment = CalculatePaymentForPeriods(principal, rate, periods),
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
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the home value is not greater than zero.</exception>
	protected static decimal CalculateLoanToValue(decimal loanAmount, decimal homeValue)
	{
		if (homeValue <= 0)
			throw new ArgumentOutOfRangeException(nameof(homeValue), "Home value must be greater than zero to calculate a loan to value ratio.");

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
		ValidateLoanToValue(loanToValue);
		return loanToValue > PmiLtvThreshold && annualPmi > 0;
	}
	
	/// <summary>
	/// Rounds a monetary value down to the nearest hundred.
	/// </summary>
	/// <remarks>
	/// Rounds toward negative infinity, so a negative amount moves away from zero (-50 becomes -100).
	/// </remarks>
	/// <param name="amount">The amount to round down.</param>
	/// <returns>The largest multiple of one hundred that is less than or equal to <paramref name="amount"/>.</returns>
	protected static decimal RoundDownToNearestHundred(decimal amount)
	{
		return Math.Floor(amount / 100) * 100;
	}
	
	private static bool IsValidPercentage(decimal value, decimal min = 0, decimal max = 100)
	{
		return value >= min && value <= max;
	}
	
	/// <summary>
	/// Validates that a percentage is within the given range.
	/// </summary>
	/// <param name="percentage">The percentage to validate.</param>
	/// <param name="min">The minimum percentage to validate.</param>
	/// <param name="max">The maximum percentage to validate.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the percentage is outside the min/max range.</exception>
	private static void ValidatePercentage(decimal percentage, decimal min = 0, decimal max = 100)
	{
		if (!IsValidPercentage(percentage, min, max))
			throw new ArgumentOutOfRangeException(nameof(percentage), $"Percentage must be between {min} and {max}. {percentage} is invalid.");
	}
	
	/// <summary>
	/// Validates that a loan to value ratio is within the inclusive range [0, 200].
	/// </summary>
	/// <param name="percentage">The percentage to validate.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the percentage is outside [0,100].</exception>
	private static void ValidateLoanToValue(decimal percentage)
	{
		const decimal min = 0;
		const decimal max = 200;
		ValidatePercentage(percentage, min, max);
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