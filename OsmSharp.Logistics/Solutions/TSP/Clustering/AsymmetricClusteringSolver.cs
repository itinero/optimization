// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solvers;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP.Clustering
{
    /// <summary>
    /// A solver that uses the clustering algorithm to pre-process the weights and post-processes the route afterwards.
    /// </summary>
    public class AsymmetricClusteringSolver : SolverBase<ITSP, ITSPObjective, IRoute>
    {
        private readonly SolverBase<ITSP, ITSPObjective, IRoute> _solver;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public AsymmetricClusteringSolver(SolverBase<ITSP, ITSPObjective, IRoute> solver)
        {
            _solver = solver;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return string.Format("CLUST({0})", _solver.Name); }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP problem, ITSPObjective objective, out double fitness)
        {
            // do the clustering.
            var clustering = new AsymmetricClustering(problem.Weights);
            clustering.Run();

            // define the new problem.
            var clusteredProblem = new TSPProblem(problem.First, clustering.Weights);

            // execute the solver.
            var clusteredRoute = _solver.Solve(clusteredProblem, objective);

            // 'uncluster' the route.
            var unclusteredRouteList = new List<int>(problem.Weights.Length);
            foreach(var clusteredCustomer in clusteredRoute)
            {
                unclusteredRouteList.AddRange(clustering.Cluster[clusteredCustomer]);
            }
            var unclusteredRoute = new Route(unclusteredRouteList);

            // execute a local search to optimize the clusters.
            double delta;
            var localSearch = new LocalSearch.Local1Shift();
            while (localSearch.Apply(problem, objective, unclusteredRoute, out delta)) { }

            // calculate fitness.
            fitness = objective.Calculate(problem, unclusteredRoute);
            return unclusteredRoute;
        }
    }
}