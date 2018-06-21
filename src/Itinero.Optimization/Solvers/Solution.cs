///*
// *  Licensed to SharpSoftware under one or more contributor
// *  license agreements. See the NOTICE file distributed with this work for 
// *  additional information regarding copyright ownership.
// * 
// *  SharpSoftware licenses this file to you under the Apache License, 
// *  Version 2.0 (the "License"); you may not use this file except in 
// *  compliance with the License. You may obtain a copy of the License at
// * 
// *       http://www.apache.org/licenses/LICENSE-2.0
// * 
// *  Unless required by applicable law or agreed to in writing, software
// *  distributed under the License is distributed on an "AS IS" BASIS,
// *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// *  See the License for the specific language governing permissions and
// *  limitations under the License.
// */
//
//using System.Collections.Generic;
//using Itinero.Optimization.Models.Mapping;
//
//namespace Itinero.Optimization.Solvers
//{
//    /// <summary>
//    /// Represent a generic solution to a VRP.
//    /// </summary>
//    public class Solution
//    {
//        /// <summary>
//        /// Creates a new solution.
//        /// </summary>
//        /// <param name="vehicleAndTours">The tours and associated vehicles.</param>
//        public Solution(IEnumerable<(int vehicle, IEnumerable<int> tour)> vehicleAndTours)
//        {
//            this.Model = model;
//            this.Tours = vehicleAndTours;
//        }
//        
//        /// <summary>
//        /// Gets the vehicles and their tours.
//        /// </summary>
//        public IEnumerable<(int vehicle, IEnumerable<int> tour)> Tours
//        {
//            get;
//        }
//    }
//}