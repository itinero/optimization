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
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a solution to a capacitated no-depot VRP.
    /// </summary>
    public class NoDepotCVRPSolution : ISolution, IRelocateSolution, IExchangeSolution, IMultiExchangeSolution, IMultiRelocateSolution,
        ISeededCheapestInsertionSolution, IGuidedVNSSolution
    {
        private readonly List<CapacityExtensions.Content> _contents;
        private readonly List<ITour> _tours; // does not use multitour, as some problems require the same depot to be visited multiple times
        

        /// <summary>
        /// Creates a new solution.
        /// </summary>
        public NoDepotCVRPSolution(int size)
        {
            _tours = new List<ITour>(size);
            _contents = new List<CapacityExtensions.Content>(size);
        }

        /// <summary>
        /// Creates a new solution by deep-copying what's given.
        /// </summary>
        protected NoDepotCVRPSolution(List<ITour> tours, List<CapacityExtensions.Content> contents)
        {
            // make a deep-copy of the contents.
            _contents = new List<CapacityExtensions.Content>(contents.Count);
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
            _tours = new List<ITour>();
            foreach (var tour in tours)
            {
                _tours.Add(tour.Clone() as Tour);
            }
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
        /// Gets the # of tours.
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
            var tour = new Tour(new int[] { first} , last);
            _tours.Add(tour);
            return tour;
        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new NoDepotCVRPSolution(_tours, _contents);
        }

        /// <summary>
        /// Overwrites what's in this solution by what's in the given solution.
        /// </summary>
        /// <param name="solution"></param>
        public void CopyFrom(ISolution solution)
        {
            var other = (solution as NoDepotCVRPSolution);
            if (other == null)
            {
                throw new Exception("Can only copy from another NoDepot CVRP solution.");
            }
            
            // make a deep-copy of the contents.
            _contents.Clear();
            for (var c = 0; c < other._contents.Count; c++)
            {
                _contents.Add(new CapacityExtensions.Content()
                {
                    Weight = other._contents[c].Weight
                });
                if (other._contents[c].Quantities != null)
                {
                    _contents[c].Quantities = other._contents[c].Quantities.Clone() as float[];
                }
            }

            // make a deep-copy of the tours.
            _tours.Clear();
            foreach (var tour in other._tours)
            {
                _tours.Add(tour.Clone() as Tour);
            }
        }

        /// <summary>
        /// Removes the given tour.
        /// </summary>
        /// <param name="i"></param>
        public void RemoveTour(int i)
        {
            _tours.RemoveAt(i);
            _contents.RemoveAt(i);
        }
    }
}