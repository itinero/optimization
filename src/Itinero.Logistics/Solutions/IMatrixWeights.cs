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

using Itinero.Logistics.Weights;

namespace Itinero.Logistics.Solutions
{
    /// <summary>
    /// An abstract representation of a problem containing weights in the form of a matrix.
    /// </summary>
    public interface IMatrixWeights<T>
        where T : struct
    {
        /// <summary>
        /// Gets the weight handler.
        /// </summary>
        WeightHandler<T> WeightHandler
        {
            get;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        T[][] Weights { get; }
    }
}