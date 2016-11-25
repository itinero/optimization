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

namespace Itinero.Logistics.Routing.Weights
{
    /// <summary>
    /// A weight handler for augmented weights.
    /// </summary>
    public class AugmentedTimeWeightHandler : Itinero.Logistics.Weights.WeightHandler<Weight>
    {
        /// <summary>
        /// Gets the weight that represents infinity.
        /// </summary>
        public override Weight Infinity
        {
            get
            {
                return new Weight()
                {
                    Distance = float.MaxValue,
                    Time = float.MaxValue,
                    Value = float.MaxValue
                };
            }
        }

        /// <summary>
        /// Gets the weight that represents zero.
        /// </summary>
        public override Weight Zero
        {
            get
            {
                return new Weight()
                {
                    Distance = 0,
                    Time = 0,
                    Value = 0
                };
            }
        }

        /// <summary>
        /// Adds weight1 and weight2.
        /// </summary>
        public override Weight Add(Weight weight1, Weight weight2)
        {
            return new Weight()
            {
                Distance = weight1.Distance + weight2.Distance,
                Time = weight1.Time + weight2.Time,
                Value = weight1.Value + weight2.Value
            };
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        public override int Compare(Weight x, Weight y)
        {
            return x.Time.CompareTo(y.Time);
        }

        /// <summary>
        /// Divides the given weight with the given divider.
        /// </summary>
        public override Weight Divide(Weight weight, float divider)
        {
            return new Weight()
            {
                Distance = weight.Distance / divider,
                Time = weight.Time / divider,
                Value = weight.Value / divider
            };
        }

        /// <summary>
        /// Gets the time component.
        /// </summary>
        public override float GetTime(Weight weight)
        {
            return weight.Time;
        }

        /// <summary>
        /// Returns true if the given weight is bigger in any of it's fields compared to the other weight.
        /// </summary>
        public override bool IsLargerThanAny(Weight weight, Weight other)
        {
            return weight.Time > other.Time &&
                weight.Distance > other.Distance &&
                weight.Value > other.Value;
        }

        /// <summary>
        /// Multiplies the given weight with the given factor.
        /// </summary>
        public override Weight Multiply(Weight weight, float factor)
        {
            return new Weight()
            {
                Distance = weight.Distance * factor,
                Time = weight.Time * factor,
                Value = weight.Value * factor
            };
        }

        /// <summary>
        /// Subtracts weight2 from weight1.
        /// </summary>
        public override Weight Subtract(Weight weight1, Weight weight2)
        {
            return new Weight()
            {
                Distance = weight1.Distance - weight2.Distance,
                Time = weight1.Time - weight2.Time,
                Value = weight1.Value - weight2.Value
            };
        }
    }
}