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
//using System;
//using Itinero.Optimization.Solvers.Tours;
//using Itinero.Optimization.Strategies;
//
//namespace Itinero.Optimization.Solvers.TSP._3Opt
//{
//    /// <summary>
//    /// A 3-opt solver.
//    /// </summary>
//    internal sealed class HillClimbing3OptSolver : Strategy<TSProblem, Candidate<TSProblem, Tour>>
//    {
//        private readonly Strategy<TSProblem, Candidate<TSProblem, Tour>> _generator;
//        private readonly bool _nearestNeighbours = false;
//        private readonly bool _dontLook = false;
//        private readonly double _epsilon = 0.001f;
//
//        /// <summary>
//        /// Creates a new solver.
//        /// </summary>
//        public HillClimbing3OptSolver()
//            : this(new RandomSolver())
//        {
//
//        }
//
//        /// <summary>
//        /// Creates a new solver.
//        /// </summary>
//        public HillClimbing3OptSolver(Strategy<TSProblem, Candidate<TSProblem, Tour>> generator, bool nearestNeighbours = true, bool dontLook = true)
//        {
//            _generator = generator;
//            _dontLook = dontLook;
//            _nearestNeighbours = nearestNeighbours;
//        }
//
//        /// <summary>
//        /// Retuns the name of this solver.
//        /// </summary>
//        public override string Name
//        {
//            get
//            {
//                if (_nearestNeighbours && _dontLook)
//                {
//                    return "3OHC_(NN)_(DL)";
//                }
//                else if (_nearestNeighbours)
//                {
//                    return "3OHC_(NN)";
//                }
//                else if (_dontLook)
//                {
//                    return "3OHC_(DL)";
//                }
//                return "3OHC";
//            }
//        }
//
//        /// <summary>
//        /// Solves the given problem.
//        /// </summary>
//        /// <returns></returns>
//        public override Candidate<TSProblem, Tour> Search(TSProblem problem)
//        {
//            // generate some route first.
//            var tour = _generator.Search(problem);
//
//            // improve the existing solution.
//            return this.Apply(problem, tour);
//        }
//
//       
//    }
//}