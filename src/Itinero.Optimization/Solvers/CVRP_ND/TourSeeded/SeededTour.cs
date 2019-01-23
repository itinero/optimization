using System.Collections.Generic;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.CVRP_ND.TourSeeded
{
    /// <summary>
    /// A seeded tour.
    /// </summary>
    internal class SeededTour
    {
        public Tour Tour { get; set; }
        
        public HashSet<int> Visits { get; set; }

        public (float weight, (string metric, float value)[] constraints) TourData { get; set; }
    }
}