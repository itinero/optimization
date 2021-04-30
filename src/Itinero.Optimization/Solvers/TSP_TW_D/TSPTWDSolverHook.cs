using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Logging;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers.Shared;

namespace Itinero.Optimization.Solvers.TSP_TW_D
{
    /// <summary>
    /// Hooks the TSP-TW-D solvers into the solver registry.
    /// </summary>
    internal static class TSPTWDSolverHook
    {
        /// <summary>
        /// The default solver hook.
        /// </summary>
        public static readonly SolverRegistry.SolverHook Default = new SolverRegistry.SolverHook()
        {
            Name = "TSP-TW-D",
            TrySolve = TSPTWDSolverHook.Solve
        };

        private static Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>> Solve(MappedModel model, 
            Action<IEnumerable<(int vehicle, IEnumerable<int>)>> intermediateResult)
        {
            var tsptw = model.TryToTSPTWD();
            if (tsptw.IsError)
            {
                return tsptw.ConvertError<IEnumerable<(int vehicle, IEnumerable<int> tour)>>();
            }
            
            // use a default solver to solve the TSP-TW here.
            try
            {
                var solution = EAX.EAXSolver.Default.Search(tsptw.Value);

                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>(
                    (new (int vehicle, IEnumerable<int> tour) [] { (0, solution.Solution) }));
            }
            catch (Exception ex)
            {
                Logger.Log($"{nameof(TSPTWDSolverHook)}.{nameof(Solve)}", TraceEventType.Critical, $"Unhandled exception: {ex.ToString()}.");
                return new Result<IEnumerable<(int vehicle, IEnumerable<int> tour)>>($"Unhandled exception: {ex.ToString()}.");
            }
        }

        /// <summary>
        /// Converts the given abstract model to a TSP-TW.
        /// </summary>
        internal static Result<TSPTWDProblem> TryToTSPTWD(this MappedModel model)
        {
            if (!model.IsTSPTWD(out var reasonWhenFailed))
            {
                return new Result<TSPTWDProblem>($"Model is not a TSP-TW-D: {reasonWhenFailed}");
            }

            var vehicle = model.VehiclePool.Vehicles[0];
            var metric = vehicle.Metric;
            if (!model.TryGetTravelCostsForMetric(metric, out var weights))
            {
                throw new Exception("Travel costs not found but model was declared valid.");
            }
            var first = 0;
            int? last = null;
            if (vehicle.Departure.HasValue)
            {
                first = vehicle.Departure.Value;
            }
            if (vehicle.Arrival.HasValue)
            {
                last = vehicle.Arrival.Value;
            }

            var windows = new TimeWindow[model.Visits.Length];
            for (var v = 0; v < windows.Length; v++)
            {
                var visit = model.Visits[v];
                
                windows[v] = new TimeWindow();
                if (visit.TimeWindow is {IsEmpty:false})
                {
                    windows[v] = new TimeWindow()
                    {
                        Times = visit.TimeWindow.Times
                    };
                }
            }
            return new Result<TSPTWDProblem>(new TSPTWDProblem(first, last, weights.Costs, vehicle.TurnPentalty, windows));
        }

        private static bool IsTSPTWD(this MappedModel model, out string reasonIfNot)
        {
            if (model.VehiclePool.Reusable ||
                model.VehiclePool.Vehicles.Length > 1)
            {
                reasonIfNot = "More than one vehicle or vehicle reusable.";
                return false;
            }
            var vehicle = model.VehiclePool.Vehicles[0];
            if (vehicle.TurnPentalty == 0)
            {
                reasonIfNot = "No turning penalty, this is an undirected problem.";
                return false;
            }
            if (vehicle.CapacityConstraints != null &&
                vehicle.CapacityConstraints.Length > 0)
            {
                reasonIfNot = "At least one capacity constraint was found.";
                return false;
            }

            var timeWindow = model.Visits.Any(visit => visit.TimeWindow is {IsEmpty: false});
            if (!timeWindow)
            {
                reasonIfNot = "No time windows detected.";
                return false;
            }

            reasonIfNot = string.Empty;
            return true;
        }
    }
}