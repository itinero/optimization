using System.Collections.Generic;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Shared.Seeds;

namespace Itinero.Optimization.Tests.Functional.Solvers.Shared.Seeds
{
    public class SeedHeuristicsKMeansTest :  FunctionalTest<int[], (List<int> visits, Coordinate[] locations, int vehicleCount)>
    {
        /// <summary>
        /// Gets the default test.
        /// </summary>
        public static SeedHeuristicsKMeansTest Default => new SeedHeuristicsKMeansTest();

        protected override int[] Execute((List<int> visits, Coordinate[] locations, int vehicleCount) input)
        {
            return SeedHeuristics.GetSeedsKMeans(input.visits, input.vehicleCount, (v) => input.locations[v]);
        }
    }
}