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
 
namespace Itinero.Optimization.General
{
    /// <summary>
    /// Contains a collection of generally reusable delegates.
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        /// Returns true if the two given tours overlap.
        /// </summary>
        /// <param name="problem">The problem these tours are for.</param>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns>True if the two tours geographically overlap.</returns>        
        public delegate bool OverlapsFunc<TProblem, TTour>(TProblem problem, TTour tour1, TTour tour2);
    }
}