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

using System.Threading;
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.TSP_TW.Operators
{
    /// <summary>
    /// An operator that runs the local 2 opt operator.
    /// </summary>
    internal class Local2OptOperator : Operator<Candidate<TSPTWProblem, Tour>>
    {
        public override string Name => "2OPT";

        public override bool Apply(Candidate<TSPTWProblem, Tour> candidate)
        {
            if (!candidate.Solution.Do2Opt(candidate.Problem.Weight, candidate.Problem.Windows)) return false;
            
            candidate.Fitness = candidate.Solution.Fitness(candidate.Problem);
            return true;
        }
        
        private static readonly ThreadLocal<Local2OptOperator> DefaultLazy = new ThreadLocal<Local2OptOperator>(() => new Local2OptOperator());
        public static Local2OptOperator Default => DefaultLazy.Value;
    }
}