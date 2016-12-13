using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System;

namespace Itinero.Optimization.TSP.TimeWindows.Directed.Solvers
{
    /// <summary>
    /// A 3-opt solver.
    /// </summary>
    public sealed class HillClimbing3OptSolver : SolverBase<float, TSPTWProblem, TSPTWObjective, Tour, float>, IOperator<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        private readonly ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> _generator;
        private readonly bool _nearestNeighbours = false;
        private readonly bool _dontLook = false;
        private readonly double _epsilon = 0.001f;
        private bool[] _dontLookBits;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public HillClimbing3OptSolver()
            : this(new RandomSolver<TSPTWObjective>(), true, true)
        {

        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public HillClimbing3OptSolver(ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> generator, bool nearestNeighbours, bool dontLook)
        {
            _generator = generator;
            _dontLook = dontLook;
            _nearestNeighbours = nearestNeighbours;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                if (_nearestNeighbours && _dontLook)
                {
                    return "3OHC_(NN)_(DL)";
                }
                else if (_nearestNeighbours)
                {
                    return "3OHC_(NN)";
                }
                else if (_dontLook)
                {
                    return "3OHC_(DL)";
                }
                return "3OHC";
            }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TSPTWObjective objective)
        {
            return false;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override Tour Solve(TSPTWProblem problem, TSPTWObjective objective, out float fitness)
        {
            if (!problem.Last.HasValue) { throw new ArgumentException("OPT3 cannot be used on open TSP-problems."); }

            // generate some route first.
            var route = _generator.Solve(problem, objective);

            // calculate fitness.
            fitness = objective.Calculate(problem, route);

            // improve the existing solution.
            float difference;
            if (this.Apply(problem, objective, route, out difference))
            { // improvement!
                fitness = fitness + difference;
            }

            return route;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TSPTWObjective objective, Tour solution, out float delta)
        {
            if (!problem.Last.HasValue) { throw new ArgumentException("3OPT operator cannot be used on open TSP-problems."); }
            if (solution.First != solution.Last) { throw new ArgumentException("3OPT operator cannot be used on open TSP-problems."); }
            if ((solution.First == solution.Last) != problem.Last.HasValue) { throw new ArgumentException("Route and problem have to be both closed."); }

            _dontLookBits = new bool[problem.Times.Length / 2];
            var weights = problem.Times;

            // loop over all customers.
            bool anyImprovement = false;
            bool improvement = true;
            delta = 0;
            while (improvement)
            {
                improvement = false;
                var moveDelta = 0.0f;
                foreach (int v1 in solution)
                {
                    if (!this.Check(problem, v1))
                    {
                        if (this.Try3OptMoves(problem, weights, solution, v1, out moveDelta))
                        {
                            anyImprovement = true;
                            improvement = true;
                            delta = delta + moveDelta;
                            break;
                        }
                        this.Set(problem, v1, true);
                    }
                }
            }
            return anyImprovement;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v1.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(TSPTWProblem problem, float[][] weights, Tour tour, int v1, out float delta)
        {
            // get v_2.
            var v2 = tour.GetNeigbour(v1);
            if (v2 < 0)
            {
                delta = 0;
                return false;
            }

            var betweenV2V1 = tour.Between(v2, v1);
            //var weightV1V2 = weights[v1][v2];
            var weightV1V2 = weights.WeightWithoutTurns(v1, v2);
            if (DirectedHelper.ExtractId(v2) == problem.First && !problem.Last.HasValue)
            { // set to zero if not closed.
                weightV1V2 = 0;
            }
            int v3 = -1;
            NearestNeighbours neighbours = null;
            if (_nearestNeighbours)
            {
                neighbours = problem.GetNNearestNeighboursForward(10, DirectedHelper.ExtractId(v1));
            }

            foreach (int v4 in betweenV2V1)
            {
                if (v3 >= 0 && v3 != v1)
                {
                    var v4Id = DirectedHelper.ExtractId(v4);
                    if (!_nearestNeighbours ||
                        neighbours.Contains(v4Id))
                    {
                        //var weightV1V4 = weights[v1][v4];
                        var weightV1V4 = weights.WeightWithoutTurns(v1, v4);
                        //var weightV3V4 = weights[v3][v4];
                        var weightV3V4 = weights.WeightWithoutTurns(v3, v4);
                        if (v4Id == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV1V4 = 0;
                            weightV3V4 = 0;
                        }
                        var weightV1V2PlusV3V4 = weightV1V2 + weightV3V4;
                        //var weightsV3 = weights[v3];
                        var weightsV3 = weights[DirectedHelper.ExtractDepartureId(v3)];
                        if (this.Try3OptMoves(problem, weights, tour, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4, out delta))
                        {
                            return true;
                        }
                    }
                }
                v3 = v4;
            }
            delta = 0;
            return false;
        }

        ///// <summary>
        ///// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        ///// </summary>
        ///// <returns></returns>
        //public bool Try3OptMoves(TSProblem problem, float[][] weights, Route route,
        //    int v1, int v2, float weightV1V2,
        //    int v3, out float delta)
        //{
        //    // get v_4.
        //    var v4 = route.GetNeigbour(v3);
        //    var weightV1V2PlusV3V4 = weightV1V2 + weights[v3][v4];
        //    var weightV1V4 = weights[v1][v4];
        //    if (v4 == problem.First && !problem.Last.HasValue)
        //    { // set to zero if not closed.
        //        weightV1V4 = 0;
        //    }
        //    var weightsV3 = weights[v3];
        //    return this.Try3OptMoves(problem, weights, route, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4, out delta);
        //}

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(TSPTWProblem problem, float[][] weights, Tour tour,
            int v1, int v2, int v3, float[] weightsV3, int v4, float weightV1V2PlusV3V4, float weightV1V4, out float delta)
        {
            var v2ArrivalId = DirectedHelper.ExtractArrivalId(v2); // TODO: optimize this, extract all v2 info in one go.
            var v2Id = DirectedHelper.ExtractId(v2);
            var betweenV4V1Enumerator = tour.Between(v4, v1).GetEnumerator();
            if (betweenV4V1Enumerator.MoveNext())
            {
                var v5 = betweenV4V1Enumerator.Current;
                if (v5 != v1)
                {
                    while (betweenV4V1Enumerator.MoveNext())
                    {
                        var v6 = betweenV4V1Enumerator.Current;
                        //var weightV3V6 = weightsV3[v6];
                        var v6Id = DirectedHelper.ExtractId(v6); // TODO: optimize this, extract all v6 info in one go.
                        var v6ArrivalId = DirectedHelper.ExtractArrivalId(v6);
                        var v5DepartureId = DirectedHelper.ExtractDepartureId(v5);
                        var weightV3V6 = weightsV3[v6ArrivalId];
                        //var weightV5V2 = weights[v5][v2];
                        var weightV5V2 = weights[v5DepartureId][v2ArrivalId];
                        //var weightV5V6 = weights[v5][v6];
                        var weightV5V6 = weights[v5DepartureId][v6ArrivalId];
                        if (v6Id == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV3V6 = 0;
                            weightV5V6 = 0;
                        }
                        if (v2Id == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV5V2 = 0;
                        }

                        // calculate the total weight of the 'new' arcs.
                        var weightNew = weightV1V4 + weightV3V6 + weightV5V2;

                        // calculate the total weights.
                        var weight = weightV1V2PlusV3V4 + weightV5V6;

                        if (weight - weightNew > _epsilon)
                        { // actually do replace the vertices.
                            var countBefore = tour.Count;
                            delta = weightNew - weight;

                            tour.ReplaceEdgeFrom(v1, v4);
                            tour.ReplaceEdgeFrom(v3, v6);
                            tour.ReplaceEdgeFrom(v5, v2);

                            //int count_after = route.Count;

                            // set bits.
                            this.Set(problem, v3, false);
                            this.Set(problem, v5, false);

                            return true; // move succeeded.
                        }
                        v5 = v6;
                    }
                }
            }
            delta = 0;
            return false;
        }

        #region Don't Look

        private bool Check(TSPTWProblem problem, int directedCustomer)
        {
            if (_dontLook)
            {
                var originalId = DirectedHelper.ExtractId(directedCustomer);
                return _dontLookBits[originalId];
            }
            return false;
        }

        private void Set(TSPTWProblem problem, int directedCustomer, bool value)
        {
            if (_dontLook)
            {
                var originalId = DirectedHelper.ExtractId(directedCustomer);
                _dontLookBits[originalId] = value;
            }
        }

        #endregion
    }
}
