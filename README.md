# Mortgage Calculators

A collection of C# mortgage calculators, with input validation using [FluentValidation](https://fluentvalidation.net/).

## Features

- **Affordability Calculator**: Estimate how much home you can afford.
- **Loan Comparison Calculator**: Compare different loan options.
- **Monthly Payment Calculator**: Estimate how much your monthly payments will be.
- **Refinance Calculator**: Analyze the benefits of refinancing your mortgage.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

FluentValidation is a dependency of the package and is installed with it.

### Installation

.NET CLI:

```sh
dotnet add package MortgageCalculators
```

### Configuration

**Register the calculators and validators** (for example, in your DI container if using ASP.NET Core):

```csharp
using MortgageCalculators.Extensions;

builder.Services.AddMortgageCalculators();
```

This registers each calculator as `IMortgageCalculator<TRequest, TResponse>` and each validator as `IValidator<TRequest>`.

If you are not using dependency injection, instantiate calculators and validators directly:

```csharp
using MortgageCalculators.Validation.Validators;

var validator = new AffordabilityRequestValidator();
var result = validator.Validate(request);
if (!result.IsValid)
{
    // Handle validation errors
}
```

Validate a request before calculating. The calculators assume inputs that pass their validator and throw
`ArgumentException` or `ArgumentOutOfRangeException` for inputs they cannot handle.

## Usage

```csharp
using MortgageCalculators;
using MortgageCalculators.Models;
using MortgageCalculators.Validation.Validators;

var request = new MonthlyPaymentCalculatorRequest
{
    LoanAmount = 250000m,
    HomeValue = 300000m,
    InterestRate = 4.5m,
    Term = 30,
    AnnualTaxes = 3000m,
    AnnualInsurance = 1200m,
    Pmi = 0.5m
};

var validation = new MonthlyPaymentRequestValidator().Validate(request);
if (!validation.IsValid)
{
    // Handle validation errors
}

var calculator = new MonthlyPaymentCalculator();
MonthlyPaymentCalculatorResponse result = calculator.Calculate(request);

Console.WriteLine($"Monthly Payment: {result.MonthlyPayment:C}");
Console.WriteLine($"Principal & Interest: {result.MonthlyPrincipalAndInterest:C}");
Console.WriteLine($"Months with PMI: {result.Amortization.MonthsWithPmi}");
```

Rates and ratios are percentages (`6` for 6%), amounts are dollars, and terms are years. Down payment on the
affordability request is a percentage of the home value.

## Running Tests

Run all unit tests using:

```sh
dotnet test
```
