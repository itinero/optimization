/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a solution to a capacitated no-depot VRP.
    /// </summary>
    public class NoDepotCVRPSolution : ISolution, IRelocateSolution, IExchangeSolution, IMultiExchangeSolution, IMultiRelocateSolution,
        ISeededCheapestInsertionSolution
    {
        private readonly List<CapacityExtensions.Content> _contents;
        private readonly List<ITour> _tours;

        /// The visit-ID _after_ which the depot is visited
        private readonly List<int> _depotPoint;

        /// The cost of visiting the depot, at this location
        private readonly List<int> _depotCost;

        /// <summary>
        /// Creates a new solution.
        /// </summary>
        public NoDepotCVRPSolution(int size)
        {
            _tours = new List<ITour>(size);
            _contents = new List<CapacityExtensions.Content>(size);
            _depotCost = new List<int>(size);
            _depotPoint = new List<int>(size);

        }

        /// <summary>
        /// Creates a new solution by deep-copying what's given.
        /// </summary>
        protected NoDepotCVRPSolution(NoDepotCVRPSolution toCopy)
            : this(toCopy._contents.Count)
        {

            // make a deep-copy of the contents.
            List<CapacityExtensions.Content> contents = toCopy._contents;
            for (var c = 0; c < contents.Count; c++)
            {
                _contents.Add(new CapacityExtensions.Content()
                {
                    Weight = contents[c].Weight
                });
                if (contents[c].Quantities != null)
                {
                    _contents[c].Quantities = contents[c].Quantities.Clone() as float[];
                }
            }

            // make a deep-copy of the tours.
            foreach (var tour in toCopy._tours)
            {
                _tours.Add(tour.Clone() as Tour);
            }

            _depotPoint.AddRange(toCopy._depotPoint);
            _depotCost.AddRange(toCopy._depotCost);

        }


        /// <summary>
        /// Calculates the weight of the given tour.
        /// </summary>
        public float CalculateWeightOf(NoDepotCVRProblem problem, int tourIdx)
        {
            var weight = 0f;
            var tour = this.Tour(tourIdx);

            Pair? last = null;
            foreach (var pair in tour.Pairs())
            {
                weight += problem.GetVisitCost(pair.From);
                weight += problem.Weights[pair.From][pair.To];
            }
            if (last.HasValue && !tour.IsClosed())
            {
                weight += problem.GetVisitCost(last.Value.To);
            }

            return weight;
        }








        /// <summary>
        /// Gets or sets the contents of each tour.
        /// </summary>
        public List<CapacityExtensions.Content> Contents
        {
            get
            {
                return _contents;
            }
        }

        /// <summary>
        /// Gets the number of tours.
        /// </summary>
        public int Count
        {
            get
            {
                return _tours.Count;
            }
        }


        /// <summary>
        /// Gets the tour at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ITour Tour(int i)
        {
            return _tours[i];
        }

        /// <summary>
        /// Replaces the given tour at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="tour"></param>
        public void ReplaceTour(int i, ITour tour)
        {
            _tours[i] = tour;
        }

        /// <summary>
        /// Adds a new tour.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public ITour Add(int first, int? last)
        {
            var tour = new Tour(new int[] { first }, last);
            _tours.Add(tour);
            return tour;
        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new NoDepotCVRPSolution(this);
        }

        /// <summary>
        /// Overwrites what's in this solution by what's in the given solution.
        /// </summary>
        /// <param name="solution"></param>
        public void CopyFrom(ISolution solution)
        {
            throw new NotImplementedException();
        }
    }
}