# Mortgage Calculators

A collection of C# calculators for mortgage calculators using [FluentValidation](https://fluentvalidation.net/).

## Features

- **Affordability Calculator**: Estimate how much home you can afford.
- **Loan Comparison Calculator**: Compare different loan options.
- **Refinance Calculator**: Analyze the benefits of refinancing your mortgage.
- **Input Validation**: All request models are validated using FluentValidation.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [FluentValidation](https://fluentvalidation.net/) NuGet package

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/dalvare/mortgage-calculators.git
    cd mortgage-calculators
    ```

2. Restore dependencies:
    ```sh
    dotnet restore
    ```

### Configuring FluentValidation

1. **Install FluentValidation** (if not already installed):

    ```sh
    dotnet add package FluentValidation
    ```

2. **Register Validators** (for example, in your DI container if using ASP.NET Core):

    ```csharp
    using FluentValidation;
    using Calculators.Models;
    using Calculators.Validation.Validators;

    builder.Services.AddValidatorsFromAssemblyContaining<AffordabilityRequestValidator>();
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

3. **Using Validators in Tests**

   The project uses [FluentValidation.TestHelper](https://docs.fluentvalidation.net/en/latest/testing.html) for unit testing validators. Example:

    ```csharp
    var validator = new AffordabilityRequestValidator();
    var result = validator.TestValidate(request);
    result.ShouldNotHaveAnyValidationErrors();
    ```

## Running Tests

Run all unit tests using:

```sh
dotnet test