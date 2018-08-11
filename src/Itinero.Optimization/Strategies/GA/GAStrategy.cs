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
using System.Threading.Tasks;
using Itinero.Logging;

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
        private const bool _useParalell = false;

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
            _selector = selector ?? new TournamentSelector<TCandidate>(50, 0.8f);

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
            Logger.Log($"{nameof(GAStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                $"{this.Name}: Generating population of {_settings.PopulationSize} individuals.");
            if (_useParalell)
            {
                Parallel.For(0, _settings.PopulationSize, (i) =>
                {
                    population[i] = _generator.Search(problem);
                });
            }
            else
            {
                for (var i = 0; i < _settings.PopulationSize; i++)
                {
                    population[i] = _generator.Search(problem);
                }
            }
            
            // sort population & determine the best candidate.
            Array.Sort(population);
            var best = population[0];

            // do the mutation/crossover loop until stopping conditions are met.
            var stagnation = 0;
            var generation = 0;
            var elitism = (int)(_settings.PopulationSize * (_settings.ElitismPercentage / 100.0));
            if (elitism == 0 && _settings.ElitismPercentage != 0)
            { // make sure we have at least one, elitism was requested but population was too small.
                elitism = 1;
            }

            var selectionPoolSize = _settings.PopulationSize / 10; // (int)(_settings.PopulationSize * (_settings.CrossOverPercentage / 100.0));
            if (selectionPoolSize < 2 &&
                _settings.CrossOverPercentage != 0)
            { // make sure we have at least 2, some crossover was requested but population was too small.
                selectionPoolSize = 2;
            }
            var crossOverIndividuals = new TCandidate[selectionPoolSize];
            var exclude = new HashSet<int>();
            while (stagnation < _settings.StagnationCount &&
                   generation < _settings.MaxGenerations)
            {
                Logger.Log($"{nameof(GAStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                    $"{this.Name}: Started generation {generation} @ stagnation {stagnation} with {population[0]} and {population[population.Length -1 ]}.");
                // select individuals for crossover.
                exclude.Clear();
                for (var i = 0; i < selectionPoolSize; i++)
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
                if (_useParalell)
                {
                    Parallel.For(elitism, population.Length, (i) =>
                    {
                        if (Random.RandomGenerator.Default.Generate(100f) > _settings.CrossOverPercentage) return;

                        Random.RandomGenerator.Generate2(selectionPoolSize, out var c1, out var c2);
                        population[i] = _crossOver.Apply(crossOverIndividuals[c1], crossOverIndividuals[c2]);
                    });
                }
                else
                {
                    for (var i = elitism; i < population.Length; i++)
                    {
                        if (Random.RandomGenerator.Default.Generate(100f) > _settings.CrossOverPercentage) continue;

                        Random.RandomGenerator.Generate2(selectionPoolSize, out var c1, out var c2);
                        population[i] = _crossOver.Apply(crossOverIndividuals[c1], crossOverIndividuals[c2]);
                    }
                }

                // mutate part of the population.
                if (_useParalell)
                {
                    Parallel.For(elitism, population.Length, (i) =>
                    {
                        if (_settings.MutationPercentage == 0 || Random.RandomGenerator.Default.Generate(100f) > _settings.MutationPercentage) return;

                        // ok, mutate this individual.
                        _mutation.Apply(population[i]); // by ref so should be fine.
                    });
                }
                else
                {
                    for (var i = elitism; i < population.Length; i++)
                    {
                        if (_settings.MutationPercentage == 0 || Random.RandomGenerator.Default.Generate(100) > _settings.MutationPercentage) continue;

                        // ok, mutate this individual.
                        _mutation.Apply(population[i]); // by ref so should be fine.
                    }
                }

                // again, sort things and check for the best.
                Array.Sort(population);
                if (CandidateComparison.Compare(best, population[0]) > 0)
                { // the new candidate is better, yay!
                    stagnation = 0;
                    best = population[0];
                    Logger.Log($"{nameof(GAStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                        $"{this.Name}: New best individual found: {best} @ generation {generation} with stagnation {stagnation}.");
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