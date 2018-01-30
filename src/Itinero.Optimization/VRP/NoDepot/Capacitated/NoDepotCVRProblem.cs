using System;
using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated
{
    public class NoDepotCVRProblem
    {
        /// <summary>
        /// The vehicle capacity.!--
        /// </summary>
        /// <returns></returns>
        public float Max { get; set; } 

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }
    }
}
