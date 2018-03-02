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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;
using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP.TimeWindows.Solvers.Operators
{
    /// <summary>
    /// A local search procedure to move around and improve the time window 'violations' in a solution.
    /// </summary>
    public class Local1Shift<TObjective> : IOperator<float, TSPTWProblem, TObjective, Tour, float>
        where TObjective : ObjectiveBase<TSPTWProblem, Tour, float>
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

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            var before = float.MaxValue;
            if (objective.IsNonContinuous)
            {
                before = objective.Calculate(problem, solution);
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
                    if (objective.IsNonContinuous)
                    {
                        delta = before - objective.Calculate(problem, solution);
                    }
                    return delta > 0;
                }
            }
            if (this.MoveNonViolatedForward(problem, objective, solution, out delta))
            { // success already, don't try anything else.
                if (objective.IsNonContinuous)
                {
                    delta = before - objective.Calculate(problem, solution);
                }
                return delta > 0;
            }
            if (this.MoveNonViolatedBackward(problem, objective, solution, out delta))
            { // success already, don't try anything else.
                if (objective.IsNonContinuous)
                {
                    delta = before - objective.Calculate(problem, solution);
                }
                return delta > 0;
            }
            if (!_assumeFeasible)
            {
                if (this.MoveViolatedForward(problem, objective, solution, out delta))
                { // success already, don't try anything else.
                    if (objective.IsNonContinuous)
                    {
                        delta = before - objective.Calculate(problem, solution);
                    }
                    return delta > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool MoveViolatedBackward(TSPTWProblem problem, TObjective objective, Tour solution, out float delta)
        {
            // search for invalid customers.
            var enumerator = solution.GetEnumerator();
            var time = 0.0f;
            var fitness = 0.0f;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Times[previous][current];
                }
                var window = problem.Windows[enumerator.Current];
                if (window.Max < time && position > 1)
                { // ok, unfeasible and customer is not the first 'moveable' customer.
                    fitness += time - window.Max;
                    if (enumerator.Current != problem.Last)
                    { // when the last customer is fixed, don't try to relocate.
                        invalids.Add(new Tuple<int, int>(enumerator.Current, position));
                    }
                }
                if (window.Min > time)
                { // wait here!
                    time = window.Min;
                }

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var invalid in invalids)
            {
                // ok try the new position.
                for (var newPosition = 1; newPosition < invalid.Item2; newPosition++)
                {
                    var before = solution.GetVisitAt(newPosition - 1);

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != invalid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Times[previous][current];
                            }
                            var window = problem.Windows[enumerator.Current];
                            if (window.Max < time)
                            { // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                            }
                            if (window.Min > time)
                            { // wait here!
                                time = window.Min;
                            }
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Times[current][invalid.Item1];
                                window = problem.Windows[invalid.Item1];
                                if (window.Max < time)
                                { // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // wait here!
                                    time = window.Min;
                                }
                                previous = invalid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(invalid.Item1, before);
                        return true;
                    }
                }
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
            // search for invalid customers.
            var enumerator = solution.GetEnumerator();
            var time = 0f;
            var fitness = 0f;
            var position = 0;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Times[previous][current];
                }
                var window = problem.Windows[enumerator.Current];
                if (window.Max < time)
                { // ok, unfeasible.
                    fitness += time - window.Max;
                }
                else if (position > 0 && position < problem.Times.Length - 1)
                { // window is valid and customer is not the first 'moveable' customer.
                    if (enumerator.Current != problem.Last)
                    { // when the last customer is fixed, don't try to relocate.
                        valids.Add(new Tuple<int, int>(enumerator.Current, position));
                    }
                }
                if (window.Min > time)
                { // wait here!
                    time = window.Min;
                }

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = valid.Item2 + 1; newPosition < problem.Times.Length; newPosition++)
                {
                    var before = solution.GetVisitAt(newPosition);

                    if (before == problem.Last)
                    { // cannot move a customer after a fixed last customer.
                        continue;
                    }
                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != valid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Times[previous][current];
                            }
                            var window = problem.Windows[enumerator.Current];
                            if (window.Max < time)
                            { // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                                if (_assumeFeasible)
                                {
                                    newFitness = float.MaxValue;
                                    break;
                                }
                            }
                            if (window.Min > time)
                            { // wait here!
                                time = window.Min;
                            }
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Times[current][valid.Item1];
                                window = problem.Windows[valid.Item1];
                                if (window.Max < time)
                                { // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // wait here!
                                    time = window.Min;
                                }
                                previous = valid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(valid.Item1, before);
                        return true;
                    }
                }
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
            // search for invalid customers.
            var enumerator = solution.GetEnumerator();
            var time = 0f;
            var fitness = 0f;
            var position = 0;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Times[previous][current];
                }
                var window = problem.Windows[enumerator.Current];
                if (window.Max < time)
                { // ok, unfeasible.
                    fitness += time - window.Max;
                }
                else if (position > 1)
                { // window is valid and customer is not the first 'moveable' customer.
                    if (enumerator.Current != problem.Last)
                    { // when the last customer is fixed, don't try to relocate.
                        valids.Add(new Tuple<int, int>(enumerator.Current, position));
                    }
                }
                if (window.Min > time)
                { // wait here!
                    time = window.Min;
                }

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = 1; newPosition < valid.Item2; newPosition++)
                {
                    var before = solution.GetVisitAt(newPosition - 1);

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != valid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Times[previous][current];
                            }
                            var window = problem.Windows[enumerator.Current];
                            if (window.Max < time)
                            { // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                                if (_assumeFeasible)
                                {
                                    newFitness = float.MaxValue;
                                    break;
                                }
                            }
                            if (window.Min > time)
                            { // wait here!
                                time = window.Min;
                            }
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Times[current][valid.Item1];
                                window = problem.Windows[valid.Item1];
                                if (window.Max < time)
                                { // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // wait here!
                                    time = window.Min;
                                }
                                previous = valid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(valid.Item1, before);
                        return true;
                    }
                }
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
            // search for invalid customers.
            var enumerator = solution.GetEnumerator();
            var time = 0f;
            var fitness = 0f;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Constants.NOT_SET;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Times[previous][current];
                }
                var window = problem.Windows[enumerator.Current];
                if (window.Max < time && position > 0 && position < problem.Times.Length - 1)
                { // ok, unfeasible and customer is not the first 'moveable' customer.
                    fitness += time - window.Max;
                    if (enumerator.Current != problem.Last)
                    { // when the last customer is fixed, don't try to relocate.
                        invalids.Add(new Tuple<int, int>(enumerator.Current, position));
                    }
                }
                if (window.Min > time)
                { // wait here!
                    time = window.Min;
                }

                // increase position.
                position++;
                previous = enumerator.Current;
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var invalid in invalids)
            {
                // ok try the new position.
                for (var newPosition = invalid.Item2 + 1; newPosition < problem.Times.Length; newPosition++)
                {
                    var before = solution.GetVisitAt(newPosition);
                    if (before == problem.Last)
                    { // cannot move a customer after a fixed last customer.
                        continue;
                    }

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Constants.NOT_SET;
                    time = 0;
                    enumerator = solution.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (current != invalid.Item1)
                        { // ignore invalid, add it after 'before'.
                            if (previous != Constants.NOT_SET)
                            { // keep track if time.
                                time += problem.Times[previous][current];
                            }
                            var window = problem.Windows[enumerator.Current];
                            if (window.Max < time)
                            { // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                            }
                            if (window.Min > time)
                            { // wait here!
                                time = window.Min;
                            }
                            previous = current;
                            if (current == before)
                            { // also add the before->invalid.
                                time += problem.Times[current][invalid.Item1];
                                window = problem.Windows[invalid.Item1];
                                if (window.Max < time)
                                { // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // wait here!
                                    time = window.Min;
                                }
                                previous = invalid.Item1;
                            }
                        }
                    }

                    if (newFitness < fitness)
                    { // there is improvement!
                        delta = fitness - newFitness;
                        solution.ShiftAfter(invalid.Item1, before);
                        return true;
                    }
                }
            }
            delta = 0;
            return false;
        }

    }
}
