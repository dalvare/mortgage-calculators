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
		services.TryAddScoped<IMortgageCalculator<AffordabilityRequest, AffordabilityResponse>, AffordabilityCalculator>();
		services.TryAddScoped<IMortgageCalculator<LoanComparisonRequest, LoanComparisonResponse>, LoanComparisonCalculator>();
		services.TryAddScoped<IMortgageCalculator<MonthlyPaymentRequest, MonthlyPaymentResponse>, MonthlyPaymentCalculator>();
		services.TryAddScoped<IMortgageCalculator<RefinanceRequest, RefinanceResponse>, RefinanceCalculator>();
		
		services.TryAddScoped<IValidator<AffordabilityRequest>, AffordabilityRequestValidator>();
		services.TryAddScoped<IValidator<LoanComparisonRequest>, LoanComparisonRequestValidator>();
		services.TryAddScoped<IValidator<MonthlyPaymentRequest>, MonthlyPaymentRequestValidator>();
		services.TryAddScoped<IValidator<RefinanceRequest>, RefinanceRequestValidator>();
		return services;
	}
}