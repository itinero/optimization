// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero.Logistics.Algorithms;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solvers.GA
{
    /// <summary>
    /// A selector selecting individials using a tournament base selection.
    /// </summary>
    public class TournamentSelectionOperator<TProblem, TSolution> : ISelectionOperator<TProblem, TSolution>
    {
        private double _tournamentSize;
        private double _tournamentProbability;
        private readonly IRandomGenerator _random = RandomGeneratorExtensions.GetRandom();

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
        public int Select(TProblem problem, Individual<TSolution>[] population, System.Collections.Generic.ISet<int> exclude)
        {
            var tournamentSizeInt = (int)System.Math.Ceiling(((_tournamentSize / 100f) * (double)population.Length));
            var tempPop = new List<Tuple<int, Individual<TSolution>>>(tournamentSizeInt);

            while (tempPop.Count < tournamentSizeInt)
            { // keep looping until enough individuals are selected or until no more are available.
                var idx = _random.Generate(population.Length);
                if (exclude == null || !exclude.Contains(idx))
                { // do not tournament excluded solutions.
                    tempPop.Add(new Tuple<int, Individual<TSolution>>(idx, population[idx]));
                }
            }

            // sort the population..
            tempPop.Sort((x, y) =>
            {
                return y.Item2.Fitness.CompareTo(x.Item2.Fitness);
            });

            // choose a candidate.
            for (var idx = 0; idx < tempPop.Count; idx++)
            { // choose a candidate.
                if (_random.Generate(1.0) < _tournamentProbability)
                { // candidate choosen!
                    return tempPop[idx].Item1;
                }
            }
            return -1;
        }
    }
}