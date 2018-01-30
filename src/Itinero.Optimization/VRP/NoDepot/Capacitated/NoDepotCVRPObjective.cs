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

using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// An objective of a no-depot CVRP.
    /// </summary>
    public class NoDepotCVRPObjective : ObjectiveBase<NoDepotCVRProblem, NoDepotCVRPSolution, float>
    {
        public override string Name => throw new System.NotImplementedException();

        public override bool IsNonContinuous => throw new System.NotImplementedException();

        public override float Zero => throw new System.NotImplementedException();

        public override float Infinite => throw new System.NotImplementedException();

        public override float Add(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            throw new System.NotImplementedException();
        }
        
        public float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int tourIdx)
        {
            throw new System.NotImplementedException();
        }

        public float[] CalculateCumul(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int tourIdx)
        {
            var tour = solution.Tour(tourIdx);

            // intialize the result array.
            var cumul = new float[tour.Count + 1];

            int previous = -1; // the previous visit.
            float time = 0; // the current weight.
            int idx = 0; // the current index.
            foreach (int visit1 in tour)
            { // loop over all visits.
                if (previous >= 0)
                { // there is a previous visit.
                    // add one visit and the distance to the previous visit.
                    time = time + problem.Weights[previous][visit1];
                    cumul[idx] = time;
                }
                else
                { // there is no previous visit, this is the first one.
                    cumul[idx] = 0;
                }

                idx++; // increase the index.
                previous = visit1; // prepare for next loop.
            }
            // handle the edge last->first.
            time = time + problem.Weights[previous][tour.First];
            cumul[idx] = time;
            return cumul;
        }

        public override float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
        {
            throw new System.NotImplementedException();
        }

        public override int CompareTo(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsZero(NoDepotCVRProblem problem, float fitness)
        {
            throw new System.NotImplementedException();
        }

        public override float Subtract(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            throw new System.NotImplementedException();
        }
    }
}
