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
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<MonthlyPaymentCalculatorRequest, MonthlyPaymentCalculatorResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<AffordabilityCalculatorRequest, AffordabilityCalculatorResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<LoanComparisonCalculatorRequest, LoanComparisonCalculatorResponse>>());
        Assert.NotNull(serviceProvider.GetService<IMortgageCalculator<RefinanceCalculatorRequest, RefinanceCalculatorResponse>>());
    }
}