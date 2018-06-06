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

namespace Itinero.Optimization.Models.Validation
{
    /// <summary>
    /// Contains extension methods for model validation.
    /// </summary>
    public static class ModelValidation
    {
        /// <summary>
        /// Validates the model, returns a message if invalid.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="message">The message if invalid.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public static bool IsValid(this Model model, out string message)
        {
            message = string.Empty;
            return false;
        }
    }
}