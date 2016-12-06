﻿// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Routes;

namespace Itinero.Optimization.STSP
{
    /// <summary>
    /// The default TSP objective.
    /// </summary>
    public sealed class STSPObjective : ObjectiveBase<STSProblem, Route, STSPFitness>
    {
        /// <summary>
        /// Gets the value that represents infinity.
        /// </summary>
        public sealed override STSPFitness Infinite
        {
            get
            {
                return new STSPFitness()
                {
                    Customers = int.MaxValue,
                    Weight = float.MaxValue
                };
            }
        }

        /// <summary>
        /// Returns true if the object is non-linear.
        /// </summary>
        public sealed override bool IsNonContinuous
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public sealed override string Name
        {
            get
            {
                return "STSP";
            }
        }

        /// <summary>
        /// Gets the value that represents 0.
        /// </summary>
        public sealed override STSPFitness Zero
        {
            get
            {
                return new STSPFitness()
                {
                    Customers = 0,
                    Weight = 0
                };
            }
        }

        /// <summary>
        /// Adds the two given fitness values.
        /// </summary>
        public sealed override STSPFitness Add(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            return new STSPFitness()
            {
                Customers = fitness1.Customers + fitness2.Customers,
                Weight = fitness1.Weight + fitness2.Weight
            };
        }

        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public sealed override STSPFitness Calculate(STSProblem problem, Route solution)
        {
            var fitness = new STSPFitness()
            {
                Customers = solution.Count,
                Weight = 0
            };
            var weights = problem.Weights;
            foreach (var pair in solution.Pairs())
            {
                fitness.Weight = fitness.Weight + weights[pair.From][pair.To];
            }
            return fitness;
        }

        /// <summary>
        /// Compares the two fitness values.
        /// </summary>
        public sealed override int CompareTo(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            if (fitness1.Customers == fitness2.Customers)
            {
                return fitness1.Weight.CompareTo(fitness2.Weight);
            }
            return fitness1.Customers.CompareTo(fitness2.Customers);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public sealed override bool IsZero(STSProblem problem, STSPFitness fitness)
        {
            return fitness.Weight == 0 &&
                fitness.Customers == 0;
        }

        /// <summary>
        /// Subtracts the given fitness values.
        /// </summary>
        public sealed override STSPFitness Subtract(STSProblem problem, STSPFitness fitness1, STSPFitness fitness2)
        {
            return new STSPFitness()
            {
                Customers = fitness1.Customers - fitness2.Customers,
                Weight = fitness1.Weight - fitness2.Weight
            };
        }
    }
}