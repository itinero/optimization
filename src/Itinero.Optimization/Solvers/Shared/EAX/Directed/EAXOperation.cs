using System;
using Itinero.Optimization.Solvers.Shared.EAX;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.EAX.Directed
{
    internal static class EAXOperation
    {
        public static (bool success, float weight, Tour newTour) DoEAXWith(this Tour tour1, Tour tour2,
            Func<int, int, float> weightFunc, float turnPenalty,
            int maxOffspring = 30, EAXSelectionStrategyEnum strategy = EAXSelectionStrategyEnum.SingleRandom)
        {
            return (false, 0, null);
        }
    }
}