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

using System;
using Itinero.Optimization.Algorithms;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization
{
    /// <summary>
    /// Contains functional extension method to make it easier to chain solvers/operators together.
    /// </summary>
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Adds an iterator around the solver and optionally adds extra operators.
        /// </summary>
        /// <param name="createSolver">Function to create solver to iterate.</param>
        /// <param name="n">The # of times to iterate.</param>
        /// <param name="operators">The operators to apply.</param>
        /// <returns></returns>
        public static Func<TProblem, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>> Iterate<TWeight, TProblem, TObjective, TSolution, TFitness>(
            this Func<TProblem, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>> createSolver, int n,
                params IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[] operators)
            where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
            where TWeight : struct
            where TSolution : ISolution
        {
            return (p) =>
            {
                var generator = createSolver(p);
                return new IterativeSolver<TWeight, TProblem, TObjective, TSolution, TFitness>(generator, n, operators);
            };
        }
        
        /// <summary>
        /// Applies the given operators.
        /// </summary>
        /// <param name="createSolver">Function to create solver to iterate.</param>
        /// <param name="operators">The operators to apply.</param>
        /// <returns></returns>
        public static Func<TProblem, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>> Apply<TWeight, TProblem, TObjective, TSolution, TFitness>(
            this Func<TProblem, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>> createSolver,
                params IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[] operators)
            where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
            where TWeight : struct
            where TSolution : ISolution
        {
            return createSolver.Iterate(1, operators);
        }

        /// <summary>
        /// Expands the operator by looping over it n-times.
        /// </summary>
        public static IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> Iterate<TWeight, TProblem, TObjective, TSolution, TFitness>(
            this IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> op, int n)
            where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
            where TWeight : struct
            where TSolution : ISolution
        {
            return new Itinero.Optimization.Algorithms.Solvers.IterativeOperator<TWeight, TProblem, TObjective, TSolution, TFitness>(op, n);
        }

        /// <summary>
        /// Concatenates all the given operators into one new one.
        /// </summary>
        public static IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> Concat<TWeight, TProblem, TObjective, TSolution, TFitness>(
            this IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> op, 
                params IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[] operators)
            where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
            where TWeight : struct
            where TSolution : ISolution
        {
            var allOperators = new IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[operators.Length + 1];
            allOperators[0] = op;
            operators.CopyTo(allOperators, 1);
            return new Itinero.Optimization.Algorithms.Solvers.MultiOperator<TWeight, TProblem, TObjective, TSolution, TFitness>(allOperators);
        }
    }
}