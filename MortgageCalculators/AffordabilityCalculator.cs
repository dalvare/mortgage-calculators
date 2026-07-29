using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;

namespace MortgageCalculators;

/// <summary>
/// Provides methods to calculate mortgage affordability based on user income, expenses, and loan parameters.
/// </summary>
public class AffordabilityCalculator : MortgageCalculator, IMortgageCalculator<AffordabilityCalculatorRequest, AffordabilityCalculatorResponse>
{
    private const string UnaffordableMessage =
        "Income and qualifying ratios leave nothing for a principal and interest payment after taxes, insurance, and PMI.";

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
        if (calculatorRequest.DownPayment >= 100)
            throw new ArgumentOutOfRangeException(nameof(calculatorRequest), "Down payment percentage must be less than 100.");

        var monthlyTaxes = calculatorRequest.AnnualTaxes / 12;
        var monthlyInsurance = calculatorRequest.AnnualInsurance / 12;

        var (maxPI, monthlyPmi) = SolveMaxPrincipalAndInterest(calculatorRequest);
        if (maxPI <= 0)
            throw new ArgumentOutOfRangeException(nameof(calculatorRequest), UnaffordableMessage);

        var loanAmount = CalculateLoanAmount(maxPI, calculatorRequest.InterestRate, calculatorRequest.Term);
        var homeValue = loanAmount / (1 - calculatorRequest.DownPayment / 100);
        var downPayment = homeValue - loanAmount;

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

    /// <summary>
    /// Determines whether the request leaves any room for a principal and interest payment once taxes,
    /// insurance, and PMI are covered.
    /// </summary>
    /// <param name="request">The affordability request to test.</param>
    /// <returns>True when a positive principal and interest payment is affordable; otherwise false.</returns>
    internal static bool CanFundPrincipalAndInterest(AffordabilityCalculatorRequest request)
    {
        // Structurally invalid requests are reported by the field-level rules; a validator must not throw.
        if (request.Term <= 0 || request.InterestRate <= 0 || request.DownPayment is < 0 or >= 100)
            return true;

        return SolveMaxPrincipalAndInterest(request).MaxPrincipalAndInterest > 0;
    }

    /// <summary>
    /// Solves for the monthly principal and interest a borrower can support, net of the PMI the resulting
    /// loan would carry. A non-positive result means the request affords no loan at all.
    /// </summary>
    private static (decimal MaxPrincipalAndInterest, decimal MonthlyPmi) SolveMaxPrincipalAndInterest(
        AffordabilityCalculatorRequest request)
    {
        var monthlyTaxes = request.AnnualTaxes / 12;
        var monthlyInsurance = request.AnnualInsurance / 12;

        var maxFront = request.FrontRatio * request.TotalMonthlyIncome / 100;
        var maxBack = (request.BackRatio * request.TotalMonthlyIncome / 100) - request.TotalMonthlyExpenses;
        var maxMonthlyPayment = Math.Min(maxFront, maxBack);

        var maxPI = maxMonthlyPayment - monthlyTaxes - monthlyInsurance;
        if (maxPI <= 0)
            return (maxPI, 0);

        // First pass ignores PMI, then charges the PMI the resulting loan would carry against the payment.
        var loanAmount = CalculateLoanAmount(maxPI, request.InterestRate, request.Term);
        var homeValue = loanAmount / (1 - request.DownPayment / 100);

        var loanToValue = CalculateLoanToValue(loanAmount, homeValue);
        var monthlyPmi = DoesLoanHavePmi(loanToValue, request.Pmi) ? (loanAmount * request.Pmi / 100) / 12 : 0;

        return (maxPI - monthlyPmi, monthlyPmi);
    }
}