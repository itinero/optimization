﻿/*
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
using System.Linq;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Contains extension methods to handle capacity.
    /// </summary>
    public static class 
        CapacityExtensions
    {


        public static bool ConstraintsAreHonored(this Capacity capacity, List<Content> contents, List<ITour> tours)
        {
            if (contents.Count != tours.Count)
            {
                throw new IndexOutOfRangeException($"The number of contents ({contents.Count}) and number of tours ({tours.Count}) don't match");
            }
            for (var i = 0; i < contents.Count; i++)
            {
                var honored = capacity.ConstraintsAreHonored(contents[i], tours[i]);
                if (!honored)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the contents of this tour meet the given capacity contraints. Returns false if not.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="content"></param>
        /// <param name="visits"></param>
        /// <returns></returns>
        public static bool ConstraintsAreHonored(this Capacity capacity, Content content, IEnumerable<int> visits)
        {
             if (capacity.Constraints == null)
            { // no constraints, no work.
                return true;
            }
            
            if (content.Quantities == null ||
                content.Quantities.Length != capacity.Constraints.Length)
            {
                content.Quantities = new float[capacity.Constraints.Length];
            }
            foreach (var constraint in capacity.Constraints)
            {
                var constraint1 = constraint;
                var q = visits.Sum(visit => constraint1.Values[visit]);
                if (q > constraint.Max)
                {
                    return false;
                }
            }

            return true;
        }

        
        /// <summary>
        /// Updates the costs associated with a tour with the given visits.
        /// </summary>
        public static void UpdateCosts(this Capacity capacity, Content content, IEnumerable<int> visits)
        {
            if (capacity.Constraints == null)
            { // no constraints, no work.
                return;
            }

            if (content.Quantities == null ||
                content.Quantities.Length != capacity.Constraints.Length)
            {
                content.Quantities = new float[capacity.Constraints.Length];
            }
            
            for (var i = 0; i < capacity.Constraints.Length; i++)
            {
                var constraint = capacity.Constraints[i];
                var q = 0f;
                foreach (var visit in visits)
                {
                    q += constraint.Values[visit];
                }
                if (q > constraint.Max)
                {
                    throw new Exception($"Contraints violated. The total {constraint.Name} is {q}, but the maximum allowed is {constraint.Max}");
                }
                content.Quantities[i] = q;
            }
        }

        /// <summary>
        /// Updates and checks the costs.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="content">The content.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="visit">The visit.</param>
        /// <returns></returns>
        public static bool UpdateAndCheckCosts(this Capacity capacity, Content content, float weight, int visit)
        {
            if (capacity.Max < weight)
            {
                return false;
            }
            content.Weight = weight;
            if (capacity.Constraints == null)
            { // no constraints, always succeed.
                return true;
            }

            // verify all first.
            for (var i = 0; i < capacity.Constraints.Length; i++)
            {
                var constraint = capacity.Constraints[i];
                var newCost = content.Quantities[i] + constraint.Values[visit];
                if (constraint.Max < newCost)
                {
                    return false;
                }
            }

            // update all after verification.
            for (var i = 0; i < capacity.Constraints.Length; i++)
            {
                var constraint = capacity.Constraints[i];
                content.Quantities[i] = content.Quantities[i] + constraint.Values[visit];
            }
            return true;
        }

        /// <summary>
        /// Updates the quantities with the given sequence exchange.
        /// </summary>
        public static void UpdateExchange(this Capacity capacity, Content content, Seq s1, Seq s2)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var diff = 0f;
                for (var i = 1; i < s2.Length - 1; i++)
                {
                    diff += constraint.Values[s2[i]];
                }
                for (var i = 1; i < s1.Length - 1; i++)
                {
                    diff -= constraint.Values[s1[i]];
                }

                var q = content.Quantities[c] + diff;
                if (q > constraint.Max)
                {
                    throw new Exception("Cannot update quantity, contraint violated.");
                }
                content.Quantities[c] = q;
            }
            return;
        }

        /// <summary>
        /// Returns true if the exchange of the of the two visits is possible within the given capacity constraints.
        /// </summary>
        public static void UpdateExchange(this Capacity capacity, Content content, int visit1, int visit2)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];

                var diff = 0f;
                diff += constraint.Values[visit2];
                diff -= constraint.Values[visit1];

                var q = content.Quantities[c] + diff;
                if (q > constraint.Max)
                {
                    throw new Exception("Cannot update quantity, contraint violated.");
                }
                content.Quantities[c] = q;
            }
            return;
        }

        /// <summary>
        /// Returns true if the exchange of the of the two sequences is possible within the given capacity constraints.
        /// </summary>
        public static bool ExchangeIsPossible(this Capacity capacity, Content content, Seq s1, Seq s2)
        {
            if (capacity.Constraints == null)
            {
                return true;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var diff = 0f;
                for (var i = 1; i < s2.Length - 1; i++)
                {
                    diff += constraint.Values[s2[i]];
                }
                for (var i = 1; i < s1.Length - 1; i++)
                {
                    diff -= constraint.Values[s1[i]];
                }

                if (diff < 0)
                { // if quantity goes down, no need to check constraint.
                    continue;
                }
                var q = content.Quantities[c] + diff;
                if (q > constraint.Max)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the exchange of the of the two visits is possible within the given capacity constraints.
        /// </summary>
        public static bool ExchangeIsPossible(this Capacity capacity, Content content, int removedVisit, int addedVisit)
        {
            return capacity.ExchangeIsPossible(content, removedVisit, addedVisit, null);
        }
        /// <summary>
        /// Returns true if the exchange of the of the two visits is possible within the given capacity constraints.
        /// <param name="capacity"></param>
        /// <param name="content"></param>
        /// <param name="removedVisit"></param>
        /// <param name="addedVisit"></param>
        /// <param name="extraCost">The optional 'extra cost' is subtracted from the constraint if its type matches the capacity metric. This is for additional travel costs, such as a depot round trip</param>
        /// </summary>
        public static bool ExchangeIsPossible(this Capacity capacity, Content content, int removedVisit, int addedVisit,
            float? extraCost)
        {
            if (capacity.Constraints == null)
            {
                return true;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];

                var diff = 0f;
                diff += constraint.Values[addedVisit];
                diff -= constraint.Values[removedVisit];

                if (extraCost != null)
                {
                    diff += (float)extraCost;
                }
                if (diff < 0)
                { // if quanity goes down, no need to check constraint.
                    continue;
                }
                var q = content.Quantities[c] + diff;
                if (q > constraint.Max)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given visits can be added without violating the constraints.
        /// </summary>
        public static bool CanAdd(this Capacity capacity, Content content, int visit)
        {
            if (capacity.Constraints == null)
            {
                return true;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                q += constraint.Values[visit];
                if (q > constraint.Max)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given visits can be added without violating the constraints.
        /// </summary>
        public static bool CanAdd(this Capacity capacity, Content content, IEnumerable<int> visits)
        {
            if (capacity.Constraints == null)
            {
                return true;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                foreach (var visit in visits)
                {
                    q += constraint.Values[visit];
                    if (q > constraint.Max)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given visits can be added without violating the constraints.
        /// </summary>
        public static bool CanAdd(this Capacity capacity, Content content, Seq seq)
        {
            if (capacity.Constraints == null)
            {
                return true;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                for (var v = 1; v < seq.Length - 1; v++)
                {
                    var visit = seq[v];
                    q += constraint.Values[visit];
                    if (q > constraint.Max)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Adds the given visits.
        /// </summary>
        public static void Add(this Capacity capacity, Content content, int visit)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                q += constraint.Values[visit];
                if (q > constraint.Max)
                {
                    throw new Exception("Cannot update quantity, contraint violated.");
                }
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Adds the given visits.
        /// </summary>
        public static void Add(this Capacity capacity, Content content, IEnumerable<int> visits)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                foreach (var visit in visits)
                {
                    q += constraint.Values[visit];
                    if (q > constraint.Max)
                    {
                        throw new Exception("Cannot update quantity, contraint violated.");
                    }
                }
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Adds the given visits.
        /// </summary>
        public static void Add(this Capacity capacity, Content content, Seq seq)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                for (var i = 1; i < seq.Length - 1; i++)
                {
                    q += constraint.Values[seq[i]];
                    if (q > constraint.Max)
                    {
                        throw new Exception("Cannot update quantity, contraint violated.");
                    }
                }
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Removes the given visits.
        /// </summary>
        public static void Remove(this Capacity capacity, Content content, int visit)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                q -= constraint.Values[visit];
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Removes the given visits.
        /// </summary>
        public static void Remove(this Capacity capacity, Content content, IEnumerable<int> visits)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                foreach (var visit in visits)
                {
                    q -= constraint.Values[visit];
                }
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Removes the given visits.
        /// </summary>
        public static void Remove(this Capacity capacity, Content content, Seq seq)
        {
            if (capacity.Constraints == null)
            {
                return;
            }

            for (var c = 0; c < capacity.Constraints.Length; c++)
            {
                var constraint = capacity.Constraints[c];
                var q = content.Quantities[c];

                for (var i = 1; i < seq.Length - 1; i++)
                {
                    q -= constraint.Values[seq[i]];
                    if (q > constraint.Max)
                    {
                        throw new Exception("Cannot update quantity, contraint violated.");
                    }
                }
                content.Quantities[c] = q;
            }
        }

        /// <summary>
        /// Generates empty content based on the given capacity.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <returns></returns>
        public static Content Empty(this Capacity capacity)
        {
            if (capacity.Constraints == null)
            {
                return new Content()
                {
                    Weight = 0
                };
            }
            return new Content()
            {
                Weight = 0,
                Quantities = new float[capacity.Constraints.Length]
            };
        }

        /// <summary>
        /// Describes the content of a tour in relation to a capacity.
        /// </summary>
        public class Content
        {
            /// <summary>
            /// Gets or sets the weight (time or distance for example).
            /// </summary>
            public float Weight { get; set; }

            /// <summary>
            /// The quantities per constraint.
            /// </summary>
            public float[] Quantities { get; set; }
        }
    }
}