// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

using System.Collections.Generic;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Profiles;

namespace Itinero.Optimization.Routing.Matrices
{
    /// <summary>
    /// A wrapper around the weight matrix algorithm implementing the abstraction description of the functionality.
    /// </summary>
    public sealed class WeightMatrixAlgorithmWrapper : IWeightMatrixAlgorithm
    {
        private readonly WeightMatrixAlgorithm _weightMatrixAlgorithm;

        /// <summary>
        /// Creates a new weight matrix algorithm wrapper.
        /// </summary>
        public WeightMatrixAlgorithmWrapper(WeightMatrixAlgorithm weightMatrixAlgorithm)
        {
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _weightMatrixAlgorithm.ErrorMessage;
            }
        }

        /// <summary>
        /// Returns true if the algorithm has run.
        /// </summary>
        public bool HasRun
        {
            get
            {
                return _weightMatrixAlgorithm.HasRun;
            }
        }

        /// <summary>
        /// Returns true if the algorithm has succeeded.
        /// </summary>
        public bool HasSucceeded
        {
            get
            {
                return _weightMatrixAlgorithm.HasSucceeded;
            }
        }

        /// <summary>
        /// Returns the mass resolver used.
        /// </summary>
        public MassResolvingAlgorithm MassResolver
        {
            get
            {
                return _weightMatrixAlgorithm.MassResolver;
            }
        }

        /// <summary>
        /// Returns the profile.
        /// </summary>
        public IProfileInstance Profile
        {
            get
            {
                return _weightMatrixAlgorithm.Profile;
            }
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        public RouterBase Router
        {
            get
            {
                return _weightMatrixAlgorithm.Router;
            }
        }

        /// <summary>
        /// Returns the routerpoints.
        /// </summary>
        public List<RouterPoint> RouterPoints
        {
            get
            {
                return _weightMatrixAlgorithm.RouterPoints;
            }
        }
        
        /// <summary>
        /// Returns the weights.
        /// </summary>
        public float[][] Weights
        {
            get
            {
                return _weightMatrixAlgorithm.Weights;
            }
        }

        /// <summary>
        /// Returns the index of the given point.
        /// </summary>
        public int IndexOf(int originalRouterPointIndex)
        {
            return _weightMatrixAlgorithm.IndexOf(originalRouterPointIndex);
        }

        /// <summary>
        /// Runs the algorithm.
        /// </summary>
        public void Run()
        {
            _weightMatrixAlgorithm.Run();
        }

        /// <summary>
        /// Returns the errors indexed per original routerpoint index.
        /// </summary>
        public Dictionary<int, RouterPointError> Errors
        {
            get
            {
                return _weightMatrixAlgorithm.Errors;
            }
        }

        /// <summary>
        /// Gets the original index of the given routerpoint index.
        /// </summary>
        public int OriginalIndexOf(int routerPointIdx)
        {
            return _weightMatrixAlgorithm.OriginalIndexOf(routerPointIdx);
        }
    }
}
