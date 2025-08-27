using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;

namespace MortgageCalculators;

/// <summary>
/// Provides methods to calculate mortgage affordability based on user income, expenses, and loan parameters.
/// </summary>
public class AffordabilityCalculator : MortgageCalculator, IMortgageCalculator<AffordabilityCalculatorRequest, AffordabilityCalculatorResponse>
{
    /// <summary>
    /// Calculates the maximum affordable loan amount, home value, and related monthly payments
    /// based on the provided affordability request parameters.
    /// </summary>
    /// <param name="calculatorRequest">The affordability request containing income, expenses, and loan details.</param>
    /// <returns>
    /// An <see cref="AffordabilityCalculatorResponse"/> with calculated loan amount, down payment, home value,
    /// monthly payments, and amortization schedule.
    /// </returns>
    public AffordabilityCalculatorResponse Calculate(AffordabilityCalculatorRequest calculatorRequest)
    {
        var monthlyTaxes = calculatorRequest.AnnualTaxes / 12;
        var monthlyInsurance = calculatorRequest.AnnualInsurance / 12;
        
        var maxFront = calculatorRequest.FrontRatio * calculatorRequest.TotalMonthlyIncome / 100;
        var maxBack = (calculatorRequest.BackRatio * calculatorRequest.TotalMonthlyIncome / 100) - calculatorRequest.TotalMonthlyExpenses;
        var maxMonthlyPayment = Math.Min(maxFront, maxBack);

        var maxPI = maxMonthlyPayment - monthlyTaxes - monthlyInsurance;

        // Estimate loan amount (ignoring PMI for first pass)
        var loanAmount = CalculateLoanAmount(maxPI, calculatorRequest.InterestRate, calculatorRequest.Term);

        var homeValue = loanAmount / (1 - calculatorRequest.DownPayment / 100);
        var downPayment = homeValue - loanAmount;

        var loanToValue = CalculateLoanToValue(loanAmount, homeValue);
        var monthlyPmi = DoesLoanHavePmi(loanToValue, calculatorRequest.Pmi) ? (loanAmount * calculatorRequest.Pmi / 100) / 12 : 0;

        // Recalculate max PI if PMI applies
        if (monthlyPmi > 0)
        {
            maxPI -= monthlyPmi;
            loanAmount = CalculateLoanAmount(maxPI, calculatorRequest.InterestRate, calculatorRequest.Term);
            homeValue = loanAmount / (1 - calculatorRequest.DownPayment / 100);
            downPayment = homeValue - loanAmount;
        }

        loanAmount = RoundDownToNearestHundred(loanAmount);
        downPayment = RoundDownToNearestHundred(downPayment);
        homeValue = loanAmount + downPayment;

        var totalPaymentPeriods = calculatorRequest.Term * 12;
        var amortization = CalculateAmortization(loanAmount, calculatorRequest.InterestRate, totalPaymentPeriods, DateTime.Now,
            homeValue, calculatorRequest.Pmi);
        
        return new AffordabilityCalculatorResponse
        {
            MonthlyPrincipalAndInterest = maxPI.ToDollar(),
            MonthlyTaxes = monthlyTaxes.ToDollar(),
            MonthlyInsurance = monthlyInsurance.ToDollar(),
            MonthlyPmi = monthlyPmi.ToDollar(),
            MonthlyTotal = (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi).ToDollar(),
            ActualFrontRatio = 100 * (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi) / calculatorRequest.TotalMonthlyIncome,
            ActualBackRatio = 100 * (maxPI + monthlyTaxes + monthlyInsurance + monthlyPmi + calculatorRequest.TotalMonthlyExpenses) / calculatorRequest.TotalMonthlyIncome,
            LoanAmount = loanAmount.ToDollar(),
            DownPayment = downPayment.ToDollar(),
            HomeValue = homeValue.ToDollar(),
            Amortization = amortization
        };
    }
}