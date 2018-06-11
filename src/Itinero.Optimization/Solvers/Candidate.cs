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

namespace Itinero.Optimization.Solvers
{
    /// <summary>
    /// Represents a candidate, a solution, the problem it's for, and associated fitness value.
    /// </summary>
    /// <typeparam name="TSolution">The solution type.</typeparam>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    public class Candidate<TProblem, TSolution> : IComparable<Candidate<TProblem, TSolution>>
    {
        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        public TSolution Solution { get; set; }
        
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        public float Fitness { get; set; }
        
        /// <summary>
        /// Gets or sets the problem.
        /// </summary>
        public TProblem Problem { get; set; }

        /// <inheritdoc />
        public virtual int CompareTo(Candidate<TProblem, TSolution> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return this.Fitness.CompareTo(other.Fitness);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Fitness}@{this.Solution}";
        }
    }
    
    /// <summary>
    /// Represents a candidate, a solution, the problem it's for, and associated fitness value.
    /// </summary>
    /// <typeparam name="TSolution">The solution type.</typeparam>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TFitness">The fitness type.</typeparam>
    public class Candidate<TProblem, TSolution, TFitness> : IComparable<Candidate<TProblem, TSolution, TFitness>>
        where TFitness : IComparable
    {
        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        public TSolution Solution { get; set; }
        
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        public TFitness Fitness { get; set; }
        
        /// <summary>
        /// Gets or sets the problem.
        /// </summary>
        public TProblem Problem { get; set; }

        /// <inheritdoc />
        public virtual int CompareTo(Candidate<TProblem, TSolution, TFitness> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return this.Fitness.CompareTo(other.Fitness);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Fitness}@{this.Solution}";
        }
    }
}