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
using System.Text;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// The model mapper registry.
    /// </summary>
    public static class ModelMapperRegistry
    {
        private static readonly List<ModelMapperHook> ModelMappers = new List<ModelMapperHook>();
        
        /// <summary> 
        /// A delegate to define a call for mapping a model onto a road network.
        /// </summary>
        /// <param name="router">The router to use.</param>
        /// <param name="model">The model to map.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="reasonWhy">The reason why if the mapping fails.</param>
        public delegate bool TryMapDelegate(RouterBase router, Model model, out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string reasonWhy);

        /// <summary>
        /// Registers a new solver.
        /// </summary>
        /// <param name="name">The name of the mapper.</param>
        /// <param name="tryMap">A function to call the mapper.</param>
        public static void Register(string name, TryMapDelegate tryMap)
        {
            ModelMappers.Add(new ModelMapperHook()
            {
                Name = name,
                TryMap = tryMap
            });
        }

        /// <summary>
        /// Maps the given model by linking it to the road network.
        /// </summary>
        /// <param name="router">The router to use.</param>
        /// <param name="model">The model to map.</param>
        /// <returns>A mapped model and the mapping details.</returns>
        public static (MappedModel mappedModel, IModelMapping mapping) Map(RouterBase router, Model model)
        {
            if (!model.IsValid(out var failReason))
            {
                throw new Exception($"The given model can never be solved: {failReason}");
            }

            var reasonsWhy = new StringBuilder();
            for (var i = ModelMappers.Count - 1; i >= 0; i--)
            {
                // loop from last registered to first.
                if (ModelMappers[i].TryMap(router, model, out var mappings, out var reasonWhy))
                {
                    // mapping worked, return the result.
                    return mappings;
                }

                reasonsWhy.Append(reasonWhy);
                reasonsWhy.Append(Environment.NewLine);
            }

            throw new Exception("The given model cannot be mapped by any of the registered mapppers: " +
                                reasonsWhy.ToString());
        }

        private class ModelMapperHook
        {
            public TryMapDelegate TryMap { get; set; }

            public string Name { get; set; }
        }
    }
}