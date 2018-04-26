using System.Collections.Generic;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a collection of penalties.
    /// </summary>
    public class Penalties : Dictionary<Pair, Penalty>
    {
        /// <summary>
        /// Gets or sets the total penalty.
        /// </summary>
        /// <returns></returns>
        public float Total { get; set; } = 0;

        /// <summary>
        /// Gets or sets the worst pair travel cost.
        /// </summary>
        /// <returns></returns>
        public float Worst { get; set; } = 0;

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Penaltiy {0} - {1}", this.Total, this.Worst);
        }
    }
}