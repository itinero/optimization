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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Operations;
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// A local search procedure to move around and improve the time window 'violations' in a solution.
    /// </summary>
    public class Local1Shift<TObjective> : IOperator<float, TSPTWProblem, TObjective, Tour, float>
        where TObjective : TSPTWObjectiveBase
    {
        private readonly bool _assumeFeasible;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        public Local1Shift()
        {

        }

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        public Local1Shift(bool assumeFeasible)
        {
            _assumeFeasible = assumeFeasible;
        }

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LCL_1SHFT_TW"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }

        private bool[] _validFlags;

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            // STRATEGY: 
            // 1: try to move a violated customer backwards.
            // 2: try to move a non-violated customer forward.
            // 3: try to move a non-violated customer backward.
            // 4: try to move a violated customer forward.
            if (!_assumeFeasible)
            {
                if (this.MoveViolatedBackward(problem, objective, solution, out delta))
                { // success already, don't try anything else.
                    return delta > 0;
                }
            }
            if (this.MoveNonViolatedForward(problem, objective, solution, out delta))
            { // success already, don't try anything else.
                return delta > 0;
            }
            if (this.MoveNonViolatedBackward(problem, objective, solution, out delta))
            { // success already, don't try anything else.
                return delta > 0;
            }
            if (!_assumeFeasible)
            {
                if (this.MoveViolatedForward(problem, objective, solution, out delta))
                { // success already, don't try anything else.
                    return delta > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        public bool MoveViolatedBackward(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            float time, waitTime, violatedTime;
            int violated;
            var fitness = objective.Calculate(problem, solution, out violated, out violatedTime, out waitTime, out time, ref _validFlags);

            // if no violated customer found return false.
            if (violated == 0)
            {
                delta = 0;
                return false;
            }

            // loop over all customers.
            var enumerator = solution.GetEnumerator();
            var position = 0;
            while (enumerator.MoveNext())
            {
                if (position == 0)
                { // don't move the first customer.
                    position++;
                    continue;
                }

                // get the id of the current customer.
                var current = enumerator.Current;
                var id = DirectedHelper.ExtractId(current);

                if (problem.Last == id)
                { // don't place a fixed last customer.
                    position++;
                    continue;
                }

                // is this customer violated.
                if (_validFlags[id])
                { // no it's not, move on.
                    position++;
                    continue;
                }

                // move over all customers before the current position.
                var enumerator2 = solution.GetEnumerator();
                var position2 = 0;
                while (enumerator2.MoveNext())
                {
                    var current2 = enumerator2.Current;
                    var id2 = DirectedHelper.ExtractId(current2);
                    if (problem.Last == id2 &&
                        problem.Last != problem.First)
                    { // don't place a fixed last customer.
                        position2++;
                        continue;
                    }

                    // test and check fitness.
                    // TODO: do best-placement? check best turn at 'current'.
                    var shiftedTour = solution.GetShiftedAfter(current, current2); // generates a tour as if current was placed right after current2.
                    var newFitness = objective.Calculate(problem, shiftedTour);

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(current, current2);
                        return true;
                    }

                    position2++;
                    if (position2 >= position)
                    { // stop, we've reached the customer to place.
                        break;
                    }
                }

                position++;
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool MoveNonViolatedForward(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            float time, waitTime, violatedTime;
            int violated;
            var fitness = objective.Calculate(problem, solution, out violated, out violatedTime, out waitTime, out time, ref _validFlags);

            // if no valid customer found return false.
            if (violated == solution.Count)
            {
                delta = 0;
                return false;
            }
            
            // loop over all customers.
            var enumerator = solution.GetEnumerator();
            var position = 0;
            while (enumerator.MoveNext())
            {
                if (position == 0)
                { // don't move the first customer.
                    position++;
                    continue;
                }

                // get the id of the current customer.
                var current = enumerator.Current;
                var id = DirectedHelper.ExtractId(current);

                if (problem.Last == id)
                { // don't place a fixed last customer.
                    position++;
                    continue;
                }

                // is this customer valid.
                if (!_validFlags[id])
                { // no it's not, move on.
                    position++;
                    continue;
                }

                // move over all customers before the current position.
                var enumerator2 = solution.GetEnumerator();
                var position2 = 0;
                while (enumerator2.MoveNext())
                {
                    if (position2 <= position)
                    { // only consider placement after.
                        position2++;
                        continue;
                    }

                    var current2 = enumerator2.Current;
                    var id2 = DirectedHelper.ExtractId(current2);
                    if (problem.Last == id2 &&
                        problem.Last != problem.First)
                    { // don't place a fixed last customer.
                        position2++;
                        continue;
                    }

                    // test and check fitness.
                    // TODO: do best-placement? check best turn at 'current'.
                    var shiftedTour = solution.GetShiftedAfter(current, current2); // generates a tour as if current was placed right after current2.
                    var newFitness = objective.Calculate(problem, shiftedTour);

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(current, current2);
                        return true;
                    }

                    position2++;
                }

                position++;
            }

            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool MoveNonViolatedBackward(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            float time, waitTime, violatedTime;
            int violated;
            var fitness = objective.Calculate(problem, solution, out violated, out violatedTime, out waitTime, out time, ref _validFlags);

            // if no valid customer found return false.
            if (violated == solution.Count)
            {
                delta = 0;
                return false;
            }

            // loop over all customers.
            var enumerator = solution.GetEnumerator();
            var position = 0;
            while (enumerator.MoveNext())
            {
                if (position == 0)
                { // don't move the first customer.
                    position++;
                    continue;
                }

                // get the id of the current customer.
                var current = enumerator.Current;
                var id = DirectedHelper.ExtractId(current);

                if (problem.Last == id)
                { // don't place a fixed last customer.
                    position++;
                    continue;
                }

                // is this customer valid.
                if (!_validFlags[id])
                { // no it's not, move on.
                    position++;
                    continue;
                }

                // move over all customers before the current position.
                var enumerator2 = solution.GetEnumerator();
                var position2 = 0;
                while (enumerator2.MoveNext())
                {
                    var current2 = enumerator2.Current;
                    var id2 = DirectedHelper.ExtractId(current2);
                    if (problem.Last == id2 &&
                        problem.Last != problem.First)
                    { // don't place a fixed last customer.
                        position2++;
                        continue;
                    }

                    // test and check fitness.
                    // TODO: do best-placement? check best turn at 'current'.
                    var shiftedTour = solution.GetShiftedAfter(current, current2); // generates a tour as if current was placed right after current2.
                    var newFitness = objective.Calculate(problem, shiftedTour);

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(current, current2);
                        return true;
                    }
                    
                    position2++;
                    if (position2 >= position)
                    { // stop, we've reached the customer to place.
                        break;
                    }
                }

                position++;
            }

            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool MoveViolatedForward(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            if (_validFlags == null)
            {
                _validFlags = new bool[problem.Times.Length / 2];
            }

            float time, waitTime, violatedTime;
            int violated;
            var fitness = objective.Calculate(problem, solution, out violated, out violatedTime, out waitTime, out time, ref _validFlags);

            // if no violated customer found return false.
            if (violated == 0)
            {
                delta = 0;
                return false;
            }

            // loop over all customers.
            var enumerator = solution.GetEnumerator();
            var position = 0;
            while (enumerator.MoveNext())
            {
                if (position == 0)
                { // don't move the first customer.
                    position++;
                    continue;
                }

                // get the id of the current customer.
                var current = enumerator.Current;
                var id = DirectedHelper.ExtractId(current);

                if (problem.Last == id)
                { // don't place a fixed last customer.
                    position++;
                    continue;
                }

                // is this customer violated.
                if (_validFlags[id])
                { // no it's not, move on.
                    position++;
                    continue;
                }

                // move over all customers before the current position.
                var enumerator2 = solution.GetEnumerator();
                var position2 = 0;
                while (enumerator2.MoveNext())
                {
                    if (position2 <= position)
                    { // only consider placement after.
                        position2++;
                        continue;
                    }

                    var current2 = enumerator2.Current;
                    var id2 = DirectedHelper.ExtractId(current2);
                    if (problem.Last == id2 &&
                        problem.Last != problem.First)
                    { // don't place a fixed last customer.
                        position2++;
                        continue;
                    }

                    // test and check fitness.
                    // TODO: do best-placement? check best turn at 'current'.
                    var shiftedTour = solution.GetShiftedAfter(current, current2); // generates a tour as if current was placed right after current2.
                    var newFitness = objective.Calculate(problem, shiftedTour);

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(current, current2);
                        return true;
                    }

                    position2++;
                }

                position++;
            }
            delta = 0;
            return false;
        }

    }
}
