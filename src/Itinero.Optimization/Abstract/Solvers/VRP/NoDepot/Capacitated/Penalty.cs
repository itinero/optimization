namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// A penalty.
    /// </summary>
    public struct Penalty
    {
        /// <summary>
        /// The penalization count.
        /// </summary>
        /// <returns></returns>
        public byte Count { get; set; }

        /// <summary>
        /// The original weight.
        /// </summary>
        /// <returns></returns>
        public float Original { get; set; }

        /// <summary>
        /// Gets an empty penalty.
        /// </summary>
        /// <returns></returns>
        public static Penalty Empty = new Penalty()
        {
            Count = 0,
            Original = -1
        };
    }
}