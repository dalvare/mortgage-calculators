namespace MortgageCalculators.Models
{
    /// <summary>
    /// Simple helper model representing a duration in years and months.
    /// </summary>
    public class YearsMonths
    {
        /// <summary>
        /// Number of full years.
        /// </summary>
        public int? Years { get; set; }
        /// <summary>
        /// Number of remaining months.
        /// </summary>
        public int? Months { get; set; }
    }
} 