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
using Itinero.Optimization.Sequences.Directed.Solver;
using Itinero.Optimization.Sequences.Directed.Solver.Operators;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Sequences.Directed
{
    /// <summary>
    /// Defines a directed sequence problem.
    /// </summary>
    public class SequenceDirectedProblem
    {
        /// <summary>
        /// Creates a new problem.
        /// </summary>
        /// <param name="sequence">The sequence to optimize.</param>
        /// <param name="turnPenalty">The turn penalty.</param>
        /// <param name="weights">The directed weights.</param>
        public SequenceDirectedProblem(Tour sequence, float[][] weights, float turnPenalty)
        {
            if (sequence.Last == null)
            {
                throw new System.Exception("Sequence directed problem can only apply to tours with a fixed end customer.");
            }

            this.Weights = weights;
            this.Sequence = sequence;
            this.TurnPenalties = new float[] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };
        }

        /// <summary>
        /// Gets the closed flag.
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        public Tour Sequence { get; set; }

        /// <summary>
        /// Gets or sets the turn penalties per type of turn.
        /// 0: forward, forward.
        /// 1: forward, backward.
        /// 2: backward, forward.
        /// 3: backward, backward.
        /// </summary>
        public float[] TurnPenalties { get; set; }
        
        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            return this.Solve(new IterativeSolver<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float>(
                new ConstructionSolver(), 10, new DirectionLocalSearchOperator()));
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(Itinero.Optimization.Algorithms.Solvers.ISolver<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float> solver)
        {
            return solver.Solve(this, new SequenceDirectedObjective());
        }
    }
}