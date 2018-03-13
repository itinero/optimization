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

using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Abstract.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Directed
{
    /// <summary>
    /// An abstract base class for all TSP-TW objectives.
    /// </summary>
    public abstract class TSPTWObjectiveBase : ObjectiveBase<TSPTWProblem, Tour, float>
    {
        /// <summary>
        /// Calculates the fitness value of the given tour, assumes the tour is closed.
        /// </summary>
        public abstract float Calculate(TSPTWProblem problem, IEnumerable<int> tour);

        /// <summary>
        /// Calculates the fitness value of the given tour, assumes the tour is closed.
        /// </summary>
        public abstract float Calculate(TSPTWProblem problem, IEnumerable<int> tour, out int violated, out float violatedTime, out float waitTime, out float time,
            ref bool[] validFlags);
    }
}