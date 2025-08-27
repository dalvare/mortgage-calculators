namespace MortgageCalculators.Extensions;

/// <summary>
/// Utility extensions for currency-related formatting and rounding.
/// </summary>
public static class DollarExtension
{
	/// <summary>
	/// Rounds a decimal value to two fractional digits for currency display/consistency.
	/// </summary>
	/// <param name="value">The value to round.</param>
	/// <returns>The value rounded to two decimal places.</returns>
	public static decimal ToDollar(this decimal value)
	{
		return decimal.Round(value, 2);
	}
}