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
        /// The weight excluding the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float Between { get; set; }

        /// <summary>
        /// The original weight excluding the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float BetweenOriginal { get; set; }
    }
}