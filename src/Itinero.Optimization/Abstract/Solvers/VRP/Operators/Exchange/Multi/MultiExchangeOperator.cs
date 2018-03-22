using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Random;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi
{
    /// <summary>
    /// An improvement operator that tries to exchange parts of routes.
    /// </summary>
    /// <remarks>
    /// This follows a 'stop on first'-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 tours (random, given or all pairs once).
    /// - Loop over all edge-ranges in tour1.
    ///   - Loop over all edge-ranges in tour2.
    ///     - Check if a swap between tours improves things.
    /// 
    /// The search stops from the moment any improvement is found unless configured to keep testing all tour pairs.
    /// </remarks>
    public class MultiExchangeOperator<TObjective, TProblem, TSolution> : IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>
        where TObjective : IMultiExchangeObjective<TProblem, TSolution>
        where TSolution : IMultiExchangeSolution
        {
            private readonly int _maxWindowSize = 8;
            private readonly int _minWindowSize = 2;
            private readonly bool _tryReversed = true;
            private readonly bool _tryAll = false;
            private readonly bool _bestImprovement = false;

            /// <summary>
            /// Creates a new operator.
            /// </summary>
            /// <param name="minWindowSize">The minimum window size to search for sequences to exchange.</param>
            /// <param name="maxWindowSize">The maximum window size to search for sequences to exchange.</param>
            /// <param name="tryReversed">True when exchanged sequenced also need to be reversed before testing.</param>
            /// <param name="tryAll">True when all tour pairs need to be tested.</param>
            /// <param name="bestImprovement">When true the best-improvement is chosen, not the first.</param>
            public MultiExchangeOperator(int minWindowSize = 2, int maxWindowSize = 8, bool tryReversed = true, bool tryAll = false, bool bestImprovement = false)
            {
                _tryAll = tryAll;
                _minWindowSize = minWindowSize;
                _maxWindowSize = maxWindowSize;
                _tryReversed = tryReversed;
                _bestImprovement = bestImprovement;
            }

            /// <summary>
            /// Gets the name of this operator.
            /// </summary>
            public string Name
            {
                get
                {
                    var name = string.Format("CROSS-MUL-{0}-{1}", _minWindowSize, _maxWindowSize);
                    if (_tryAll)
                    {
                        name += "_(ALL)";
                    }
                    if (_tryReversed)
                    {
                        name += "_(REV)";
                    }
                    return name;
                }
            }

            /// <summary>
            /// Returns true if it doesn't matter if tour indexes are switched.
            /// </summary>
            public bool IsSymmetric => true;

            /// <summary>
            /// Applies this operator.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="objective">The objective.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="delta">The difference in fitness.</param>
            /// <returns></returns>        
            public bool Apply(TProblem problem, TObjective objective, TSolution solution, out float delta)
            {
                if (_tryAll)
                {
                    if (_bestImprovement)
                    { // try all and do best improvement.
                        return this.ApplyBestImprovement(problem, objective, solution, out delta);
                    }

                    // try all and first improvement.
                    delta = 0;
                    for (var t1 = 0; t1 < solution.Count; t1++)
                    {
                        for (var t2 = 0; t2 < t1; t2++)
                        {
                            if (t1 == t2)
                            {
                                continue;
                            }

                            if (this.Apply(problem, objective, solution, t1, t2, out float localDelta))
                            { // success!
                                delta += localDelta;
                                //improved = true;
                            }
                        }
                    }

                    return delta != 0;
                }
                else
                { // just choose random routes.
                    // check if solution has at least two tours.
                    if (solution.Count < 2)
                    {
                        delta = 0;
                        return false;
                    }

                    // choose two random routes.
                    var random = RandomGeneratorExtensions.GetRandom();
                    var tourIdx1 = random.Generate(solution.Count);
                    var tourIdx2 = random.Generate(solution.Count - 1);
                    if (tourIdx2 >= tourIdx1)
                    {
                        tourIdx2++;
                    }

                    return Apply(problem, objective, solution, tourIdx1, tourIdx2, out delta);
                }
            }

            /// <summary>
            /// Applies this operator but using best-improvement.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="objective">The objective.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="delta">The difference in fitness.</param>
            /// <returns></returns>
            private bool ApplyBestImprovement(TProblem problem, TObjective objective, TSolution solution, out float delta)
            {
                // go over all combinations and choose the best.
                var bestDelta = 0f;
                Seq? bestS1 = null;
                Seq? bestS2 = null;
                var bestT1 = -1;
                var bestT2 = -1;

                for (var t1 = 0; t1 < solution.Count; t1++)
                {
                    for (var t2 = 0; t2 < t1; t2++)
                    {
                        if (t1 == t2)
                        {
                            continue;
                        }

                        var tour1 = solution.Tour(t1);
                        var tour2 = solution.Tour(t2);

                        var tour1Enumerable = objective.SeqAndSmaller(problem, tour1, _minWindowSize + 2, _maxWindowSize + 2);
                        var tour2Enumerable = objective.SeqAndSmaller(problem, tour2, _minWindowSize + 2, _maxWindowSize + 2);

                        // loop over all sequences of size 4->maxWindowSize + 2. 
                        // - A minimum of 4 because otherwise we exchange just one visit.
                        // - The edge to be exchanged are also included.
                        foreach (var s1 in tour1Enumerable)
                        {
                            // switch s1.
                            Seq? s1Rev = null;
                            if (_tryReversed)
                            { // only setup reversed data if needed.
                                s1Rev = objective.Reverse(problem, s1);
                            }

                            foreach (var s2 in tour2Enumerable)
                            {
                                // try exchanging without change order.
                                if (objective.SimulateSwap(problem, solution, t1, t2, s1, s2, out float localDelta) &&
                                    localDelta > bestDelta)
                                { // exchange succeeded.
                                    bestDelta = localDelta;
                                    bestT1 = t1;
                                    bestT2 = t2;
                                    bestS1 = s1;
                                    bestS2 = s2;
                                }

                                if (!_tryReversed)
                                { // don't try the other options with sequences reversed.
                                    continue;
                                }

                                // try exchanging with s1 reversed.
                                if (s1.Visits.Length > 3)
                                {
                                    if (objective.SimulateSwap(problem, solution, t1, t2, s1Rev.Value, s2, out localDelta) &&
                                        localDelta > bestDelta)
                                    { // exchange succeeded.
                                        bestDelta = localDelta;
                                        bestT1 = t1;
                                        bestT2 = t2;
                                        bestS1 = s1Rev.Value;
                                        bestS2 = s2;
                                    }
                                }

                                // reverse s2.
                                if (s2.Visits.Length <= 3)
                                { // not need to check reverse for single-visit sequences.
                                    continue;
                                }
                                var s2Rev = objective.Reverse(problem, s2);

                                // try exchanging with s2 reversed.
                                if (objective.SimulateSwap(problem, solution, t1, t2, s1, s2Rev, out localDelta) &&
                                    localDelta > bestDelta)
                                { // exchange succeeded.
                                    bestDelta = localDelta;
                                    bestT1 = t1;
                                    bestT2 = t2;
                                    bestS1 = s1;
                                    bestS2 = s2Rev;
                                }

                                // try exchanging with both reversed.
                                if (s1.Visits.Length > 3)
                                {
                                    if (objective.SimulateSwap(problem, solution, t1, t2, s1Rev.Value, s2Rev, out localDelta) &&
                                        localDelta > bestDelta)
                                    { // exchange succeeded.
                                        bestDelta = localDelta;
                                        bestT1 = t1;
                                        bestT2 = t2;
                                        bestS1 = s1Rev.Value;
                                        bestS2 = s2Rev;
                                    }
                                }
                            }
                        }
                    }
                }

                if (bestDelta > 0)
                { // a swap is available.
                    return objective.TrySwap(problem, solution, bestT1, bestT2, bestS1.Value, bestS2.Value, out delta);
                }
                delta = 0;
                return false;
            }

            /// <summary>
            /// Applies this operator.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="objective">The objective.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="delta">The difference in fitness.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <returns></returns>
            public bool Apply(TProblem problem, TObjective objective, TSolution solution, int t1, int t2, out float delta)
            {
                var tour1 = solution.Tour(t1);
                var tour2 = solution.Tour(t2);

                var tour1Enumerable = objective.SeqAndSmaller(problem, tour1, _minWindowSize + 2, _maxWindowSize + 2);
                var tour2Enumerable = objective.SeqAndSmaller(problem, tour2, _minWindowSize + 2, _maxWindowSize + 2);

                if (_bestImprovement)
                { // go over all combinations and choose the best.
                    var bestDelta = 0f;
                    Seq? bestS1 = null;
                    Seq? bestS2 = null;

                    // loop over all sequences of size 4->maxWindowSize + 2. 
                    // - A minimum of 4 because otherwise we exchange just one visit.
                    // - The edge to be exchanged are also included.
                    foreach (var s1 in tour1Enumerable)
                    {
                        // switch s1.
                        Seq? s1Rev = null;
                        if (_tryReversed)
                        { // only setup reversed data if needed.
                            s1Rev = objective.Reverse(problem, s1);
                        }

                        foreach (var s2 in tour2Enumerable)
                        {
                            // try exchanging without change order.
                            if (objective.SimulateSwap(problem, solution, t1, t2, s1, s2, out float localDelta) &&
                                localDelta > bestDelta)
                            { // exchange succeeded.
                                bestDelta = localDelta;
                                bestS1 = s1;
                                bestS2 = s2;
                            }

                            if (!_tryReversed)
                            { // don't try the other options with sequences reversed.
                                continue;
                            }

                            // try exchanging with s1 reversed.
                            if (s1.Visits.Length > 3)
                            {
                                if (objective.SimulateSwap(problem, solution, t1, t2, s1Rev.Value, s2, out localDelta) &&
                                    localDelta > bestDelta)
                                { // exchange succeeded.
                                    bestDelta = localDelta;
                                    bestS1 = s1Rev.Value;
                                    bestS2 = s2;
                                }
                            }

                            // reverse s2.
                            if (s2.Visits.Length <= 3)
                            { // not need to check reverse for single-visit sequences.
                                continue;
                            }
                            var s2Rev = objective.Reverse(problem, s2);

                            // try exchanging with s2 reversed.
                            if (objective.SimulateSwap(problem, solution, t1, t2, s1, s2Rev, out localDelta) &&
                                localDelta > bestDelta)
                            { // exchange succeeded.
                                bestDelta = localDelta;
                                bestS1 = s1;
                                bestS2 = s2Rev;
                            }

                            // try exchanging with both reversed.
                            if (s1.Visits.Length > 3)
                            {
                                if (objective.SimulateSwap(problem, solution, t1, t2, s1Rev.Value, s2Rev, out localDelta) &&
                                    localDelta > bestDelta)
                                { // exchange succeeded.
                                    bestDelta = localDelta;
                                    bestS1 = s1Rev.Value;
                                    bestS2 = s2Rev;
                                }
                            }
                        }
                    }

                    if (bestDelta > 0)
                    { // a swap is available.
                        return objective.TrySwap(problem, solution, t1, t2, bestS1.Value, bestS2.Value, out delta);
                    }
                }
                else
                { // choose the first successful exchange.
                    // loop over all sequences of size 4->maxWindowSize + 2. 
                    // - A minimum of 4 because otherwise we exchange just one visit.
                    // - The edge to be exchanged are also included.
                    foreach (var s1 in tour1Enumerable)
                    {
                        // switch s1.
                        Seq? s1Rev = null;
                        if (_tryReversed)
                        { // only setup reversed data if needed.
                            s1Rev = objective.Reverse(problem, s1);
                        }

                        foreach (var s2 in tour2Enumerable)
                        {
                            // try exchanging without change order.
                            if (objective.TrySwap(problem, solution, t1, t2, s1, s2, out float localDelta))
                            { // exchange succeeded.
                                delta = localDelta;
                                return true;
                            }

                            if (!_tryReversed)
                            { // don't try the other options with sequences reversed.
                                continue;
                            }

                            // try exchanging with s1 reversed.
                            if (s1.Visits.Length > 3)
                            {
                                if (objective.TrySwap(problem, solution, t1, t2, s1Rev.Value, s2, out localDelta))
                                { // exchange succeeded.
                                    delta = localDelta;
                                    return true;
                                }
                            }

                            // reverse s2.
                            if (s2.Visits.Length <= 3)
                            { // not need to check reverse for single-visit sequences.
                                continue;
                            }
                            var s2Rev = objective.Reverse(problem, s2);

                            // try exchanging with s2 reversed.
                            if (objective.TrySwap(problem, solution, t1, t2, s1, s2Rev, out localDelta))
                            { // exchange succeeded.
                                delta = localDelta;
                                return true;
                            }

                            // try exchanging with both reversed.
                            if (s1.Visits.Length > 3)
                            {
                                if (objective.TrySwap(problem, solution, t1, t2, s1Rev.Value, s2Rev, out localDelta))
                                { // exchange succeeded.
                                    delta = localDelta;
                                    return true;
                                }
                            }
                        }
                    }
                }

                delta = 0;
                return false;
            }

            /// <summary>
            /// Returns true if the given objective is supported.
            /// </summary>
            /// <param name="objective">The objective.</param>
            /// <returns></returns>
            public bool Supports(TObjective objective)
            {
                return true;
            }
        }
}