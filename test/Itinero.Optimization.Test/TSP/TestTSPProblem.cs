using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers.GA;
using Itinero.Optimization.Tours;
using Itinero.Optimization.TSP;
using Itinero.Optimization.TSP.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itinero.Optimization.Test.TSP
{
    public class TestTSPProblem : ITSProblem
    {
        private readonly NearestNeighbourCache _nnCache;

        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private TestTSPProblem()
        {

        }

        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public TestTSPProblem(int first, float[][] weights)
        {
            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.Count = weights.Length;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first] = 0;
            }

            _nnCache = new NearestNeighbourCache(this.Weights.Length,
                (x, y) => this.Weights[x][y]);
        }

        /// <summary>
        /// Creates a new TSP, 'closed' when first equals last.
        /// </summary>
        public TestTSPProblem(int first, int last, float[][] weights)
        {
            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.Count = weights.Length;

            this.Weights[first][last] = 0;

            _nnCache = new NearestNeighbourCache(this.Weights.Length,
                (x, y) => this.Weights[x][y]);
        }

        /// <summary>
        /// Gets the weight between the two given visits.
        /// </summary>
        public float Weight(int from, int to)
        {
            return this.Weights[from][to];
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Gets the number of visits.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the nearest neighbour cache.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbourCache NearestNeighbourCache
        {
            get
            {
                return _nnCache;
            }
        }

        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            return this.Solve(new EAXSolver(GASettings.Default));
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(Itinero.Optimization.Algorithms.Solvers.ISolver<float, ITSProblem, TSPObjective, Tour, float> solver)
        {
            return solver.Solve(this, new TSPObjective());
        }
    }
}
