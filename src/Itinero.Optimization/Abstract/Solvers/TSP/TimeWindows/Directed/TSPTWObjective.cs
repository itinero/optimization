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
using System;
using Itinero.Optimization.Algorithms.Directed;

namespace Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Directed
{
    /// <summary>
    /// The TSP-TW objective.
    /// </summary>
    public class TSPTWObjective : TSPTWObjectiveBase
    {
        /// <summary>
        /// Gets the value that represents infinity.
        /// </summary>
        public sealed override float Infinite
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Returns true if the object is non-linear.
        /// </summary>
        public override bool IsNonContinuous
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TSP-TW";
            }
        }

        /// <summary>
        /// Gets the value that represents 0.
        /// </summary>
        public sealed override float Zero
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds the two given fitness values.
        /// </summary>
        public sealed override float Add(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        private bool[] _validFlags = null;
        
        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public sealed override float Calculate(TSPTWProblem problem, Tour solution)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[solution.Count];
            }

            // calculate everything here.
            float violatedTime, waitTime, time;
            var violated = problem.TimeAndViolations(solution, out time, out waitTime, out violatedTime, ref _validFlags);

            // TODO:there is need to seperate the violations, waiting time and weights.
            //      expand the logistics library to allow for pareto-optimal or other 
            //      more advanced weight comparisons.
            //      => this may not be needed anymore for the TSP-TW but for other problems it may be useful.
            return time + violatedTime * 1000;
        }

        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public sealed override float Calculate(TSPTWProblem problem, IEnumerable<int> solution)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            // calculate everything here.
            float violatedTime, waitTime, time;
            var violated = problem.TimeAndViolations(solution, out time, out waitTime, out violatedTime, ref _validFlags);

            // TODO:there is need to seperate the violations, waiting time and weights.
            //      expand the logistics library to allow for pareto-optimal or other 
            //      more advanced weight comparisons.
            //      => this may not be needed anymore for the TSP-TW but for other problems it may be useful.
            return time + violatedTime * 1000;
        }

        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public override float Calculate(TSPTWProblem problem, IEnumerable<int> tour, out int violated, out float violatedTime, out float waitTime, out float time, 
            ref bool[] validFlags)
        {
            // calculate everything here.
            violated = problem.TimeAndViolations(tour, out time, out waitTime, out violatedTime, ref validFlags);

            // TODO:there is need to seperate the violations, waiting time and weights.
            //      expand the logistics library to allow for pareto-optimal or other 
            //      more advanced weight comparisons.
            //      => this may not be needed anymore for the TSP-TW but for other problems it may be useful.
            return time + violatedTime * 1000;
        }

        /// <summary>
        /// Calculates the time for a part of the tour including the turns at first and last position.
        /// </summary>
        public float CalculateTimeForPart(TSPTWProblem problem, IEnumerable<int> part, float timeBefore, out int violated, out float violatedTime, out float waitTime)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            return this.CalculateTimeForPart(problem, part, timeBefore, out violated, out violatedTime, out waitTime, ref _validFlags);
        }

        /// <summary>
        /// Calculates the time for a part of the tour including the turns at first and last position.
        /// </summary>
        public float CalculateTimeForPart(TSPTWProblem problem, IEnumerable<int> part, float timeBefore, out int violated, out float violatedTime, out float waitTime, 
            ref bool[] validFlags)
        {
            // calculate everything here.
            float time;
            violated = problem.TimeAndViolationsForPart(part, timeBefore, out time, out waitTime, out violatedTime, ref validFlags);
            return time;
        }

        /// <summary>
        /// Compares the two fitness values.
        /// </summary>
        public sealed override int CompareTo(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public sealed override bool IsZero(TSPTWProblem problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Subtracts the given fitness values.
        /// </summary>
        public sealed override float Subtract(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }
    }
}
