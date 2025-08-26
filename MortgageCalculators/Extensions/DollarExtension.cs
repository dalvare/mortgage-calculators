namespace MortgageCalculators.Extensions;

public static class DollarExtension
{
    public static decimal ToDollar(this decimal value)
    {
        return decimal.Round(value, 2);
    }
}