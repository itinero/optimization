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
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Itinero.Optimization.Strategies.GA
{
    /// <summary>
    /// A genetic algorithm.
    /// </summary>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public class GAStrategy<TProblem, TCandidate> : Strategy<TProblem, TCandidate>
        where TCandidate : class
    {
        private readonly Strategy<TProblem, TCandidate> _generator;
        private readonly Operator<TCandidate> _mutation;
        private readonly CrossOverOperator<TCandidate> _crossOver;
        private readonly ISelector<TCandidate> _selector;
        private readonly GASettings _settings;

        /// <summary>
        /// Creates a new genetic algorithm.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="crossOver">The crossover operator.</param>
        /// <param name="mutation">The mutation operator.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="settings">The settings.</param>
        public GAStrategy(Func<TProblem, TCandidate> generator, Func<TCandidate, TCandidate, TCandidate> crossOver,
            Func<TCandidate, bool> mutation, GASettings settings,
            ISelector<TCandidate> selector = null)
            : this(generator.ToStrategy(), crossOver, mutation, settings, selector)
        {
            
        }

        /// <summary>
        /// Creates a new genetic algorithm.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="crossOver">The crossover operator.</param>
        /// <param name="mutation">The mutation operator.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="settings">The settings.</param>
        public GAStrategy(Strategy<TProblem, TCandidate> generator, CrossOverOperator<TCandidate> crossOver,
            Operator<TCandidate> mutation, GASettings settings,
            ISelector<TCandidate> selector = null)
        {
            _crossOver = crossOver;
            _generator = generator;
            _mutation = mutation;
            _selector = selector ?? new TournamentSelector<TCandidate>();

            _settings = settings;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name => $"GA_[{_generator.Name}_{_mutation.Name}_{_crossOver.Name}_{_selector.Name}]";

        /// <summary>
        /// Runs this strategy on the given problem and returns the best candidate.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A candidate.</returns>
        public override TCandidate Search(TProblem problem)
        {
            var population = new TCandidate[_settings.PopulationSize];
            
            // generate initial population.
            var solutionCount = 0;
            while (solutionCount < _settings.PopulationSize)
            {
                population[solutionCount] = _generator.Search(problem);
                solutionCount++;
            }
            
            // sort population & determine the best candidate.
            Array.Sort(population);
            var best = population[0];

            // do the mutation/crossover loop until stopping conditions are met.
            var stagnation = 0;
            var generation = 0;
            var elitism = (int)(_settings.PopulationSize * (_settings.ElitismPercentage / 100.0));
            var crossOver = (int)(_settings.PopulationSize * (_settings.CrossOverPercentage / 100.0));
            var crossOverIndividuals = new TCandidate[crossOver];
            var exclude = new HashSet<int>();
            while (stagnation < _settings.StagnationCount &&
                   generation < _settings.MaxGenerations)
            {
                // select individuals for crossover.
                exclude.Clear();
                for (var i = 0; i < crossOver; i++)
                { // select individual.
                    var selected = -1;
                    while (selected < 0)
                    {
                        selected = _selector.Select(population, (c) => exclude.Contains(c));
                    }
                    crossOverIndividuals[i] = population[selected];
                    exclude.Add(selected);
                }

                // replace part of the population by offspring.
                for (var i = elitism; i < population.Length; i++)
                {
                    Random.RandomGenerator.Generate2(crossOver, out var c1, out var c2);
                    population[i] = _crossOver.Apply(population[c1], population[c2]);
                }

                // mutate part of the population.
                for (var i = elitism; i < population.Length; i++)
                {
                    if (Random.RandomGenerator.Default.Generate(100) > _settings.MutationPercentage) continue; 
                    
                    // ok, mutate this individual.
                    _mutation.Apply(population[i]); // by ref so should be fine.
                }
                
                // again, sort things and check for the best.
                Array.Sort(population);
                if (CandidateComparison.Compare(best, population[0]) > 0)
                { // the new candidate is better, yay!
                    stagnation = 0;
                    best = population[0];
                    this.ReportIntermidiateResult(best);
                }
                else
                { // oeps, failed attempt.
                    stagnation++;
                }

                generation++;
            }

            return best;
        }
    }
}