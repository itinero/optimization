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

using Itinero.Optimization.Abstract.Solvers.VRP.Operators.CrossOver;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.GA;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GA
{
    /// <summary>
    /// A solver based on a genetic algorithm strategy.
    /// </summary>
    /// <typeparam name="TProblem"></typeparam>
    /// <typeparam name="TObjective"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    public class GASolver<TProblem, TObjective, TSolution> : SolverBase<float, TProblem, TObjective, TSolution, float>
        where TSolution : class
        where TObjective : ObjectiveBase<TProblem, TSolution, float>, IGAObjective<TProblem, TSolution>
    {
        private readonly ISolver<float, TProblem, TObjective, TSolution, float> _generator;
        private readonly IOperator<float, TProblem, TObjective, TSolution, float> _mutation;
        private readonly GASettings _settings;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="mutation">The mutation operator.</param>
        /// <param name="settings">The settings.</param>
        public GASolver(ISolver<float, TProblem, TObjective, TSolution, float> generator, IOperator<float, TProblem, TObjective, TSolution, float> mutation,
            GASettings settings)
        {
            _generator = generator;
            _mutation = mutation;
            _settings = settings;
        }

        /// <inheritdoc />
        public override string Name => $"GA_{_generator.Name}_{_mutation.Name}";

        /// <inheritdoc />
        public override TSolution Solve(TProblem problem, TObjective objective, out float fitness)
        {
            var crossOver = new TourExchangeCrossOverOperator<TObjective, TProblem, TSolution>();
            var selection = new TournamentSelectionOperator<TProblem, TSolution, TObjective, float>(50, 0.75);
            
            var gaSolver = new Itinero.Optimization.Algorithms.Solvers.GA.GASolver<float, TProblem, TObjective, TSolution, float>(objective, _generator, crossOver, 
                selection, _mutation, _settings);
            gaSolver.IntermidiateResult += (res) =>
              {
                  this.ReportIntermidiateResult(res);
              };
            return gaSolver.Solve(problem, objective, out fitness);
        }
    }
}