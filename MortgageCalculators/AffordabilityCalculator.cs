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
    /// <remarks>
    /// The loan amount is rounded down to the nearest hundred. The home value and down payment are then derived
    /// from the rounded loan and the requested down payment percentage, so the loan-to-value ratio that decides
    /// PMI is exactly the one the caller asked for. Every monthly figure in the response describes the rounded
    /// loan rather than the qualifying maximum it was solved from, so the payment, PMI, and amortization
    /// schedule agree with the reported loan amount.
    /// </remarks>
    /// <param name="calculatorRequest">The affordability request containing income, expenses, and loan details.</param>
    /// <returns>
    /// An <see cref="AffordabilityCalculatorResponse"/> with calculated loan amount, down payment, home value,
    /// monthly payments, and amortization schedule.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the down payment percentage is 100 or more, or when the request cannot fund a loan of at
    /// least one hundred dollars once taxes, insurance, and PMI are covered.
    /// </exception>
    public AffordabilityCalculatorResponse Calculate(AffordabilityCalculatorRequest calculatorRequest)
    {
        if (calculatorRequest.DownPayment >= 100)
            throw new ArgumentOutOfRangeException(nameof(calculatorRequest), "Down payment percentage must be less than 100.");

        var monthlyTaxes = calculatorRequest.AnnualTaxes / 12;
        var monthlyInsurance = calculatorRequest.AnnualInsurance / 12;

        var loanAmount = SolveMaxLoanAmount(calculatorRequest);
        if (loanAmount <= 0)
            throw new ArgumentOutOfRangeException(nameof(calculatorRequest), UnaffordableMessage);

        var homeValue = loanAmount / (1 - calculatorRequest.DownPayment / 100);
        var downPayment = homeValue - loanAmount;

        var monthlyPrincipalAndInterest = CalculatePayment(loanAmount, calculatorRequest.InterestRate, calculatorRequest.Term);
        var monthlyPmi = DoesLoanHavePmi(CalculateLoanToValue(loanAmount, homeValue), calculatorRequest.Pmi)
            ? CalculatePmiAnnualAmount(loanAmount, calculatorRequest.Pmi) / 12
            : 0;
        var monthlyTotal = monthlyPrincipalAndInterest + monthlyTaxes + monthlyInsurance + monthlyPmi;

        var totalPaymentPeriods = calculatorRequest.Term * 12;
        var amortization = CalculateAmortization(loanAmount, calculatorRequest.InterestRate, totalPaymentPeriods, DateTime.Now,
            homeValue, calculatorRequest.Pmi);

        return new AffordabilityCalculatorResponse
        {
            MonthlyPrincipalAndInterest = monthlyPrincipalAndInterest.ToDollar(),
            MonthlyTaxes = monthlyTaxes.ToDollar(),
            MonthlyInsurance = monthlyInsurance.ToDollar(),
            MonthlyPmi = monthlyPmi.ToDollar(),
            MonthlyTotal = monthlyTotal.ToDollar(),
            ActualFrontRatio = (100 * monthlyTotal / calculatorRequest.TotalMonthlyIncome).ToPercent(),
            ActualBackRatio = (100 * (monthlyTotal + calculatorRequest.TotalMonthlyExpenses) / calculatorRequest.TotalMonthlyIncome).ToPercent(),
            LoanAmount = loanAmount.ToDollar(),
            DownPayment = downPayment.ToDollar(),
            HomeValue = homeValue.ToDollar(),
            Amortization = amortization
        };
    }

    /// <summary>
    /// Determines whether the request can fund a loan of at least one hundred dollars once taxes, insurance,
    /// and PMI are covered. This is the same test <see cref="Calculate"/> applies before producing a response.
    /// </summary>
    /// <remarks>
    /// Expects a request that already satisfies the field-level validation rules; out-of-range inputs may throw.
    /// </remarks>
    /// <param name="request">The affordability request to test.</param>
    /// <returns>True when the request affords a loan; otherwise false.</returns>
    internal static bool CanAffordLoan(AffordabilityCalculatorRequest request)
    {
        return SolveMaxLoanAmount(request) > 0;
    }

    /// <summary>
    /// Solves for the largest loan the request can support, rounded down to the nearest hundred.
    /// Returns zero when the request affords no loan at all.
    /// </summary>
    /// <remarks>
    /// When PMI applies, the loan must satisfy <c>payment(loan) + pmi(loan) = allowance</c>. Both terms are
    /// proportional to the loan, so the loan is solved in closed form rather than by iterating: with
    /// <c>L0</c> the loan the whole allowance would fund and <c>r</c> the monthly PMI rate per dollar borrowed,
    /// <c>L = L0 * allowance / (allowance + r * L0)</c>. Whether PMI applies depends only on the down payment
    /// percentage, because the loan-to-value ratio it sets is the same for any loan size.
    /// </remarks>
    private static decimal SolveMaxLoanAmount(AffordabilityCalculatorRequest request)
    {
        var maxPrincipalInterestAndPmi = SolveMaxPaymentForLoan(request);
        if (maxPrincipalInterestAndPmi <= 0)
            return 0;

        var loanAmount = CalculateLoanAmount(maxPrincipalInterestAndPmi, request.InterestRate, request.Term);

        var loanToValue = 100 - request.DownPayment;
        if (DoesLoanHavePmi(loanToValue, request.Pmi))
        {
            var monthlyPmiPerDollar = request.Pmi / 100 / 12;
            loanAmount = loanAmount * maxPrincipalInterestAndPmi / (maxPrincipalInterestAndPmi + monthlyPmiPerDollar * loanAmount);
        }

        return RoundDownToNearestHundred(loanAmount);
    }

    /// <summary>
    /// Solves for the monthly amount left for principal, interest, and PMI once taxes and insurance are taken
    /// from the smaller of the front-ratio and back-ratio allowances. A non-positive result means the request
    /// affords no loan at all.
    /// </summary>
    private static decimal SolveMaxPaymentForLoan(AffordabilityCalculatorRequest request)
    {
        var monthlyTaxes = request.AnnualTaxes / 12;
        var monthlyInsurance = request.AnnualInsurance / 12;

        var maxFront = request.FrontRatio * request.TotalMonthlyIncome / 100;
        var maxBack = (request.BackRatio * request.TotalMonthlyIncome / 100) - request.TotalMonthlyExpenses;
        var maxMonthlyPayment = Math.Min(maxFront, maxBack);

        return maxMonthlyPayment - monthlyTaxes - monthlyInsurance;
    }
}
