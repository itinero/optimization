using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.TSP_TW_D
{
    internal static class TSPTWDFitness
    {
        /// <summary>
        /// Calculates a fitness value for the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="problem">The problem.</param>
        /// <param name="waitingTimePenaltyFactor">A penalty applied to time window violations as a factor. A very high number compared to travel times.</param>
        /// <param name="timeWindowViolationPenaltyFactor">A penalty applied to waiting times as a factor.</param>
        /// <returns>A fitness value that reflects violations of time windows by huge penalties.</returns>
        public static float Fitness(this Tour tour, TSPTWDProblem problem, float waitingTimePenaltyFactor = 0.9f,
            float timeWindowViolationPenaltyFactor = 1000000)
        {
            var violations = 0.0f;
            var waitingTime = 0.0f; // waits are not really a violation but a waste.
            var time = 0.0f;
            var travelTime = 0.0f;
            var previous = Tour.NOT_SET;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        var weight = problem.Weight(DirectedHelper.ExtractDepartureWeightId(previous), 
                            DirectedHelper.ExtractArrivalWeightId(current));
                        time += weight;
                        travelTime += weight;
                    }

                    var turn = DirectedHelper.ExtractTurn(current);
                    var turnPenalty = problem.TurnPenalty(turn);
                    time += turnPenalty;
                    travelTime += turnPenalty;

                    var window = problem.Windows[DirectedHelper.ExtractVisit(current)];
                    if (!window.IsEmpty)
                    {
                        if (window.Max < time)
                        {
                            // ok, unfeasible.
                            violations += time - window.Max;
                        }

                        if (window.Min > time)
                        {
                            // wait here!
                            waitingTime += (window.Min - time);
                            time = window.Min;
                        }
                    }

                    previous = current;
                }
            }

            if (tour.First == tour.Last &&
                previous != Tour.NOT_SET)
            {
                var weight = problem.Weight(DirectedHelper.ExtractDepartureWeightId(previous), DirectedHelper.ExtractDepartureWeightId(tour.First));
                travelTime += weight;
            }

            if (violations > float.Epsilon &&
                violations < 1)
            { // make the violations at least 1 if there are any, to make sure the penalty
              // stays unacceptable even for tiny violations.
                violations = 1;
            }

            return (violations * timeWindowViolationPenaltyFactor) + travelTime + (waitingTime * waitingTimePenaltyFactor);
        }
    }
}