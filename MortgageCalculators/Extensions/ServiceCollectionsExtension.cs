using FluentValidation;
using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MortgageCalculators.Validation.Validators;

namespace MortgageCalculators.Extensions;

/// <summary>
/// Provides dependency injection registration helpers for mortgage calculator services and validators.
/// </summary>
public static class ServiceCollectionsExtension
{
	/// <summary>
	/// Registers mortgage calculators and their validators into the provided <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The service collection to add registrations to.</param>
	/// <returns>The same service collection instance for chaining.</returns>
	public static IServiceCollection AddMortgageCalculators(
		this IServiceCollection services)
	{
		services.TryAddScoped<IMortgageCalculator<AffordabilityCalculatorRequest, AffordabilityCalculatorResponse>, AffordabilityCalculator>();
		services.TryAddScoped<IMortgageCalculator<LoanComparisonCalculatorRequest, LoanComparisonCalculatorResponse>, LoanComparisonCalculator>();
		services.TryAddScoped<IMortgageCalculator<MonthlyPaymentCalculatorRequest, MonthlyPaymentCalculatorResponse>, MonthlyPaymentCalculator>();
		services.TryAddScoped<IMortgageCalculator<RefinanceCalculatorRequest, RefinanceCalculatorResponse>, RefinanceCalculator>();
		
		services.TryAddScoped<IValidator<AffordabilityCalculatorRequest>, AffordabilityRequestValidator>();
		services.TryAddScoped<IValidator<LoanComparisonCalculatorRequest>, LoanComparisonRequestValidator>();
		services.TryAddScoped<IValidator<MonthlyPaymentCalculatorRequest>, MonthlyPaymentRequestValidator>();
		services.TryAddScoped<IValidator<RefinanceCalculatorRequest>, RefinanceRequestValidator>();
		return services;
	}
}