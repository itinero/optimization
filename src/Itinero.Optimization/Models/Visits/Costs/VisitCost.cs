namespace Itinero.Optimization.Models.Visits.Costs
{
    /// <summary>
    /// Represents a visit cost, a metric and value.
    /// </summary>
    public class VisitCost
    {
        /// <summary>
        /// Gets or sets the metric id.
        /// </summary>
        public string Metric { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float Value { get; set; }
        
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Value}({this.Metric})";
        }
    }
}