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
    /// A VNS-solver for the TSP-TW problem.
    /// </summary>
    public class VNSSolver : VNSSolver<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Creates a new VNS-solver.
        /// </summary>
        public VNSSolver()
            : base(new SolverObjectiveWrapper<float, TSPTWProblem, TSPTWObjective, TSPTWFeasibleObjective, Tour, float>(
                new VNSConstructionSolver(), new TSPTWFeasibleObjective(), (p, o, s) => o.Calculate(p, s)), new Random1Shift<TSPTWObjective>(),
                    new VNDOperator(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}