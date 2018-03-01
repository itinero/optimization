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

using Itinero.Optimization.Tours;
using Itinero.Optimization.Models.TimeWindows;

namespace Itinero.Optimization.Solutions.TSP.TimeWindows
{
    /// <summary>
    /// The default TSP-TW profile definition.
    /// </summary>
    public sealed class TSPTWProblem
    {
        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private TSPTWProblem()
        {

        }

        /// <summary>
        /// Creates a new 'open' TSP-TW with only a start customer.
        /// </summary>
        public TSPTWProblem(int first, float[][] times, TimeWindow[] windows)
        {
            this.First = first;
            this.Last = null;
            this.Times = times;
            this.Windows = windows;

            for (var x = 0; x < this.Times.Length; x++)
            {
                this.Times[x][first] = 0;
            }
        }

        /// <summary>
        /// Creates a new TSP-TW, 'closed' when first equals last.
        /// </summary>
        public TSPTWProblem(int first, int last, float[][] weights, TimeWindow[] windows)
        {
            this.First = first;
            this.Last = last;
            this.Times = weights;
            this.Windows = windows;

            this.Times[first][last] = 0;
        }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        public TimeWindow[] Windows { get; set; }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Times { get; set; }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public TSPTWProblem ToClosed()
        {
            if (this.Last == null)
            { // 'open' problem, just set weights to first to 0.
                // REMARK: weights already set in constructor.
                return new TSPTWProblem(this.First, this.First, this.Times, this.Windows);
            }
            else if (this.First != this.Last)
            { // 'open' problem but with fixed weights.
                var times = new float[this.Times.Length - 1][];
                for (var x = 0; x < this.Times.Length; x++)
                {
                    if (x == this.Last)
                    { // skip last edge.
                        continue;
                    }
                    var xNew = x;
                    if (x > this.Last)
                    { // decrease new index.
                        xNew = xNew - 1;
                    }

                    times[xNew] = new float[this.Times[x].Length - 1];

                    for (var y = 0; y < this.Times[x].Length; y++)
                    {
                        if (y == this.Last)
                        { // skip last edge.
                            continue;
                        }
                        var yNew = y;
                        if (y > this.Last)
                        { // decrease new index.
                            yNew = yNew - 1;
                        }

                        if (yNew == xNew)
                        { // make not sense to keep values other than '0' and to make things easier to understand just use '0'.
                            times[xNew][yNew] = 0;
                        }
                        else if (y == this.First)
                        { // replace -> first with -> last.
                            times[xNew][yNew] = this.Times[x][this.Last.Value];
                        }
                        else
                        { // nothing special about this connection, yay!
                            times[xNew][yNew] = this.Times[x][y];
                        }
                    }
                }
                return new TSPTWProblem(this.First, this.First, times, this.Windows);
            }
            return this; // problem already closed with first==last.
        }
        
        /// <summary>
        /// Creates a deep-copy of this problem.
        /// </summary>
        /// <returns></returns>
        public TSPTWProblem Clone()
        {
            var weights = new float[this.Times.Length][];
            for (var i = 0; i < this.Times.Length; i++)
            {
                weights[i] = this.Times[i].Clone() as float[];
            }
            var clone = new TSPTWProblem();
            clone.First = this.First;
            clone.Last = this.Last;
            clone.Times = this.Times;
            clone.Windows = this.Windows;
            return clone;
        }

        /// <summary>
        /// Solves this problim using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            return this.Solve(new Solvers.VNSSolver());
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(Itinero.Optimization.Algorithms.Solvers.ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> solver)
        {
            return solver.Solve(this, new TSPTWObjective());
        }
    }
}
