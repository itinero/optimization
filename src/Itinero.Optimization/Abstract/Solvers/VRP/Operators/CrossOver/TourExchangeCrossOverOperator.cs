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

using Itinero.Attributes;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GA;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.GA;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.CrossOver
{
    /// <summary>
    /// A cross over operator that uses tours from the two given solution to build a new solution.
    /// </summary>
    /// <typeparam name="TObjective"></typeparam>
    /// <typeparam name="TProblem"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    public class TourExchangeCrossOverOperator<TObjective, TProblem, TSolution> : ICrossOverOperator<float, TProblem, TObjective, TSolution, float>
        where TObjective : ObjectiveBase<TProblem, TSolution, float>, IGAObjective<TProblem, TSolution>
    {
        private readonly IOperator<float, TProblem, TObjective, TSolution, float> _postOperator;

        /// <summary>
        /// Creates a cross over operator.
        /// </summary>
        /// <param name="postOperator">The operator to apply right after crossover.</param>
        public TourExchangeCrossOverOperator(IOperator<float, TProblem, TObjective, TSolution, float> postOperator = null)
        {
            _postOperator = postOperator;
        }
        
        /// <inheritdoc />
        public string Name { get; } = "TOUR_EX";

        /// <inheritdoc />
        public TSolution Apply(TProblem problem, TObjective objective, TSolution solution1, TSolution solution2, out float fitness)
        {
            var solution = objective.NewSolution(problem);

            // try to use as many tours as possible.
            var success = true;
            while (success)
            {
                success = false;
                
                // try to select a tour from solution1 to use in the given solution.
                success |= objective.SelectTour(problem, solution1, solution);
                
                // try to select a tour from solution2 to use in the given solution.
                success |= objective.SelectTour(problem, solution2, solution);
            }
            
            // try to place the rest of the missing visits.
            objective.PlaceRemaining(problem, solution);
            
            // apply operator.
            _postOperator?.Apply(problem, objective, solution, out var _);

            fitness = objective.Calculate(problem, solution);
            return solution;
        }
    }
}