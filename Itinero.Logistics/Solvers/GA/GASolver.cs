// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solvers.GA
{
    /// <summary>
    /// A Genetic Algorithm (GA) solver.
    /// </summary>
    public class GASolver<TProblem, TObjective, TSolution> : SolverBase<TProblem, TObjective, TSolution>
    {
        private readonly ISolver<TProblem, TObjective, TSolution> _generator;
        private readonly ICrossOverOperator<TProblem, TObjective, TSolution> _crossOver;
        private readonly ISelectionOperator<TProblem, TSolution> _selection;
        private readonly IOperator<TProblem, TObjective, TSolution> _mutation;
        private readonly GASettings _settings;
        private readonly Random _random;

        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        public GASolver(TObjective objective, ISolver<TProblem, TObjective, TSolution> generator,
            ICrossOverOperator<TProblem, TObjective, TSolution> crossOver, ISelectionOperator<TProblem, TSolution> selection,
            IOperator<TProblem, TObjective, TSolution> mutation)
            : this(objective, generator, crossOver, selection, mutation, GASettings.Default)
        {

        }

        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        public GASolver(TObjective objective, ISolver<TProblem, TObjective, TSolution> generator,
            ICrossOverOperator<TProblem, TObjective, TSolution> crossOver, ISelectionOperator<TProblem, TSolution> selection,
            IOperator<TProblem, TObjective, TSolution> mutation, GASettings settings)
        {
            _generator = generator;
            _crossOver = crossOver;
            _mutation = mutation;
            _selection = selection;
            _settings = settings;

            _random = new Random();
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
        public override TSolution Solve(TProblem problem, TObjective objective, out double fitness)
        {
            var population = new Individual<TSolution>[_settings.PopulationSize];

            // generate initial population.
            var solutionCount = 0;
            while (solutionCount < _settings.PopulationSize)
            {
                double localFitness;
                var solution = _generator.Solve(problem, objective, out localFitness);
                population[solutionCount] = new Individual<TSolution>()
                {
                    Fitness = localFitness,
                    Solution = solution
                };
                solutionCount++;
            }

            // sort population.
            Array.Sort(population, (x, y) =>
                {
                    return x.Fitness.CompareTo(y.Fitness);
                });
            var bestIndividual = population[0];
            this.ReportIntermidiateResult(bestIndividual.Solution);

            // mutate/crossover population.
            var stagnation = 0;
            var generation = 0;
            var elitism = (int)(_settings.PopulationSize * (_settings.ElitismPercentage / 100.0));
            var crossOver = (int)(_settings.PopulationSize * (_settings.CrossOverPercentage / 100.0));
            var crossOverIndividuals = new Individual<TSolution>[crossOver];
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
                        selected = _selection.Select(problem, population, exclude);
                    }
                    crossOverIndividuals[i] = population[selected];
                    exclude.Add(selected);
                }

                // replace part of the population by offspring.
                for (int i = elitism; i < population.Length; i++)
                {
                    // take two random parents.
                    var individual1 = _random.Next(crossOver);
                    var individual2 = _random.Next(crossOver - 1);
                    if (individual1 <= individual2)
                    { // make sure they are different.
                        individual2++;
                    }

                    // create offspring.
                    var offspringFitness = 0.0;
                    var offspring = _crossOver.Apply(problem, objective, population[individual1].Solution,
                        population[individual2].Solution, out offspringFitness);
                    population[i] = new Individual<TSolution>()
                    {
                        Solution = offspring,
                        Fitness = offspringFitness
                    };
                }

                // mutate part of the population.
                for (int i = elitism; i < population.Length; i++)
                {
                    if (_random.Next(100) <= _settings.MutationPercentage)
                    { // ok, mutate this individual.
                        var mutatedDelta = 0.0;
                        if (_mutation.Apply(problem, objective, population[i].Solution, out mutatedDelta))
                        { // mutation succeeded.
                            population[i].Fitness = population[i].Fitness - mutatedDelta;
                        }
                    }
                }

                // sort new population.
                Array.Sort(population, (x, y) =>
                {
                    return x.Fitness.CompareTo(y.Fitness);
                });
                if (bestIndividual.Fitness > population[0].Fitness)
                { // a better individual was found.
                    bestIndividual = population[0];
                    stagnation = 0; // reset stagnation flag.
                    this.ReportIntermidiateResult(bestIndividual.Solution);
                }
                else
                { // no better solution found.
                    stagnation++;
                }
            }

            fitness = bestIndividual.Fitness;
            return bestIndividual.Solution;
        }
    }
}