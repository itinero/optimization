// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

namespace Itinero.Optimization.Sequence.Directed
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
        /// <param name="closed">Also count the turn at the source and if first and last are different also the route between the two.</param>
        /// <param name="turnPenalty">The turn penalty.</param>
        /// <param name="weights">The directed weights.</param>
        public SequenceDirectedProblem(int[] sequence, bool closed, float[][] weights, float turnPenalty)
        {
            this.Weights = weights;
            this.Sequence = sequence;
            this.TurnPenalties = new float[] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };
            this.Closed = closed;
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
        public int[] Sequence { get; set; }

        /// <summary>
        /// Gets or sets the turn penalties per type of turn.
        /// 0: forward, forward.
        /// 1: forward, backward.
        /// 2: backward, forward.
        /// 3: backward, backward.
        /// </summary>
        public float[] TurnPenalties { get; set; }
    }
}