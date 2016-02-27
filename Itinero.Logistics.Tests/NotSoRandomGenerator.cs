// Itinero - OpenStreetMap (OSM) SDK
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

using Itinero.Logistics.Algorithms;

namespace Itinero.Logistics.Tests
{
    /// <summary>
    /// A not-so random generator that just returns values from an array.
    /// </summary>
    public class NotSoRandomGenerator : IRandomGenerator
    {
        private readonly double[] _notSoRandomDoubles;
        private readonly int[] _notSoRandomIntegers;

        /// <summary>
        /// Creates a new not-so random generator.
        /// </summary>
        public NotSoRandomGenerator(double[] notSoRandomDoubles, int[] notSoRandomIntegers)
        {
            _notSoRandomDoubles = notSoRandomDoubles;
            _notSoRandomIntegers = notSoRandomIntegers;
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        /// <param name="buffer"></param>
        public void Generate(byte[] buffer)
        {

        }

        private int _currentDouble = -1;

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Generate(double max)
        {
            _currentDouble++;
            if (_currentDouble == _notSoRandomDoubles.Length)
            {
                _currentDouble = 0;
            }
            return _notSoRandomDoubles[_currentDouble];
        }

        private int _currentInt = -1;

        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Generate(int max)
        {
            _currentInt++;
            if(_currentInt == _notSoRandomIntegers.Length)
            {
                _currentInt = 0;
            }
            return _notSoRandomIntegers[_currentInt];
        }

        /// <summary>
        /// Generates a random unicode string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GenerateString(int length)
        {
            return string.Empty;
        }
    }
}