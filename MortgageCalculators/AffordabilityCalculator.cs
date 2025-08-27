using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;

namespace MortgageCalculators;

/// <summary>
/// Provides methods to calculate mortgage affordability based on user income, expenses, and loan parameters.
/// </summary>
public class AffordabilityCalculator : MortgageCalculator, IMortgageCalculator<AffordabilityRequest, AffordabilityResponse>
{
    /// <summary>
    /// Calculates the maximum affordable loan amount, home value, and related monthly payments
    /// based on the provided affordability request parameters.
    /// </summary>
    /// <param name="request">The affordability request containing income, expenses, and loan details.</param>
    /// <returns>
    /// An <see cref="AffordabilityResponse"/> with calculated loan amount, down payment, home value,
    /// monthly payments, and amortization schedule.
    /// </returns>
    public AffordabilityResponse Calculate(AffordabilityRequest request)
    {
        var monthlyTaxes = request.AnnualTaxes / 12;
        var monthlyInsurance = request.AnnualInsurance / 12;
        
        var maxFront = request.FrontRatio * request.TotalMonthlyIncome / 100;
        var maxBack = (request.BackRatio * request.TotalMonthlyIncome / 100) - request.TotalMonthlyExpenses;
        var maxMonthlyPayment = Math.Min(maxFront, maxBack);

        var maxPI = maxMonthlyPayment - monthlyTaxes - monthlyInsurance;

        // Estimate loan amount (ignoring PMI for first pass)
        var loanAmount = CalculateLoanAmount(maxPI, request.InterestRate, request.Term);

        var homeValue = loanAmount / (1 - request.DownPayment / 100);
        var downPayment = homeValue - loanAmount;

        var loanToValue = CalculateLoanToValue(loanAmount, homeValue);
        var monthlyPmi = DoesLoanHavePmi(loanToValue, request.Pmi) ? (loanAmount * request.Pmi / 100) / 12 : 0;

        // Recalculate max PI if PMI applies
        if (monthlyPmi > 0)
        {
            maxPI -= monthlyPmi;
            loanAmount = CalculateLoanAmount(maxPI, request.InterestRate, request.Term);
            homeValue = loanAmount / (1 - request.DownPayment / 100);
            downPayment = homeValue - loanAmount;
        }

        loanAmount = RoundDownToNearestHundred(loanAmount);
        downPayment = RoundDownToNearestHundred(downPayment);
        homeValue = loanAmount + downPayment;

        var totalPaymentPeriods = request.Term * 12;
        var amortization = CalculateAmortization(loanAmount, request.InterestRate, totalPaymentPeriods, DateTime.Now,
            homeValue, request.Pmi);
        
        return new AffordabilityResponse
        {
            MonthlyPrincipalAndInterest = maxPI.ToDollar(),
            MonthlyTaxes = monthlyTaxes.ToDollar(),
            MonthlyInsurance = monthlyInsurance.ToDollar(),
            MonthlyPmi = monthlyPmi.ToDollar(),
            MonthlyTotal = (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi).ToDollar(),
            MonthlyIncome = request.TotalMonthlyIncome.ToDollar(),
            MonthlyExpenses = request.TotalMonthlyExpenses.ToDollar(),
            ActualFrontRatio = 100 * (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi) / request.TotalMonthlyIncome,
            ActualBackRatio = 100 * (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi + request.TotalMonthlyExpenses) / request.TotalMonthlyIncome,
            LoanAmount = loanAmount.ToDollar(),
            DownPayment = downPayment.ToDollar(),
            HomeValue = homeValue.ToDollar(),
            Amortization = amortization
        };
    }
}