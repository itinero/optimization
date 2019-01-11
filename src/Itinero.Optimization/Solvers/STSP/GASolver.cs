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

using System.Threading;
using Itinero.Optimization.Solvers.STSP.HillClimbing3Opt;
using Itinero.Optimization.Solvers.STSP.Operators;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Tools.Selectors;

namespace Itinero.Optimization.Solvers.STSP
{
    internal class GASolver : Strategy<STSProblem, Candidate<STSProblem, Tour>>
    {
        private readonly GAStrategy<STSProblem, Candidate<STSProblem, Tour>> _gaStrategy;
        
        public GASolver(Strategy<STSProblem, Candidate<STSProblem, Tour>> generator = null, 
            Operator<Candidate<STSProblem, Tour>> mutation = null, GASettings settings = null, ISelector<Candidate<STSProblem, Tour>> selector = null)
        {
            if (generator == null) generator = HillClimbing3OptSolver.Default;
            if (mutation == null) mutation = EmptyOperator<Candidate<STSProblem, Tour>>.Default;
            if (settings == null) settings = GASettings.Default;
            var crossOver = VisitExchangeOperator.Default;
            
            _gaStrategy = new GAStrategy<STSProblem, Candidate<STSProblem, Tour>>(generator, crossOver, mutation, settings, selector);
            
            
            this.Name = $"{_gaStrategy.Name}";
        }
        
        public override string Name { get; }
        public override Candidate<STSProblem, Tour> Search(STSProblem problem)
        {
            if (problem.Count < 5)
            { // use brute-force solver when problem is very small.
                return BruteForceSolver.Default.Search(problem);
            }
            return _gaStrategy.Search(problem);
        }
        
        private static readonly ThreadLocal<GASolver> DefaultLazy = new ThreadLocal<GASolver>(() => new GASolver());
        public static GASolver Default => DefaultLazy.Value;
    }
}