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

using BenchmarkDotNet.Attributes;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP;
using Itinero.Optimization.Solvers.TSP.EAX;

namespace Itinero.Optimization.Tests.Benchmarks.Solvers.TSP.EAX
{
    public class EAXSolverBenchmark
    {
        private readonly TSProblem BR17 = new TSProblem(0, 0, new float[][] {
            new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
            new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
            new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
            new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
            new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
            new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
            new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
            new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
            new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
            new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
            new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
            new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
            new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
            new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
            new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
            new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
            new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}});
        
        [Benchmark]
        public Tour SolveBR17()
        {
            return EAXSolver.Default.Search(BR17).Solution;
        }
    }
}