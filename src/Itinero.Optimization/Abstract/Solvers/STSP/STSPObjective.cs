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
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Solutions.STSP
{
    /// <summary>
    /// The default TSP objective.
    /// </summary>
    public sealed class STSPObjective : ObjectiveBase<STSProblem, Tour, STSPFitness>
    {
        /// <summary>
        /// Gets the value that represents infinity.
        /// </summary>
        public sealed override STSPFitness Infinite
        {
            get
            {
                return new STSPFitness()
                {
                    Customers = int.MaxValue,
                    Weight = float.MaxValue
                };
            }
        }

        /// <summary>
        /// Returns true if the object is non-linear.
        /// </summary>
        public sealed override bool IsNonContinuous
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public sealed override string Name
        {
            get
            {
                return "STSP";
            }
        }

        /// <summary>
        /// Gets the value that represents 0.
        /// </summary>
        public sealed override STSPFitness Zero
        {
            get
            {
                return new STSPFitness()
                {
                    Customers = 0,
                    Weight = 0
                };
            }
        }

        /// <summary>
        /// Adds the two given fitness values.
        /// </summary>
        public sealed override STSPFitness Add(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            return new STSPFitness()
            {
                Customers = fitness1.Customers + fitness2.Customers,
                Weight = fitness1.Weight + fitness2.Weight
            };
        }

        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public sealed override STSPFitness Calculate(STSProblem problem, Tour solution)
        {
            // TODO: unittest this stuff!
            var fitness = new STSPFitness()
            {
                Customers = solution.Count,
                Weight = 0
            };
            var weights = problem.Weights;
            foreach (var pair in solution.Pairs())
            {
                fitness.Weight = fitness.Weight + weights[pair.From][pair.To];
            }
            return fitness;
        }

        /// <summary>
        /// Compares the two fitness values.
        /// </summary>
        public sealed override int CompareTo(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            if (fitness1.Customers == fitness2.Customers)
            {
                return fitness1.Weight.CompareTo(fitness2.Weight);
            }
            return fitness1.Customers.CompareTo(fitness2.Customers);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public sealed override bool IsZero(STSProblem problem, STSPFitness fitness)
        {
            return fitness.Weight == 0 &&
                fitness.Customers == 0;
        }

        /// <summary>
        /// Subtracts the given fitness values.
        /// </summary>
        public sealed override STSPFitness Subtract(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            return new STSPFitness()
            {
                Customers = fitness1.Customers - fitness2.Customers,
                Weight = fitness1.Weight - fitness2.Weight
            };
        }
    }
}