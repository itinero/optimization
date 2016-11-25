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

using Itinero.Algorithms.Weights;
using Itinero.Profiles;

namespace Itinero.Logistics.Routing.Weights
{
    /// <summary>
    /// Contains extension methods related to the augmented weight.
    /// </summary>
    public static class AugmentedWeightExtensions
    {
        /// <summary>
        /// Sets the given weight as a max for the given metric.
        /// </summary>
        public static Weight SetWithMetric(this Weight weight, ProfileMetric metric, float value)
        {
            var maxWeight = new Weight()
            {
                Distance = float.MaxValue,
                Time = float.MaxValue,
                Value = float.MaxValue
            };
            switch (metric)
            {
                case ProfileMetric.Custom:
                    maxWeight.Value = value;
                    break;
                case ProfileMetric.DistanceInMeters:
                    maxWeight.Distance = value;
                    break;
                case ProfileMetric.TimeInSeconds:
                    maxWeight.Time = value;
                    break;
            }
            return maxWeight;
        }
    }
}
