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
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a solution to a capacitated no-depot VRP.
    /// </summary>
    public class NoDepotCVRPSolution : MultiTour
    {
        /// <summary>
        /// Creates a new solution.
        /// </summary>
        public NoDepotCVRPSolution(int size) : base(size)
        {

        }

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        protected NoDepotCVRPSolution(IEnumerable<SubTour> first, int[] nextArray)
            : base(first, nextArray)
        {

        }

        /// <summary>
        /// Gets or sets the tour weights.
        /// </summary>
        /// <returns></returns>
        public float[] Weights
        {
            get;
            set;
        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new NoDepotCVRPSolution(_subtours, _nextArray);
        }
    }
}