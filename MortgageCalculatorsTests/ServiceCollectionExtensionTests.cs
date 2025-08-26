using Microsoft.Extensions.DependencyInjection;
using MortgageCalculators.Extensions;
using MortgageCalculators.Interfaces;
using MortgageCalculators.Models;

namespace MortgageCalculatorsTests;

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