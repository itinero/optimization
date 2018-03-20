namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators
{
    /// <summary>
    /// Represents a sequence.
    /// </summary>
    public struct Seq
    {
        /// <summary>
        /// Gets or sets the visits.
        /// </summary>
        /// <returns></returns>
        public int[] Visits { get; set; }

        /// <summary>
        /// The total weight of the sequence excluding any weights at first and last visit.
        /// </summary>
        /// <returns></returns>
        public float Total { get; set; }

        /// <summary>
        /// The weight excluding the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float Between { get; set; }
    }
}