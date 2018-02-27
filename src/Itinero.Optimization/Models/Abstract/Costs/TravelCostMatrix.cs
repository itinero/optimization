namespace Itinero.Optimization.Models.Abstract.Costs
{
    /// <summary>
    /// Represents a travel cost matrix, the costs between visits, this could be distance, time or some custom metric.
    /// </summary>
    public class TravelCostMatrix
    {
        /// <summary>
        /// Gets or sets the name of the type of metric used.
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; } = Metrics.Time;

        /// <summary>
        /// Gets or sets the cost matrix.
        /// </summary>
        public float[][] Costs { get;set; }

        /// <summary>
        /// Gets or sets the directed flag.
        /// </summary>
        /// <remarks>If this is true there are 4 costs per visit pair, for each direction to leave/arrive.</remarks>
        public bool Directed { get; set; }
    }
}