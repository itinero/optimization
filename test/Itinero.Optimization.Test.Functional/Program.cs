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

using Itinero.LocalGeo;
using Itinero.Optimization.Abstract.Tours;
using System;
using System.IO;
using System.Collections.Generic;
using Itinero.Logging;

namespace Itinero.Optimization.Test.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnableLogging();

#if DEBUG
            Itinero.Logging.Logger.Log("Program", TraceEventType.Information, "Performance tests are running in Debug, please run in Release mode.");
#endif

            // invoke case-specific tests.

            // TSP.TSPTests.Run();
            // STSP.STSPTests.Run();
            // TSP_TW.TSPTWTests.Run();

            VRP.NoDepot.Capacitated.NoDepotCVRPTests.Run();
        }

        private static void EnableLogging()
        {
            var loggingBlacklist = new HashSet<string>(
                new string[] { 
                    "StreamProgress",
                    "RouterDbStreamTarget",
                    "RouterBaseExtensions",
                    "HierarchyBuilder",
                    "RestrictionProcessor",
                    "NodeIndex",
                    "RouterDb",
                    "SeededLocalizedCheapestInsertionSolver",
                    "3OHC_(NN)_(DL)",
                    "IterativeSolver",
                    "VNSSolver" });
            OsmSharp.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                if (loggingBlacklist.Contains(o))
                {
                    return;
                }
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                if (loggingBlacklist.Contains(o))
                {
                    return;
                }
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };
        }
    }
}