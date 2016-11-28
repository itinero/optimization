// Itinero.Optimization - Route optimization for .NET
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

using System;

namespace Itinero.Optimization.Algorithms.TurningWeights
{
    /// <summary>
    /// An helper class to handle customer-id's with turning weights.
    /// </summary>
    public static class CustomerHelper
    {
        /// <summary>
        /// Extracts meaning from the directed id by extracting the tiny route embedded in them.
        /// </summary>
        public static void ExtractIndices(int id, out int to, out int from, out int customer)
        {
            var i = id % 4;
            var idx = (id - i) / 2;
            customer = idx / 2;
            switch (i)
            {
                case 0:
                    to = idx + 0;
                    from = idx + 0;
                    return;
                case 1:
                    to = idx + 0;
                    from = idx + 1;
                    return;
                case 2:
                    to = idx + 1;
                    from = idx + 0;
                    return;
                case 3:
                    to = idx + 1;
                    from = idx + 1;
                    return;
            }
            to = int.MaxValue;
            from = int.MaxValue;
        }

        /// <summary>
        /// Creates a directed id from a customer-id.
        /// </summary>
        public static int DirectedIdFrom(int customer, int direction = 0)
        {
            if (direction < 0 || direction > 3) { throw new ArgumentOutOfRangeException("direction"); }

            return (customer * 4) + direction;
        }
    }
}