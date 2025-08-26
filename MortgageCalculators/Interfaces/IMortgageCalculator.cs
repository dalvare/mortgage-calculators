namespace MortgageCalculators.Interfaces;

public interface IMortgageCalculator<TRequest, TResponse>
{
    TResponse Calculate(TRequest request);
}