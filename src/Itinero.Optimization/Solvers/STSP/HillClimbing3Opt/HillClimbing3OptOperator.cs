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
using Itinero.Optimization.Solvers.Shared.HillClimbing3Opt;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.STSP.HillClimbing3Opt
{
    /// <summary>
    /// A 3-opt operator.
    /// </summary>
    /// <remarks>
    /// For best performance:
    /// - Make sure nearest neighbours are cached.
    /// - Use don't look bits.
    /// </remarks>
    internal sealed class HillClimbing3OptOperator : Operator<Candidate<STSProblem, Tour>>
    {
        private readonly bool _nearestNeighbours = false;
        private readonly bool _dontLook = false;

        public HillClimbing3OptOperator(bool nearestNeighbours = true, bool dontLook = true)
        {
            _dontLook = dontLook;
            _nearestNeighbours = nearestNeighbours;
        }

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
        
        public override bool Apply(Candidate<STSProblem, Tour> candidate)
        {
            var problem = candidate.Problem;
            
            // improve by running the 3-Opt step.
            var nearestNeighbours = _nearestNeighbours ? null : problem.NearestNeighbourCache.GetNNearestNeighboursForward(10);
            var res = candidate.Solution.Do3Opt(problem.Weight, problem.WeightsSize, nearestNeighbours, _dontLook);
            if (!res.improved) return false;
            candidate.Fitness = candidate.Solution.Fitness(problem);
            return true;
        }
        
        private static readonly ThreadLocal<HillClimbing3OptOperator> DefaultLazy = new ThreadLocal<HillClimbing3OptOperator>(() => new HillClimbing3OptOperator());
        public static HillClimbing3OptOperator Default => DefaultLazy.Value;
    }
}