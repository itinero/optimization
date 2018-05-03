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
 
namespace Itinero.Optimization.Strategies.Random
{
    /// <summary>
    /// A default random generator implementation.
    /// </summary>
    public sealed class DefaultRandomGenerator : RandomGenerator
    {
        private readonly System.Random _rand;

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public DefaultRandomGenerator()
        {
            _rand = new System.Random();
        }

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public DefaultRandomGenerator(int seed)
        {
            _rand = new System.Random(seed);
        }

        /// <summary>
        /// Generates a random integer
        /// </summary>
        public sealed override void Generate(byte[] buffer)
        {
            _rand.NextBytes(buffer);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        public sealed override float Generate(float max)
        {
            return (float)(_rand.NextDouble() * max);
        }

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        public sealed override int Generate(int max)
        {
            return _rand.Next(max);
        }
    }
}