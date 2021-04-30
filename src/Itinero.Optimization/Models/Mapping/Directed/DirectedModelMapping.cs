using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Solvers.Shared.Directed;

namespace Itinero.Optimization.Models.Mapping.Directed
{
    /// <summary>
    /// A default mapping.
    /// </summary>
    internal class DirectedModelMapping : IModelMapping
    {
        private readonly MappedModel _mappedModel;
        private readonly DirectedWeightMatrixAlgorithm _weightMatrixAlgorithm;

        public DirectedModelMapping(MappedModel mappedModel, DirectedWeightMatrixAlgorithm weightMatrixAlgorithm)
        {
            _mappedModel = mappedModel;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        /// <inheritdoc />
        public IEnumerable<Result<Route>> BuildRoutes(IEnumerable<(int vehicle, IEnumerable<int> tour)> solution)
        {
            foreach (var vehicleAndTour in solution)
            {
                yield return this.BuildRoute(vehicleAndTour);
            }
        }

        private Result<Route> BuildRoute((int vehicle, IEnumerable<int> tour) vehicleAndTour)
        {
            try
            {
                // TODO: we need a better more generic way of handling this. Current rules are:
                // - use vehicle departure and arrival to close a tour or not.
                // - close a tour by default when both departure and arrival are null.
                // PROBLEM: this doesn't support the case where we have a vehicle with arrival and departure null and we generate open-tours.
                // SUGGESTED FIX: look at the enumerable differently include all visits, even if that means closing the tour in the enumerable.
                var vehicle = _mappedModel.VehiclePool.Vehicles[vehicleAndTour.vehicle];
                
                Route route = null;
                var index = -1;
                var previous = -1;
                var first = -1;
                foreach (var v in vehicleAndTour.tour)
                {
                    index++;

                    if (first < 0)
                    {
                        first = v;
                    }
                    if (previous < 0)
                    {
                        previous = v;
                        continue;
                    }

                    var localResult = AppendRoute(route, index - 1, previous, index, v);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }

                    route = localResult.Value;
                    previous = v;
                }
                
                // 3 options:
                // OPEN:
                //   when:
                //     -> vehicle.arrival is not set and vehicle.departure is set.
                //   action: 
                //     -> none.
                // FIXED:
                //   when:
                //     -> vehicle.arrival is set and vehicle.departure is set but to different visits.
                //   action:
                //     -> none, all visits should be there.
                // CLOSED:
                //   when:
                //     ->  vehicle.arrival and vehicle.departure is not set is treated as closed.
                //     ->  vehicle.arrival and vehicle.departure are set but both to the same visit and 
                //            it matches the first directed visit.
                //   action:
                //     ->  add connection between the last directed visit and the first directed visit and close the tour.
                
                if (vehicle.Arrival.HasValue &&
                    vehicle.Departure.HasValue &&
                    vehicle.Arrival == vehicle.Departure &&
                    previous != 0)
                {
                    if (DirectedHelper.ExtractVisit(first) != vehicle.Arrival.Value)
                    {
                        throw new Exception($"Vehicle should match a tour starting at {vehicle.Departure} but the start of the tour is at:" +
                                            $"{DirectedHelper.ExtractVisit(first)}");
                    }
                    var localResult = AppendRoute(route, index, previous,  0, first);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }
                    
                    route = localResult.Value;
                }

                if (!vehicle.Arrival.HasValue &&
                    !vehicle.Departure.HasValue &&
                    previous != 0)
                {
                    var localResult = AppendRoute(route, index, previous, 0, first);
                    if (localResult.IsError)
                    {
                        return localResult;
                    }
                    
                    route = localResult.Value;
                }
                
                return new Result<Route>(route);
            }
            catch (Exception e)
            {
                return new Result<Route>(e.Message);
            }
        }

        private Result<Route> AppendRoute(Route route, int directedVisit1Index, int directedVisit1, int directedVisit2Index, int directedVisit2)
        {
            var weightHandler = _weightMatrixAlgorithm.Profile.DefaultWeightHandler(_weightMatrixAlgorithm.Router);
            var pairFromDepartureId = _weightMatrixAlgorithm.SourcePaths[DirectedHelper.WeightIdDeparture(directedVisit1)];
            var pairToArrivalId = _weightMatrixAlgorithm.TargetPaths[DirectedHelper.WeightIdArrival(directedVisit2)];
                
            if (pairFromDepartureId == null)
            {
                return new Result<Route>($"No source path found at departure id for {directedVisit1}->{directedVisit2}.");
            }                
            if (pairToArrivalId == null)
            {
                return new Result<Route>($"No target path found at arrival id for {directedVisit1}->{directedVisit2}.");
            }
            
            var pairFromEdgeId = _weightMatrixAlgorithm.Router.Db.Network.GetEdges(pairFromDepartureId.From.Vertex).First(x => x.To == pairFromDepartureId.Vertex).IdDirected();
            var pairToEdgeId = _weightMatrixAlgorithm.Router.Db.Network.GetEdges(pairToArrivalId.Vertex).First(x => x.To == pairToArrivalId.From.Vertex).IdDirected();

            var visit1 = DirectedHelper.ExtractVisit(directedVisit1);
            var visit2 = DirectedHelper.ExtractVisit(directedVisit2);
            
            var visit1Stop = _weightMatrixAlgorithm.RouterPoints[visit1];
            var visit2Stop = _weightMatrixAlgorithm.RouterPoints[visit2];

            visit1Stop.Attributes.AddOrReplace("order", directedVisit1Index.ToInvariantString());
            visit2Stop.Attributes.AddOrReplace("order", directedVisit2Index.ToInvariantString());
            
            var visit1TimeWindow = _mappedModel.Visits[visit1].TimeWindow;
            if (visit1TimeWindow != null && !visit1TimeWindow.IsEmpty)
            {
                visit1Stop.Attributes.AddOrReplace("time_window", visit1TimeWindow.ToJsonArray());
            }
            
            var visit2TimeWindow = _mappedModel.Visits[visit2].TimeWindow;
            if (visit2TimeWindow != null && !visit2TimeWindow.IsEmpty)
            {
                visit2Stop.Attributes.AddOrReplace("time_window", visit2TimeWindow.ToJsonArray());
            }
            
            var localRouteRawResult = _weightMatrixAlgorithm.Router.TryCalculateRaw(_weightMatrixAlgorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null);
            if (localRouteRawResult.IsError)
            {
                return localRouteRawResult.ConvertError<Route>();
            }

            var localRouteRaw = localRouteRawResult.Value;
            localRouteRaw.StripSource();
            localRouteRaw.StripTarget();

            var localRoute = _weightMatrixAlgorithm.Router.BuildRoute(_weightMatrixAlgorithm.Profile, weightHandler, visit1Stop, visit2Stop, localRouteRaw);
            
            route = route == null ? localRoute.Value : route.Concatenate(localRoute.Value);
            return new Result<Route>(route);
        }

        /// <inheritdoc />
        public IEnumerable<(int visit, string message)> Errors
        {
            get
            {
                if (_weightMatrixAlgorithm.Errors != null)
                {
                    foreach (var error in _weightMatrixAlgorithm.Errors)
                    {
                        yield return (error.Key, $"{error.Value.Code} - {error.Value.Message}");
                    }
                }
            }
        }
    }
}