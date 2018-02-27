namespace Itinero.Optimization.Models.Costs
{
    /// <summary>
    /// Represents the costs per visit.
    /// </summary>
    public class VisitCosts
    {
        /// <summary>
        /// Gets or sets the name of the metric used for the cost.
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the costs.
        /// </summary>
        /// <returns></returns>
        public float[] Costs { get;set; }
    }
}