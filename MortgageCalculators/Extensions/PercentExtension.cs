namespace MortgageCalculators.Extensions;

/// <summary>
/// Utility extensions for percentage rounding.
/// </summary>
public static class PercentExtension
{
	/// <summary>
	/// Rounds a percentage to two fractional digits for display/consistency (e.g., 85.714285 becomes 85.71).
	/// </summary>
	/// <param name="value">The percentage to round.</param>
	/// <returns>The percentage rounded to two decimal places.</returns>
	public static decimal ToPercent(this decimal value)
	{
		return decimal.Round(value, 2);
	}
}
