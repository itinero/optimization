using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Optimization.Models.Simplification.ByEdge;
using Itinero.Optimization.Solvers.Shared.Directed;

namespace Itinero.Optimization.Models.Mapping.Directed.Simplified
{
    internal class ByEdgeDirectedModelMapping: IModelMapping
    {
        private readonly MappedModel _mappedModel;
        private readonly DirectedWeightMatrixAlgorithm _weightMatrixAlgorithm;
        private readonly EdgeBasedSimplification _edgeBasedSimplification;

        public ByEdgeDirectedModelMapping(MappedModel mappedModel, DirectedWeightMatrixAlgorithm weightMatrixAlgorithm,
            EdgeBasedSimplification edgeBasedSimplification)
        {
            _mappedModel = mappedModel;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _edgeBasedSimplification = edgeBasedSimplification;
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
                var first = -1;
                var index = 0;
                RouterPoint previousRouterPoint = null;
                var previous = -1;
                foreach (var v in vehicleAndTour.tour)
                {   
                    // gets the visits on the current segment.
                    var visits = this.FindOriginalVisits(v);
                    
                    // builds the segment route if needed.
                    Route segmentRoute = null;
                    if (visits.Count > 1)
                    {
                        segmentRoute = this.RouteAlongOriginalVisits(visits, index);
                    }
                    
                    if (first < 0)
                    {
                        route = segmentRoute;
                        first = v;
                    }
                    else
                    {
                        // append route from previous segment to first arrival location in current segment..
                        var arrivalRouterPoint = visits[0].routerPoint;
                        var localResult = this.AppendRoute(route, previousRouterPoint, index, previous,
                            arrivalRouterPoint, index, v);
                        if (localResult.IsError)
                        {
                            return localResult;
                        }
                        route = localResult.Value;

                        // append local segment route if any.
                        if (segmentRoute != null)
                        { 
                            route = route.Concatenate(segmentRoute);
                        }
                    }

                    // keep some data on the previous segment.
                    previousRouterPoint = visits[visits.Count - 1].routerPoint;
                    previous = v;
                    index += visits.Count;
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
                
//                if (vehicle.Arrival.HasValue &&
//                    vehicle.Departure.HasValue &&
//                    vehicle.Arrival == vehicle.Departure &&
//                    previous != 0)
//                { // CLOSED.
//                    if (DirectedHelper.ExtractVisit(first) != vehicle.Arrival.Value)
//                    {
//                        throw new Exception($"Vehicle should match a tour starting at {vehicle.Departure} but the start of the tour is at:" +
//                                            $"{DirectedHelper.ExtractVisit(first)}");
//                    }
//                    var localResult = AppendRoute(route, index, previous,  0, first);
//                    if (localResult.IsError)
//                    {
//                        return localResult;
//                    }
//                    
//                    route = localResult.Value;
//                }
//
//                if (!vehicle.Arrival.HasValue &&
//                    !vehicle.Departure.HasValue &&
//                    previous != 0)
//                { // CLOSED.
//                    var localResult = AppendRoute(route, index, previous, 0, first);
//                    if (localResult.IsError)
//                    {
//                        return localResult;
//                    }
//                    
//                    route = localResult.Value;
//                }
                
                return new Result<Route>(route);
            }
            catch (Exception e)
            {
                return new Result<Route>(e.Message);
            }
        }

        private List<(int visit, RouterPoint routerPoint)> FindOriginalVisits(int simplifiedVisitAndTurn)
        {
            var simplifiedVisit = DirectedHelper.ExtractVisit(simplifiedVisitAndTurn);
            var simplifiedVisitRouterPointer = _weightMatrixAlgorithm.RouterPoints[simplifiedVisit];
            var simplifiedVisitResolved = _weightMatrixAlgorithm.OriginalIndexOf(simplifiedVisit);
            
            // find all original visits on the same edge.
            var originalVisits = new List<(int visit, RouterPoint routerPoint)>();
            for (var originalVisit = 0; originalVisit < _edgeBasedSimplification.ToMapping.Count; originalVisit++)
            {
                var v = _edgeBasedSimplification.ToMapping[originalVisit];
                if (v != simplifiedVisitResolved) continue;

                originalVisits.Add((originalVisit,
                    _edgeBasedSimplification.MassResolvingAlgorithm.RouterPoints[originalVisit]));
            }

            var forward = true;
            switch (DirectedHelper.ExtractTurn(simplifiedVisitAndTurn))
            {
                case TurnEnum.BackwardBackward:
                case TurnEnum.BackwardForward:
                    forward = false;
                    break;
                case TurnEnum.ForwardForward:
                case TurnEnum.ForwardBackward:
                default:
                        break;
            }
            
            // TODO: figure out how to sort these.
            originalVisits.Sort((v1, v2) =>
            {
                if (!forward)
                {
                    return v1.routerPoint.Offset.CompareTo(v2.routerPoint.Offset);
                }

                return -v1.routerPoint.Offset.CompareTo(v2.routerPoint.Offset);
            });

            return originalVisits;
        }

        private Route RouteAlongOriginalVisits(List<(int visit, RouterPoint routerPoint)> routerPoints, int startIndex)
        {
            Route route = null;
            for (var i = 1; i < routerPoints.Count; i++)
            {
                var localRoute = _weightMatrixAlgorithm.Router.Calculate(_weightMatrixAlgorithm.Profile,
                    routerPoints[i - 1].routerPoint, routerPoints[i].routerPoint);
                
                localRoute.SetOrderAndIndex(routerPoints[i - 1].visit, (i - 1) + startIndex,
                    routerPoints[i].visit, (i) + startIndex);
                
                route = route == null ? localRoute : route.Concatenate(localRoute);
            }

            return route;
        }

        private Result<Route> AppendRoute(Route route, RouterPoint visit1RouterPoint, int directedVisit1Index, int directedVisit1, 
            RouterPoint visit2RouterPoint, int directedVisit2Index, int directedVisit2)
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

            var pairFromId = DirectedHelper.ExtractVisit(directedVisit1);
            var pairToId = DirectedHelper.ExtractVisit(directedVisit2);

            visit1RouterPoint.Attributes.AddOrReplace("order", directedVisit1Index.ToInvariantString());
            visit2RouterPoint.Attributes.AddOrReplace("order", directedVisit2Index.ToInvariantString());
            
            var localRouteRawResult = _weightMatrixAlgorithm.Router.TryCalculateRaw(_weightMatrixAlgorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null);
            if (localRouteRawResult.IsError)
            {
                return localRouteRawResult.ConvertError<Route>();
            }

            var localRouteRaw = localRouteRawResult.Value;
            localRouteRaw.StripSource();
            localRouteRaw.StripTarget();

            var localRoute = _weightMatrixAlgorithm.Router.BuildRoute(_weightMatrixAlgorithm.Profile, weightHandler, visit1RouterPoint, visit2RouterPoint, localRouteRaw);
            
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