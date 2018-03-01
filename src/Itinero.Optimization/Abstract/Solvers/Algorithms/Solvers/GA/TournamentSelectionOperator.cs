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
using Itinero.Optimization.Algorithms.Solvers.Objective;
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.Solvers.GA
{
    /// <summary>
    /// A selector selecting individials using a tournament base selection.
    /// </summary>
    public class TournamentSelectionOperator<TProblem, TSolution, TObjective, TFitness> : ISelectionOperator<TProblem, TSolution, TObjective, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        private double _tournamentSize;
        private double _tournamentProbability;
        private readonly RandomGenerator _random = RandomGeneratorExtensions.GetRandom();

        /// <summary>
        /// Creates a new tournament base selector.
        /// </summary>
        public TournamentSelectionOperator()
            : this(10, 0.5)
        {

        }

        /// <summary>
        /// Creates a new tournament base selector.
        /// </summary>
        public TournamentSelectionOperator(
            double tournamentSize,
            double tournamentProbability)
        {
            _tournamentSize = tournamentSize;
            _tournamentProbability = tournamentProbability;

            _random = RandomGeneratorExtensions.GetRandom();
        }

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "TOURN"; }
        }

        /// <summary>
        /// Selects a new solution for reproduction.
        /// </summary>
        /// <returns></returns>
        public int Select(TProblem problem, TObjective objective, Individual<TSolution, TFitness>[] population, System.Collections.Generic.ISet<int> exclude)
        {
            var tournamentSizeInt = (int)System.Math.Ceiling(((_tournamentSize / 100f) * (double)population.Length));
            var tempPop = new List<Tuple<int, Individual<TSolution, TFitness>>>(tournamentSizeInt);

            while (tempPop.Count < tournamentSizeInt)
            { // keep looping until enough individuals are selected or until no more are available.
                var idx = _random.Generate(population.Length);
                if (exclude == null || !exclude.Contains(idx))
                { // do not tournament excluded solutions.
                    tempPop.Add(new Tuple<int, Individual<TSolution, TFitness>>(idx, population[idx]));
                }
            }

            // sort the population..
            tempPop.Sort((x, y) =>
            {
                return objective.CompareTo(problem, y.Item2.Fitness, x.Item2.Fitness);
            });

            // choose a candidate.
            for (var idx = 0; idx < tempPop.Count; idx++)
            { // choose a candidate.
                if (_random.Generate(1.0f) < _tournamentProbability)
                { // candidate choosen!
                    return tempPop[idx].Item1;
                }
            }
            return -1;
        }
    }
}