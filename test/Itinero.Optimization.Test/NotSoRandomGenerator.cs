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

using Itinero.Optimization.Algorithms.Random;

namespace Itinero.Optimization.Test
{
    /// <summary>
    /// A not-so random generator that just returns values from an array.
    /// </summary>
    public class NotSoRandomGenerator : RandomGenerator
    {
        private readonly float[] _notSoRandomDoubles;
        private readonly int[] _notSoRandomIntegers;

        /// <summary>
        /// Creates a new not-so random generator.
        /// </summary>
        public NotSoRandomGenerator(float[] notSoRandomDoubles, int[] notSoRandomIntegers)
        {
            _notSoRandomDoubles = notSoRandomDoubles;
            _notSoRandomIntegers = notSoRandomIntegers;
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        /// <param name="buffer"></param>
        public override void Generate(byte[] buffer)
        {

        }

        private int _currentDouble = -1;

        /// <summary>
        /// Generates a random float
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public override float Generate(float max)
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
        public override int Generate(int max)
        {
            _currentInt++;
            if (_currentInt == _notSoRandomIntegers.Length)
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
