using Calculators.Interfaces;
using Calculators.Models;
using Calculators.Extensions;

namespace Calculators;

public class MonthlyPaymentCalculator : MortgageCalculator, IMortgageCalculator<MonthlyPaymentRequest, MonthlyPaymentResponse>
{
    public MonthlyPaymentResponse Calculate(MonthlyPaymentRequest request)
    {
        var loanToValue =  CalculateLoanToValue(request.LoanAmount, request.HomeValue);
        var pmi = DoesLoanHavePmi(loanToValue, request.Pmi) ? CalculatePmiAnnualAmount(request.LoanAmount, request.Pmi) : 0;
        var monthlyPmi = pmi / 12;
        var monthlyTaxes = request.AnnualTaxes / 12;
        var monthlyInsurance = request.AnnualInsurance / 12;
        var monthlyPrincipalAndInterest = CalculatePayment(request.LoanAmount, request.InterestRate, request.Term);
        decimal[] payments = [ monthlyPrincipalAndInterest, monthlyTaxes, monthlyInsurance, monthlyPmi ];
        var monthlyPayment = payments.Sum();
        var monthsWithPmi = 0;

        var amortization = CalculateAmortization(request.LoanAmount, request.InterestRate, request.Term * 12, DateTime.Now, request.HomeValue, request.Pmi);

        foreach (var (schedule, i) in amortization.Schedule.Select((s, i) => (s, i)))
        {
            var ltv = CalculateLoanToValue(schedule.Balance, request.HomeValue);
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

        return new MonthlyPaymentResponse
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