namespace MortgageCalculators.Interfaces;

/// <summary>
/// Defines a mortgage calculator capable of computing a response from a given request type.
/// </summary>
/// <typeparam name="TRequest">Request DTO type containing input parameters.</typeparam>
/// <typeparam name="TResponse">Response DTO type containing computed results.</typeparam>
public interface IMortgageCalculator<TRequest, TResponse>
{
	/// <summary>
	/// Calculates the mortgage-related response for the provided request.
	/// </summary>
	/// <param name="request">Input model with parameters required to compute the result.</param>
	/// <returns>The computed response for the request.</returns>
	TResponse Calculate(TRequest request);
}