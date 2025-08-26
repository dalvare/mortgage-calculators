# Mortgage Calculators

A collection of C# mortgage calculators, with input validation using [FluentValidation](https://fluentvalidation.net/).

## Features

- **Affordability Calculator**: Estimate how much home you can afford.
- **Loan Comparison Calculator**: Compare different loan options.
- **Monthly Payment Calculator**: Estimate how much your monthly payments will be.
- **Refinance Calculator**: Analyze the benefits of refinancing your mortgage.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [FluentValidation](https://fluentvalidation.net/) NuGet package

### Installation

.NET CLI:
    ```sh
    dotnet add package MortgageCalculators --version 1.0.0
    ```

### Configuration

1. **Register Calculators** (for example, in your DI container if using ASP.NET Core):

    ```csharp
    using MortgageCalculators.Extensions;

    builder.Services.AddMortgageCalcuators();
    ```

   If you are not using dependency injection, you can instantiate validators directly:

    ```csharp
    var validator = new AffordabilityRequestValidator();
    var result = validator.Validate(request);
    if (!result.IsValid)
    {
        // Handle validation errors
    }
    ```
## Usage
```csharp
using MortgageCalculators;
using MortgageCalculators.Models;

var request = new MonthlyPaymentRequest
{
    LoanAmount = 250000m,
    InterestRate = 4.5m,
    Term = 30,
    PropertyTax = 3000m,
    HomeInsurance = 1200m,
    Pmi = 0.5m
};

var calculator = new MonthlyPaymentCalculator();
MonthlyPaymentResult result = calculator.Calculate(request);

Console.WriteLine($"Monthly Payment: {result.MonthlyPayment:C}");
```
## Running Tests

Run all unit tests using:

```sh
dotnet test