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
using System.Threading;
using System.Threading.Tasks;
using Itinero.Logging;
using Itinero.Optimization.Strategies.Random;
using Itinero.Optimization.Strategies.Tools.Selectors;

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
        private readonly Operator<TCandidate> _improvement;
        private readonly CrossOverOperator<TCandidate> _crossOver;
        private readonly ISelector<TCandidate> _selector;
        private readonly GASettings _settings;
        private readonly bool _useParallel = true;

        /// <summary>
        /// Creates a new genetic algorithm.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="crossOver">The crossover operator.</param>
        /// <param name="mutation">The mutation operator.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="improvement">The improvement operator, if any.</param>
        /// <param name="useParallel">Flag to control parallelism.</param>
        public GAStrategy(Func<TProblem, TCandidate> generator, Func<TCandidate, TCandidate, TCandidate> crossOver,
            Func<TCandidate, bool> mutation, GASettings settings,
            ISelector<TCandidate> selector = null, Func<TCandidate, bool> improvement = null, bool useParallel = true)
            : this(generator.ToStrategy(), crossOver, mutation, settings, selector, improvement, useParallel)
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
        /// <param name="improvement">The improvement operator, if any.</param>
        /// <param name="useParallel">Flag to control parallelism.</param>
        public GAStrategy(Strategy<TProblem, TCandidate> generator, CrossOverOperator<TCandidate> crossOver,
            Operator<TCandidate> mutation, GASettings settings,
            ISelector<TCandidate> selector = null, Operator<TCandidate> improvement = null, bool useParallel = true)
        {
            _crossOver = crossOver;
            _generator = generator;
            _mutation = mutation;
            _selector = selector ?? new TournamentSelector<TCandidate>(50, 0.8f);
            _improvement = improvement;

            _settings = settings;
            _useParallel = useParallel;
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
            if (_useParallel)
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
            var cloneable = population[0].CanClone();
            var best = population[0];
            if (cloneable) best = best.Clone();

            // do the mutation/crossover loop until stopping conditions are met.
            var stagnation = 0;
            var maxStagnation = 0;
            var generation = 0;
            var elitism = (int)(_settings.PopulationSize * (_settings.ElitismPercentage / 100.0));
            if (elitism == 0 && _settings.ElitismPercentage >= 0)
            { // make sure we have at least one, elitism was requested but population was too small.
                elitism = 1;
            }

            var selectionPoolSize = _settings.PopulationSize / 10; // (int)(_settings.PopulationSize * (_settings.CrossOverPercentage / 100.0));
            if (selectionPoolSize < 2 &&
                _settings.CrossOverPercentage >= 0)
            { // make sure we have at least 2, some crossover was requested but population was too small.
                selectionPoolSize = 2;
            }
            var crossOverIndividuals = new TCandidate[selectionPoolSize];
            var exclude = new HashSet<int>();
            while (stagnation < _settings.StagnationCount &&
                   generation < _settings.MaxGenerations)
            {
                Logger.Log($"{nameof(GAStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                    $"{this.Name}: Started generation {generation} @ stagnation {stagnation} ({maxStagnation}) with {population[0]} and {population[population.Length -1 ]}.");
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
                if (_useParallel)
                {
                    Parallel.For(elitism, population.Length, (i) =>
                    {
                        var random = Random.RandomGenerator.Default;
                        if (random.Generate(100f) > _settings.CrossOverPercentage) return;

                        random.Generate2(selectionPoolSize, out var c1, out var c2);
                        population[i] = _crossOver.Apply(crossOverIndividuals[c1], crossOverIndividuals[c2]);
                    });
                }
                else
                {
                    for (var i = elitism; i < population.Length; i++)
                    {
                        if (Random.RandomGenerator.Default.Generate(100f) > _settings.CrossOverPercentage) continue;

                        Random.RandomGenerator.Default.Generate2(selectionPoolSize, out var c1, out var c2);
                        population[i] = _crossOver.Apply(crossOverIndividuals[c1], crossOverIndividuals[c2]);
                    }
                }

                // mutate part of the population.
                if (_useParallel)
                {
                    Parallel.For(elitism, population.Length, (i) =>
                    {
                        if (_settings.MutationPercentage <= 0 || Random.RandomGenerator.Default.Generate(100f) > _settings.MutationPercentage) return;

                        // ok, mutate this individual.
                        _mutation.Apply(population[i]); // by ref so should be fine.
                    });
                }
                else
                {
                    for (var i = elitism; i < population.Length; i++)
                    {
                        if (_settings.MutationPercentage <= 0 || Random.RandomGenerator.Default.Generate(100) > _settings.MutationPercentage) continue;

                        // ok, mutate this individual.
                        _mutation.Apply(population[i]); // by ref so should be fine.
                    }
                }

                // select individuals for improvement.
                exclude.Clear();
                for (var i = 0; i < selectionPoolSize; i++)
                { // select individual.
                    var selected = -1;
                    while (selected < 0)
                    {
                        selected = _selector.Select(population, (c) => exclude.Contains(c));
                    }
                    exclude.Add(selected);
                }

                // improve part of the population.
                if (_improvement != null)
                {
                    if (_useParallel)
                    {
                        Parallel.ForEach(exclude, (i) =>
                        { // ok, mutate this individual.
                            if (_settings.ImprovementPercentage <= 0 || Random.RandomGenerator.Default.Generate(100) > _settings.ImprovementPercentage) return;

                            var individual = population[i];
                            if (i <= elitism)
                            { // make sure to only accept improvement in the elite.
                                if (!cloneable) return;
                                individual = individual.Clone();
                                _improvement.Apply(individual); // by ref so should be fine.
                                if (CandidateComparison.Compare(population[0], individual) > 0)
                                {
                                    population[i] = individual;
                                }
                            }
                            else
                            { // apply improvement directly.
                                _improvement.Apply(population[i]);
                            }
                        });
                    }
                    else
                    {
                        foreach (var i in exclude)
                        { // ok, mutate this individual.
                            if (_settings.ImprovementPercentage <= 0 || Random.RandomGenerator.Default.Generate(100) > _settings.ImprovementPercentage) continue;

                            _improvement.Apply(population[i]); // by ref so should be fine.
                        }
                    }
                }

                // again, sort things and check for the best.
                Array.Sort(population);
                if (CandidateComparison.Compare(best, population[0]) > 0)
                { // the new candidate is better, yay!
                    if (stagnation > maxStagnation)
                    {
                        maxStagnation = stagnation;
                    }
                    stagnation = 0;
                    best = population[0];
                    if (cloneable) best = best.Clone();
                    Logger.Log($"{nameof(GAStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                        $"{this.Name}: New best individual found: {best} @ generation {generation} ({maxStagnation}) with stagnation {stagnation}.");
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