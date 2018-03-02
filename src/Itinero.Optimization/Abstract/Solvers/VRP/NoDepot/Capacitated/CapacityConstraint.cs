namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// A generic max capacity constraint, this can be weight, # of items or cubic centimeters of example.
    /// </summary>
    public class CapacityConstraint
    {
        /// <summary>
        /// Gets or sets the name of this constraint.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Gets or sets the value per visit.
        /// </summary>
        public float[] Values { get; set; }
    }
}