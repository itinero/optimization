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
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;

namespace Itinero.Optimization.Sequences.Directed.Solver
{
    /// <summary>
    /// A heuristic to construct a decent solution to the best turns in a directed sequence.
    /// </summary>
    public sealed class ConstructionSolver : SolverBase<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "CONST"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override Tour Solve(SequenceDirectedProblem problem, SequenceDirectedObjective objective, out float fitness)
        {
            var directedTour = new int[problem.Sequence.Count];
            var i = 0;
            foreach(var c in problem.Sequence)
            {
                directedTour[i] = DirectedHelper.BuildDirectedId(c,
                    RandomGeneratorExtensions.GetRandom().Generate(4));
                i++;
            }

            Tour tour = null;
            if (problem.Sequence.First == problem.Sequence.Last)
            {
                tour = new Tour(directedTour, directedTour[0]);
            }
            else
            {
                tour = new Tour(directedTour, directedTour[directedTour.Length - 1]);
            }
            fitness = objective.Calculate(problem, tour);

            return tour;
        }
    }
}