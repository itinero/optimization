using System;
using System.Linq;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.EAX;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.EAX.Directed
{
    internal static class EAXOperation
    {
        public static (bool success, float weight, Tour? newTour) DoEAXWith(this Tour tour1, Tour tour2,
            Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenaltyFunc,
            int maxOffspring = 30, EAXSelectionStrategyEnum strategy = EAXSelectionStrategyEnum.SingleRandom)
        {
            var undirected1 = new Tour(tour1.Select(DirectedHelper.ExtractVisit),
                DirectedHelper.ExtractVisit(tour1.Last));
            var undirected2 = new Tour(tour2.Select(DirectedHelper.ExtractVisit),
                DirectedHelper.ExtractVisit(tour2.Last));

            var (success, _, newTour) = Itinero.Optimization.Solvers.Shared.EAX.EAXOperation.DoEAXWith(undirected1, undirected2,
                (v1, v2) => weightFunc.UndirectedWeight(v1, v2),
                maxOffspring, strategy);
            if (!success)
            {
                return (false, 0, null);
            }

            var (weight, directedTour) =  newTour.ConvertToDirectedAndOptimizeTurns(weightFunc, turnPenaltyFunc);

            return (true, weight, directedTour);
        }
    }
}