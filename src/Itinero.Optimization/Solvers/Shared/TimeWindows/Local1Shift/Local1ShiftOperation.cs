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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.TimeWindows.Local1Shift
{
    internal static class Local1ShiftOperation
    {
        // TODO: performance test all this stuff.

        /// <summary>
        /// Tries to move a visit to improve fitness or violations in the time windows.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>All details about the move.</returns>
        public static (bool success, int shifted, int oldBefore,
            int oldAfter, int newBefore, int newAfter) TryShift(this Tour tour,
                Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            var moveDetails = tour.MoveViolatedBackward(weightFunc, windows);
            if (moveDetails.success)
            {
                return moveDetails;
            }
            
            moveDetails = tour.MoveNonViolatedForward(weightFunc, windows);
            if (moveDetails.success)
            {
                return moveDetails;
            }
            
            moveDetails = tour.MoveNonViolatedBackward(weightFunc, windows);
            if (moveDetails.success)
            {
                return moveDetails;
            }
            
            return tour.MoveViolatedForward(weightFunc, windows);
        }
        
        /// <summary>
        /// Tries to move violated time windows backwards until they are not.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>All details about the move.</returns>
        public static (bool success, int shifted, int oldBefore,
            int oldAfter, int newBefore, int newAfter) MoveViolatedBackward(this Tour tour,
                Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            // search for invalid customers.
            var time = 0.0f;
            var fitness = 0.0f;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Tour.NOT_SET;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        time += weightFunc(previous, current);
                    }

                    var window = windows[enumerator.Current];
                    if (!window.IsEmpty)
                    {
                        if (window.Max < time && position > 1)
                        {
                            // ok, unfeasible and customer is not the first 'moveable' customer.
                            fitness += time - window.Max;
                            if (enumerator.Current != tour.Last)
                            {
                                // when the last customer is fixed, don't try to relocate.
                                invalids.Add(new Tuple<int, int>(enumerator.Current, position));
                            }
                        }

                        if (window.Min > time)
                        {
                            // wait here!
                            time = window.Min;
                        }
                    }

                    // increase position.
                    position++;
                    previous = enumerator.Current;
                }
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var invalid in invalids)
            {
                // ok try the new position.
                for (var newPosition = 1; newPosition < invalid.Item2; newPosition++)
                {
                    var before = tour.GetVisitAt(newPosition - 1);

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Tour.NOT_SET;
                    time = 0;
                    using (var enumerator = tour.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current == invalid.Item1) continue;

                            // ignore invalid, add it after 'before'.
                            if (previous != Tour.NOT_SET)
                            {
                                // keep track if time.
                                time += weightFunc(previous, current);
                            }

                            var window = windows[enumerator.Current];
                            if (!window.IsEmpty)
                            {
                                if (window.Max < time)
                                {
                                    // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }

                                if (window.Min > time)
                                {
                                    // wait here!
                                    time = window.Min;
                                }
                            }

                            previous = current;
                            if (current != before) continue;

                            // also add the before->invalid.
                            time += weightFunc(current, invalid.Item1);
                            window = windows[invalid.Item1]; // always has a non-empty window, otherwise impossible to be invalid.
                            if (window.Max < time)
                            {
                                // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                            }

                            if (window.Min > time)
                            {
                                // wait here!
                                time = window.Min;
                            }

                            previous = invalid.Item1;
                        }
                    }

                    if (!(newFitness < fitness)) continue;

                    // there is improvement!
                    var result = tour.ShiftAfter(invalid.Item1, before, out var oldBefore, out var oldAfter,
                        out var newAfter);
                    Debug.Assert(result, "Shift after failed, should always succeed");
                    return (true, invalid.Item1, oldBefore, oldAfter, before, newAfter);
                }
            }

            return (false, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET);
        }

        /// <summary>
        /// Tries to move non-violated time windows forward.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>All details about the move.</returns>
        public static (bool success, int shifted, int oldBefore,
            int oldAfter, int newBefore, int newAfter) MoveNonViolatedForward(this Tour tour,
                Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            var previous = Tour.NOT_SET;
            var time = 0f;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var fitness = 0f;
            var position = 0;

            // search for invalid customers.
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        time += weightFunc(previous, current);
                    }

                    var window = windows[enumerator.Current];
                    if (!window.IsEmpty && window.Max < time)
                    {
                        // ok, unfeasible.
                        fitness += time - window.Max;
                    }
                    else if (position > 0 && position < windows.Length - 1)
                    {
                        // window is valid and customer is not the first 'moveable' customer.
                        if (enumerator.Current != tour.Last)
                        {
                            // when the last customer is fixed, don't try to relocate.
                            valids.Add(new Tuple<int, int>(enumerator.Current, position));
                        }
                    }

                    if (!window.IsEmpty && window.Min > time)
                    {
                        // wait here!
                        time = window.Min;
                    }

                    // increase position.
                    position++;
                    previous = enumerator.Current;
                }
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = valid.Item2 + 1; newPosition < windows.Length; newPosition++)
                {
                    var before = tour.GetVisitAt(newPosition);

                    if (before == tour.Last)
                    {
                        // cannot move a customer after a fixed last customer.
                        continue;
                    }

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Tour.NOT_SET;
                    time = 0;
                    using (var enumerator = tour.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current == valid.Item1) continue;

                            // ignore invalid, add it after 'before'.
                            if (previous != Tour.NOT_SET)
                            {
                                // keep track if time.
                                time += weightFunc(previous, current);
                            }

                            var window = windows[enumerator.Current];
                            if (!window.IsEmpty)
                            {
                                if (window.Max < time)
                                { // apply penaly if unfeasible.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // add waiting time.
                                    time = window.Min;
                                }
                            }

                            previous = current;
                            if (current != before) continue;

                            // also add the before->invalid.
                            time += weightFunc(current, valid.Item1);
                            window = windows[valid.Item1];
                            if (!window.IsEmpty)
                            {
                                if (window.Max < time)
                                { // apply penaly if unfeasible.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                { // add waiting time.
                                    time = window.Min;
                                }
                            }

                            previous = valid.Item1;
                        }
                    }

                    if (!(newFitness < fitness)) continue; // there is improvement!

                    // there is improvement!
                    var result = tour.ShiftAfter(valid.Item1, before, out var oldBefore, out var oldAfter,
                        out var newAfter);
                    Debug.Assert(result, "Shift after failed, should always succeed");
                    return (true, valid.Item1, oldBefore, oldAfter, before, newAfter);
                }
            }

            return (false, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET);
        }

        /// <summary>
        /// Tries to move non-violated time windows backward.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>All details about the move.</returns>
        public static (bool success, int shifted, int oldBefore,
            int oldAfter, int newBefore, int newAfter) MoveNonViolatedBackward(this Tour tour,
                Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            // search for invalid customers.
            var time = 0f;
            var fitness = 0f;
            var position = 0;
            var valids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Tour.NOT_SET;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        time += weightFunc(previous, current);
                    }

                    var window = windows[enumerator.Current];
                    if (!window.IsEmpty && window.Max < time)
                    {
                        // ok, unfeasible.
                        fitness += time - window.Max;
                    }
                    else if (position > 1)
                    {
                        // window is valid and customer is not the first 'moveable' customer.
                        if (enumerator.Current != tour.Last)
                        {
                            // when the last customer is fixed, don't try to relocate.
                            valids.Add(new Tuple<int, int>(enumerator.Current, position));
                        }
                    }

                    if (!window.IsEmpty && window.Min > time)
                    {
                        // wait here!
                        time = window.Min;
                    }

                    // increase position.
                    position++;
                    previous = enumerator.Current;
                }
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var valid in valids)
            {
                // ok try the new position.
                for (var newPosition = 1; newPosition < valid.Item2; newPosition++)
                {
                    var before = tour.GetVisitAt(newPosition - 1);

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Tour.NOT_SET;
                    time = 0;
                    using (var enumerator = tour.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current == valid.Item1) continue;
                            
                            // ignore invalid, add it after 'before'.
                            if (previous != Tour.NOT_SET)
                            {
                                // keep track if time.
                                time += weightFunc(previous, current);
                            }

                            var window = windows[enumerator.Current];
                            if (!window.IsEmpty)
                            {
                                if (window.Max < time)
                                {
                                    // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }
                                if (window.Min > time)
                                {
                                    // wait here!
                                    time = window.Min;
                                }
                            }
                            

                            previous = current;
                            if (current != before) continue;
                                
                            // also add the before->invalid.
                            time += weightFunc(current, valid.Item1);
                            window = windows[valid.Item1];
                            if (!window.IsEmpty)
                            {
                                if (window.Max < time)
                                {
                                    // ok, unfeasible and customer is not the first 'moveable' customer.
                                    newFitness += time - window.Max;
                                }

                                if (window.Min > time)
                                {
                                    // wait here!
                                    time = window.Min;
                                }
                            }

                            previous = valid.Item1;
                        }
                    }

                    if (!(newFitness < fitness)) continue;
                    
                    // there is improvement!
                    var result = tour.ShiftAfter(valid.Item1, before, out var oldBefore, out var oldAfter,
                        out var newAfter);
                    Debug.Assert(result, "Shift after failed, should always succeed");
                    return (true, valid.Item1, oldBefore, oldAfter, before, newAfter);
                }
            }

            return (false, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET);
        }

            /// <summary>
        /// Tries to move violated time windows forward.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <param name="last">The last visit if fixed, it will no be considered as a move.</param>
        /// <returns>All details about the move.</returns>
        public static (bool success, int shifted, int oldBefore,
            int oldAfter, int newBefore, int newAfter) MoveViolatedForward(this Tour tour,
                Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            // search for invalid customers.
            var time = 0f;
            var fitness = 0f;
            var position = 0;
            var invalids = new List<Tuple<int, int>>(); // al list of customer-position pairs.
            var previous = Tour.NOT_SET;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        time += weightFunc(previous, current);
                    }

                    var window = windows[enumerator.Current];
                    if (window.Max < time && position > 0 && position < windows.Length - 1)
                    {
                        // ok, unfeasible and customer is not the first 'moveable' customer.
                        fitness += time - window.Max;
                        if (enumerator.Current != tour.Last)
                        {
                            // when the last customer is fixed, don't try to relocate.
                            invalids.Add(new Tuple<int, int>(enumerator.Current, position));
                        }
                    }

                    if (window.Min > time)
                    {
                        // wait here!
                        time = window.Min;
                    }

                    // increase position.
                    position++;
                    previous = enumerator.Current;
                }
            }

            // ... ok, if a customer was found, try to move it.
            foreach (var invalid in invalids)
            {
                // ok try the new position.
                for (var newPosition = invalid.Item2 + 1; newPosition < windows.Length; newPosition++)
                {
                    var before = tour.GetVisitAt(newPosition);
                    if (before == tour.Last)
                    {
                        // cannot move a customer after a fixed last customer.
                        continue;
                    }

                    // calculate new total min diff.
                    var newFitness = 0.0f;
                    previous = Tour.NOT_SET;
                    time = 0;
                    using (var enumerator = tour.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current == invalid.Item1) continue;
                            
                            // ignore invalid, add it after 'before'.
                            if (previous != Tour.NOT_SET)
                            {
                                // keep track if time.
                                time += weightFunc(previous, current);
                            }

                            var window = windows[enumerator.Current];
                            if (window.Max < time)
                            {
                                // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                            }

                            if (window.Min > time)
                            {
                                // wait here!
                                time = window.Min;
                            }

                            previous = current;
                            if (current != before) continue;
                            
                            // also add the before->invalid.
                            time += weightFunc(current, invalid.Item1);
                            window = windows[invalid.Item1];
                            if (window.Max < time)
                            {
                                // ok, unfeasible and customer is not the first 'moveable' customer.
                                newFitness += time - window.Max;
                            }

                            if (window.Min > time)
                            {
                                // wait here!
                                time = window.Min;
                            }

                            previous = invalid.Item1;
                        }
                    }

                    if (!(newFitness < fitness)) continue;
                    
                    // there is improvement!
                    var result = tour.ShiftAfter(invalid.Item1, before, out var oldBefore, out var oldAfter,
                        out var newAfter);
                    Debug.Assert(result, "Shift after failed, should always succeed");
                    return (true, invalid.Item1, oldBefore, oldAfter, before, newAfter);
                }
            }

            return (false, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET, Tour.NOT_SET);
        }
    }
}