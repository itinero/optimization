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
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.STSP
{
    internal class RandomSolver : Strategy<STSProblem, STSPCandidate>
    {
        public override string Name => "RAN";

        private RandomPool _randomPool = null;

        public override STSPCandidate Search(STSProblem problem)
        {
            // generate random pool to select visits from.
            if (_randomPool == null || _randomPool.Size < problem.WeightsSize)
            {
                _randomPool = new RandomPool(problem.WeightsSize);
            }
            else
            {
                _randomPool.Reset();
            }

            // keep adding customers until no more space is left or no more customers available.
            var tour = problem.CreateEmptyTour();
            var fitness = 0f;
            while (_randomPool.MoveNext())
            {
                var visit = _randomPool.Current;
                if (visit == problem.First ||
                    visit == problem.Last)
                {
                    // these are already placed.
                    continue;
                }

                if (!problem.Contains(visit))
                {
                    // it's possible the random pool contains uneeded visits.
                    continue;
                }

                var result = tour.CalculateCheapest(problem.Weight, visit);
                var cost = result.cost;
                var location = result.location;
                if (!(cost + fitness <= problem.Max)) continue;
                tour.InsertAfter(location.From, visit);
                fitness += cost;
            }

            return new STSPCandidate()
            {
                Fitness = fitness,
                Problem = problem,
                Solution = tour
            };
        }
        
        private static readonly ThreadLocal<RandomSolver> DefaultLazy = new ThreadLocal<RandomSolver>(() => new RandomSolver());
        public static RandomSolver Default => DefaultLazy.Value;
    }
}