using System;
using System.Collections.Generic;
using System.Threading;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.EAX;
using Itinero.Optimization.Solvers.Shared.EAX.Directed;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.TSP_TW_D.EAX
{
    /// <summary>
    /// An edge assembly crossover.
    /// </summary>
    internal sealed class EAXOperator : CrossOverOperator<Candidate<TSPTWDProblem, Tour>>
    {
        private readonly int _maxOffspring;
        private readonly EAXSelectionStrategyEnum _strategy;
        private readonly bool _nn;
        private readonly RandomGenerator _random;

        /// <summary>
        /// Creates a new EAX crossover.
        /// </summary>
        public EAXOperator()
            : this(30, EAXSelectionStrategyEnum.SingleRandom, true)
        {

        }

        /// <summary>
        /// Creates a new EAX crossover.
        /// </summary>
        public EAXOperator(int maxOffspring, EAXSelectionStrategyEnum strategy, bool nn)
        {
            _maxOffspring = maxOffspring;
            _strategy = strategy;
            _nn = nn;

            _random = RandomGenerator.Default;
        }

        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public override string Name
        {
            get
            {
                switch (_strategy)
                {
                    case EAXSelectionStrategyEnum.SingleRandom when _nn:
                        return $"EAX_(SR{_maxOffspring}_NN)";
                    case EAXSelectionStrategyEnum.SingleRandom:
                        return $"EAX_(SR{_maxOffspring})";
                    case EAXSelectionStrategyEnum.MultipleRandom:
                        if (_nn)
                        {
                            return $"EAX_(MR{_maxOffspring}_NN)";
                        }
                        return $"EAX_(MR{_maxOffspring})";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private List<int> SelectCycles(
            List<int> cycles)
        {
            var starts = new List<int>();
            if (_strategy == EAXSelectionStrategyEnum.MultipleRandom)
            {
                foreach (int cycle in cycles)
                {
                    if (_random.Generate(1.0f) > 0.25)
                    {
                        starts.Add(cycle);
                    }
                }
                return starts;
            }
            else
            {
                if (cycles.Count <= 0) return starts;
                var idx = _random.Generate(cycles.Count);
                starts.Add(cycles[idx]);
                cycles.RemoveAt(idx);
            }
            return starts;
        }

        /// <summary>
        /// Applies this operator using the given solutions and produces a new solution.
        /// </summary>
        /// <returns></returns>
        public override Candidate<TSPTWDProblem, Tour> Apply(Candidate<TSPTWDProblem, Tour> candidate1, Candidate<TSPTWDProblem, Tour> candidate2)
        {
            // TODO: PERFORMANCE Check if we can reduce allocations here.
            var problem = candidate1.Problem;
            var solution1 = candidate1.Solution;
            var solution2 = candidate2.Solution;
            
            if (DirectedHelper.ExtractVisit(solution1.Last) != DirectedHelper.ExtractVisit(problem.Last)) { throw new ArgumentException("Tour and problem have to have the same last customer."); }
            if (DirectedHelper.ExtractVisit(solution2.Last) != DirectedHelper.ExtractVisit(problem.Last)) { throw new ArgumentException("Tour and problem have to have the same last customer."); }

            var originalProblem = problem;
            var originalSolution1 = solution1;
            var originalSolution2 = solution2;
            if (!problem.Last.HasValue)
            { // convert to closed problem.
                Logger.Log($"{typeof(EAXOperator)}.{nameof(Apply)}", TraceEventType.Warning,
                    "Performance warning: EAX operator cannot be applied to 'open' TSP's, converting problem and tours to a closed equivalent.");

                problem = problem.ClosedEquivalent;
                solution1 = new Tour(solution1, solution1.First);
                solution2 = new Tour(solution2, solution1.First);
            }
            else if (problem.First != problem.Last)
            { // last is set but is not the same as first.
                Logger.Log($"{typeof(EAXOperator)}.{nameof(Apply)}", TraceEventType.Warning,
                    "Performance warning: EAX operator cannot be applied to 'closed' TSP's with a fixed endpoint, converting problem and tours to a closed equivalent.");

                problem = problem.ClosedEquivalent;
                solution1 = new Tour(solution1, problem.First);
                solution2 = new Tour(solution2, problem.First);
                // solution1.Remove(problem.Last.Value);
                // solution2.Remove(problem.Last.Value);
            }
            
            // call the operation.
            //var nearestNeighbours = _nn ? null : problem.NearestNeighbourCache.GetNNearestNeighboursForward(10);
            var result = solution1.DoEAXWith(solution2, problem.Weight, problem.TurnPenalty, _maxOffspring, _strategy);
            var best = result.newTour;
            var fitness = result.weight;
            
            if (!originalProblem.Last.HasValue)
            { // original problem as an 'open' problem, convert to an 'open' route.
                best = new Tour(best, null);
                fitness = best.Weight(originalProblem.Weight);
            }
            else if (originalProblem.First != originalProblem.Last)
            { // original problem was a problem with a fixed last point different from the first point.
                best.InsertAfter(System.Linq.Enumerable.Last(best), originalProblem.Last.Value);
                best = new Tour(best, problem.Last.Value);
                fitness = best.Weight(originalProblem.Weight);
            }

            fitness = best.Fitness(problem);
            
            return new Candidate<TSPTWDProblem, Tour>()
            {
                Solution = best,
                Fitness = fitness,
                Problem = originalProblem
            };
        }
        
        private static readonly ThreadLocal<EAXOperator> DefaultLazy = new ThreadLocal<EAXOperator>(() => new EAXOperator());
        public static EAXOperator Default => DefaultLazy.Value;
    }
}