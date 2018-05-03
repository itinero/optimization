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

using System;
using System.Collections.Generic;
using System.Threading;

namespace Itinero.Optimization.Strategies.Random
{
    /// <summary>
    /// A random generator.
    /// </summary>
    public abstract class RandomGenerator
    {
        /// <summary>
        /// Generates a random integer
        /// </summary>
        public abstract void Generate(byte[] buffer);

        /// <summary>
        /// Generates a random double
        /// </summary>
        public abstract float Generate(float max);

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        public abstract int Generate(int max);

        private static Lazy<RandomGenerator> DefaultRandomGenerator = new Lazy<RandomGenerator>(() =>
        {
            if (RandomGenerator.GetGetNewRandom != null)
            {
                return GetGetNewRandom();
            }
            else
            {
                return new DefaultRandomGenerator();
            }
        });

        /// <summary>
        /// Resets the default random generator.
        /// </summary>
        public static void ResetDefault()
        {
            if (DefaultRandomGenerator.IsValueCreated)
            { 
                DefaultRandomGenerator = new Lazy<RandomGenerator>(() =>
                {
                    if (RandomGenerator.GetGetNewRandom != null)
                    {
                        return GetGetNewRandom();
                    }
                    else
                    {
                        return new DefaultRandomGenerator();
                    }
                });
            }
        }

        /// <summary>
        /// Gets a new random generator.
        /// </summary>
        public static RandomGenerator Default
        {
            get
            {
                return DefaultRandomGenerator.Value;
            }
        }

        /// <summary>
        /// A custom random generator creation function to inject custom random generators.
        /// </summary>
        public static Func<RandomGenerator> GetGetNewRandom
        {
            get;
            set;
        }
    }
}