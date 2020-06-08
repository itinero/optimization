using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt
{
    internal static class Local2OptOperation
    {
        /// <summary>
        /// Runs a local 2-opt search, 
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The function to get weights.</param>
        /// <param name="windows">The time windows.</param>
        /// <returns>True if the operation succeeded and an improvement was found.</returns>
        /// <remarks>* 2-opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
        public static bool Do2Opt(this Tour tour, Func<int, int, float> weightFunc, TimeWindow[] windows)
        {
            var customers = new List<int>(windows.Length + 1);
            customers.AddRange(tour);
            if (tour.Last == tour.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(tour.Last.Value);
            }

            var weight12 = 0.0f;
            if (windows[customers[0]].Min > weight12)
            { // wait here!
                weight12 = windows[customers[0]].Min;
            }
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1];
                var edge12 = customers[edge1 + 1];

                var weight11 = weight12;
                weight12 += weightFunc(edge11, edge12);
                if (windows[edge12].Min > weight12)
                { // wait here!
                    weight12 = windows[edge12].Min;
                }

                float betweenForward = 0;
                for (var edge2 = edge1 + 2; edge2 < customers.Count - 1; edge2++)
                {
                    // iterate over all to-edges.
                    var edge20 = customers[edge2 - 1];
                    var edge21 = customers[edge2];
                    var edge22 = customers[edge2 + 1];

                    // calculate existing value of the part 11->21->(reverse)->12->22.
                    // @ 22: no need to take minimum of window into account, is valid now, will stay valid on reduction of arrival-time.
                    // completely re-calculate between-backward (because window min may be violated) and determine feasible at the same time.
                    var feasible = true;
                    var currentWeight = weight11 + weightFunc(edge11, edge21);
                    var edge21Windows = windows[edge21];
                    if (!edge21Windows.IsEmpty && 
                        edge21Windows.Min > currentWeight)
                    { // wait here!
                        currentWeight = edge21Windows.Min;
                    }

                    var previous = edge21;
                    for (var i = edge2 - 1; i > edge1; i--)
                    {
                        var current = customers[i];
                        var currentWindow = windows[current];
                        currentWeight += weightFunc(previous, current);
                        if (!currentWindow.IsEmpty)
                        {
                            if (currentWindow.Min > currentWeight)
                            { // wait here!
                                currentWeight = currentWindow.Min;
                            }

                            if (currentWindow.Max < currentWeight)
                            { // unfeasible.
                                feasible = false;
                                break;
                            }
                        }

                        previous = current;
                    }

                    var potential = currentWeight + weightFunc(edge12, edge22);

                    if (!feasible) continue;

                    // new reverse is feasible.
                    // calculate existing value of the part 11->12->...->21->22.
                    // @ 22: no need to take minimum of window into account, is valid now, will stay valid on reduction of arrival-time.
                    betweenForward += weightFunc(edge20, edge21);
                    if (betweenForward + weight12 < windows[edge21].Min)
                    {
                        // take into account minimum-window constraint.
                        betweenForward = windows[edge21].Min - weight12;
                    }

                    var existing = weight12 + betweenForward + weightFunc(edge21, edge22);
                    if (!(existing > potential)) continue; 
                    
                    // we found an improvement.
                    tour.ReplaceEdgeFrom(edge11, edge21);
                    tour.ReplaceEdgeFrom(edge12, edge22);
                    for (var i = edge1 + 1; i < edge2; i++)
                    {
                        tour.ReplaceEdgeFrom(customers[i + 1], customers[i]);
                    }

                    return true;
                }
            }
            return false;
        }
    }
}