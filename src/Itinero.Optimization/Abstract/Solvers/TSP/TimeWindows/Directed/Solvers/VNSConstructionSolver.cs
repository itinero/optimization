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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.VNS;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Solutions.TSP.TimeWindows.Directed.Solvers.Operators;

namespace Itinero.Optimization.Solutions.TSP.TimeWindows.Directed.Solvers
{
    /// <summary>
    /// Implements a VNS-strategy to construct feasible solution for the TSP-TW from random tours.
    /// </summary>
    public class VNSConstructionSolver : IterativeSolver<float, TSPTWProblem, TSPTWFeasibleObjective, Tour, float>
    {
        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver()
            : this(10)
        {

        }

        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver(int maxIterations)
            : this(maxIterations, 1000)
        {

        }

        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver(int maxIterations, int levelMax)
            : base(new VNSSolver<float, TSPTWProblem, TSPTWFeasibleObjective, Tour, float>(new RandomSolver<TSPTWFeasibleObjective>(), new Random1Shift<TSPTWFeasibleObjective>(),
                new Local1Shift<TSPTWFeasibleObjective>(), (i, l, p, o, r) =>
                {
                    if (l > levelMax)
                    {
                        return true;
                    }
                    return o.Calculate(p, r) == 0;
                }), maxIterations, (i, p, o, r) =>
                {
                    return o.Calculate(p, r) == 0;
                })
        {

        }
    }
}