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
using Itinero.Optimization.Models.Mapping.Default;
using Itinero.Optimization.Models.Mapping.Directed;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// The model mapper registry.
    /// </summary>
    public class ModelMapperRegistry
    {
        private readonly List<ModelMapper> _modelMappers = new List<ModelMapper>();
        
        /// <summary>
        /// Creates a new model mappers registry.
        /// </summary>
        /// <param name="mappers">The initial mappers.</param>
        public ModelMapperRegistry(params ModelMapper[] mappers)
        {
            _modelMappers.AddRange(mappers);
        }

        /// <summary>
        /// Registers a new model mapper.
        /// </summary>
        /// <param name="mapper">The model mapper.</param>
        public void Register(ModelMapper mapper)
        {
            _modelMappers.Add(mapper);
        }

        /// <summary>
        /// Clears all registered mappers.
        /// </summary>
        public void Clear()
        {
            _modelMappers.Clear();
        }

        /// <summary>
        /// Maps the given model by linking it to the road network.
        /// </summary>
        /// <param name="router">The router to use.</param>
        /// <param name="model">The model to map.</param>
        /// <returns>A mapped model and the mapping details.</returns>
        public (MappedModel mappedModel, IModelMapping mapping) Map(RouterBase router, Model model)
        {
            if (!model.IsValid(out var failReason))
            {
                throw new Exception($"The given model can never be solved: {failReason}");
            }

            var reasonsWhy = new StringBuilder();
            for (var i = _modelMappers.Count - 1; i >= 0; i--)
            {
                // loop from last registered to first.
                if (_modelMappers[i].TryMap(router, model, out var mappings, out var reasonWhy))
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
        
        private static readonly Lazy<ModelMapperRegistry> DefaultLazy = new Lazy<ModelMapperRegistry>(() => new ModelMapperRegistry());
        
        /// <summary>
        /// Gets the default solver registry.
        /// </summary>
        public static ModelMapperRegistry Default => new ModelMapperRegistry(new ModelMapper[]
        {
            DefaultModelMapper.Default,
            DirectedModelMapper.Default
        });
    }
}