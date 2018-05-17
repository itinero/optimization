/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Tests
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