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
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.Solvers.GA
{
    /// <summary>
    /// A Genetic Algorithm (GA) solver.
    /// </summary>
    public class GASolver<TWeight, TProblem, TObjective, TSolution, TFitness> : SolverBase<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
        where TWeight : struct
    {
        private readonly ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> _generator;
        private readonly ICrossOverOperator<TWeight, TProblem, TObjective, TSolution, TFitness> _crossOver;
        private readonly ISelectionOperator<TProblem, TSolution, TObjective, TFitness> _selection;
        private readonly IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> _mutation;
        private readonly GASettings _settings;
        private readonly System.Random _random;

        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        public GASolver(TObjective objective, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> generator,
            ICrossOverOperator<TWeight, TProblem, TObjective, TSolution, TFitness> crossOver, ISelectionOperator<TProblem, TSolution, TObjective, TFitness> selection,
            IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> mutation)
            : this(objective, generator, crossOver, selection, mutation, GASettings.Default)
        {

        }

        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        public GASolver(TObjective objective, ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> generator,
            ICrossOverOperator<TWeight, TProblem, TObjective, TSolution, TFitness> crossOver, ISelectionOperator<TProblem, TSolution, TObjective, TFitness> selection,
            IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> mutation, GASettings settings)
        {
            _generator = generator;
            _crossOver = crossOver;
            _mutation = mutation;
            _selection = selection;
            _settings = settings;

            _random = new System.Random();
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return string.Format("GA_[{0}_{1}_{2}_{3}]",
                    _generator.Name, _mutation.Name, _crossOver.Name, _selection.Name);
            }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness)
        {
            var population = new Individual<TSolution, TFitness>[_settings.PopulationSize];

            // generate initial population.
            var solutionCount = 0;
            while (solutionCount < _settings.PopulationSize)
            {
                TFitness localFitness;
                var solution = _generator.Solve(problem, objective, out localFitness);
                population[solutionCount] = new Individual<TSolution, TFitness>()
                {
                    Fitness = localFitness,
                    Solution = solution
                };
                solutionCount++;
            }

            // sort population.
            Array.Sort(population, (x, y) =>
            {
                return objective.CompareTo(problem, x.Fitness, y.Fitness);
            });
            var bestIndividual = population[0];
            this.ReportIntermidiateResult(bestIndividual.Solution);
            
            Console.WriteLine("Generation {0}: {1} -> {2}", 0,
                population[0].Fitness, population[population.Length - 1].Fitness);

            // mutate/crossover population.
            var stagnation = 0;
            var generation = 0;
            var elitism = (int)(_settings.PopulationSize * (_settings.ElitismPercentage / 100.0));
            if (elitism == 0 && _settings.ElitismPercentage != 0)
            {
                elitism = 1;
            }
            var crossOver = (int)(_settings.PopulationSize * (_settings.CrossOverPercentage / 100.0));
            var crossOverIndividuals = new Individual<TSolution, TFitness>[crossOver];
            var exclude = new HashSet<int>();
            while (stagnation < _settings.StagnationCount &&
                generation < _settings.MaxGenerations &&
                !this.IsStopped)
            {
                // select individuals for crossover.
                exclude.Clear();
                for (int i = 0; i < crossOver; i++)
                {
                    // select individual.
                    var selected = -1;
                    while (selected < 0)
                    {
                        selected = _selection.Select(problem, objective, population, exclude);
                    }
                    crossOverIndividuals[i] = population[selected];
                    exclude.Add(selected);
                }

                // replace part of the population by offspring.
                for (int i = population.Length - 1; i > population.Length - 1 - elitism - crossOver; i--)
                {
                    // take two random parents.
                    var individual1 = _random.Next(crossOver);
                    var individual2 = _random.Next(crossOver - 1);
                    if (individual1 <= individual2)
                    { // make sure they are different.
                        individual2++;
                    }

                    // create offspring.
                    var offspring = _crossOver.Apply(problem, objective, crossOverIndividuals[individual1].Solution,
                        crossOverIndividuals[individual2].Solution, out TFitness _);
                    population[i] = new Individual<TSolution, TFitness>()
                    {
                        Solution = offspring,
                        Fitness = objective.Calculate(problem, offspring)
                    };
                }

                // sort new population.
                Array.Sort(population, (x, y) =>
                {
                    return objective.CompareTo(problem, x.Fitness, y.Fitness);
                });

                // mutate part of the population.
                for (int i = elitism; i < population.Length; i++)
                {
                    if (_random.Next(100) < _settings.MutationPercentage)
                    { // ok, mutate this individual.
                        TFitness mutatedDelta;
                        if (_mutation.Apply(problem, objective, population[i].Solution, out mutatedDelta))
                        { // mutation succeeded.
                            population[i].Fitness = objective.Calculate(problem, population[i].Solution);
                        }
                    }
                }

                // sort new population.
                Array.Sort(population, (x, y) =>
                {
                    return objective.CompareTo(problem, x.Fitness, y.Fitness);
                });
                if (objective.IsBetterThan(problem, population[0].Fitness, bestIndividual.Fitness))
                { // a better individual was found.
                    Itinero.Logging.Logger.Log("GASolver", Itinero.Logging.TraceEventType.Verbose,
                        "Found a better solution at generation {0}: {1} -> {2}", generation, bestIndividual.Fitness, population[0].Fitness);

                    bestIndividual = population[0];
                    stagnation = 0; // reset stagnation flag.
                    this.ReportIntermidiateResult(bestIndividual.Solution);
                }
                else
                { // no better solution found.
                    stagnation++;
                }
                generation++;

                Console.WriteLine("Generation {0}: {1} -> {2}", generation, 
                    population[0].Fitness, population[population.Length - 1].Fitness);

                if (EqualityComparer<TFitness>.Default.Equals(population[0].Fitness, population[population.Length - 1].Fitness))
                {
                    //break;
                }
            }

            fitness = bestIndividual.Fitness;
            return bestIndividual.Solution;
        }
    }
}
