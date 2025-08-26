using MortgageCalculators.Extensions;
using MortgageCalculators.Models;

namespace MortgageCalculators;

public abstract class MortgageCalculator
{
    private const int PmiLtvThreshold = 80;
    private static double CalculateMonthlyInterestRate(decimal interestRate, int annualPayments, int annualCompounds)
    {
        var adjustedInterest = interestRate / 100 / annualCompounds;
        return Math.Pow((double)(1 + adjustedInterest), (annualCompounds / annualPayments)) - 1;
    }
    
    protected static decimal CalculatePayment(decimal loanAmount, decimal interest, int termInYears, int annualPayments = 12, int annualCompounds = 12)
    {
        var monthlyInterestRate = (decimal)CalculateMonthlyInterestRate(interest, annualPayments, annualCompounds);
        return loanAmount * (monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -annualPayments * termInYears)));
    }
    
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

    protected static decimal CalculateLoanToValue(decimal loanAmount, decimal homeValue)
    {
        return (loanAmount / homeValue) * 100;
    }

    protected static decimal CalculatePmiAnnualAmount(decimal loanAmount, decimal annualPmi)
    {
        return annualPmi > 0 ? ((loanAmount * annualPmi) / 100) : 0;
    }
    
    protected static bool DoesLoanHavePmi(decimal loanToValue, decimal annualPmi)
    {
        ValidatePercentage(loanToValue);
        return loanToValue > PmiLtvThreshold && annualPmi > 0;
    }

    protected static decimal RoundDownToNearestHundred(decimal amount)
    {
        return (amount / 100) * 100;
    }

    private static bool IsValidPercentage(decimal percentage)
    {
        return percentage is (>= 0 and <= 100);
    }

    private static void ValidatePercentage(decimal percentage)
    {
        if (!IsValidPercentage(percentage))
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");
    }
    
    protected static decimal CalculatePoints(decimal loanAmount, decimal pointsPercentage)
    {
        ValidatePercentage(pointsPercentage);
        return (loanAmount * pointsPercentage) / 100;
    }
    
    protected static decimal CalculateOriginationFees(decimal loanAmount, decimal originationFeesPercentage)
    {
        ValidatePercentage(originationFeesPercentage);
        return (loanAmount * originationFeesPercentage) / 100;
    }
}