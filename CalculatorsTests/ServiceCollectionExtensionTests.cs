using Microsoft.Extensions.DependencyInjection;
using Calculators.Extensions;
using Calculators.Interfaces;
using Calculators.Models;

namespace CalculatorsTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMortgageCalculators_RegistersAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddMortgageCalculators();
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<MonthlyPaymentRequest, MonthlyPaymentResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<AffordabilityRequest, AffordabilityResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<LoanComparisonRequest, LoanComparisonResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<RefinanceRequest, RefinanceResponse>>());
    }
}