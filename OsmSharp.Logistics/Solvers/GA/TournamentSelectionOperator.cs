// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solvers.GA
{
    /// <summary>
    /// A selector selecting individials using a tournament base selection.
    /// </summary>
    /// <typeparam name="TProblem"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    public class TournamentSelectionOperator<TProblem, TSolution> : ISelectionOperator<TProblem, TSolution>
    {
        private double _tournamentSize;
        private double _tournamentProbability;

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
        /// <param name="tournamentSize"></param>
        /// <param name="tournamentProbability"></param>
        public TournamentSelectionOperator(
            double tournamentSize,
            double tournamentProbability)
        {
            _tournamentSize = tournamentSize;
            _tournamentProbability = tournamentProbability;
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
                var idx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(population.Length);
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
                if (OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0) < _tournamentProbability)
                { // candidate choosen!
                    return tempPop[idx].Item1;
                }
            }
            return -1;
        }
    }
}