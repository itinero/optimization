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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Operations;
using System;

namespace Itinero.Optimization.General.Operators
{
    /// <summary>
    /// An operator to execute n random 1-shift* relocations.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public sealed class GeneralRandom1Shift<TProblem, TObjective> : IPerturber<float, TProblem, TObjective, Tour, float>
    {
        private readonly RandomGenerator _random = RandomGeneratorExtensions.GetRandom();
        private readonly Func<TProblem, float[][]> _getWeights;

        /// <summary>
        /// Creates a new general random 1 shift operator.
        /// </summary>
        public GeneralRandom1Shift(Func<TProblem, float[][]> getWeights)
        {
            _getWeights = getWeights;
        }

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "RAN_1SHFT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, Tour tour, out float difference)
        {
            return this.Apply(problem, objective, tour, 1, out difference);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, Tour tour, int level, out float difference)
        {
            var weights = _getWeights(problem);

            difference = 0;
            while (level > 0)
            {
                // remove random customer after another random customer.
                var customer = _random.Generate(weights.Length);
                var insert = _random.Generate(weights.Length - 1);
                if (insert >= customer)
                { // customer is the same of after.
                    insert++;
                }

                float shiftDiff;
                if (!ShiftAfter.Do(weights, tour, customer, insert, out shiftDiff))
                {
                    throw new Exception(
                        string.Format("Failed to shift customer {0} after {1} in route {2}.", customer, insert, tour.ToInvariantString()));
                }
                difference += shiftDiff;

                // decrease level.
                level--;
            }
            return difference < 0;
        }
    }
}