using Calculators.Interfaces;
using Calculators.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Calculators.Extensions;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddMortgageCalculators(
        this IServiceCollection services)
    {
        services.TryAddScoped<IMortgageCalculator<AffordabilityRequest, AffordabilityResponse>, AffordabilityCalculator>();
        services.TryAddScoped<IMortgageCalculator<LoanComparisonRequest, LoanComparisonResponse>, LoanComparisonCalculator>();
        services.TryAddScoped<IMortgageCalculator<MonthlyPaymentRequest, MonthlyPaymentResponse>, MonthlyPaymentCalculator>();
        services.TryAddScoped<IMortgageCalculator<RefinanceRequest, RefinanceResponse>, RefinanceCalculator>();
        return services;
    }
}