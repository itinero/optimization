using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
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
        public IEnumerable<Result<Route>> BuildRoutesBetweenVisits((int vehicle, IEnumerable<int> tour) tourAndVehicle)
        {
            // TODO: we need a better more generic way of handling this. Current rules are:
            // - use vehicle departure and arrival to close a tour or not.
            // - close a tour by default when both departure and arrival are null.
            // PROBLEM: this doesn't support the case where we have a vehicle with arrival and departure null and we generate open-tours.
            // SUGGESTED FIX: look at the enumerable differently include all visits, even if that means closing the tour in the enumerable.
            var vehicle = _mappedModel.VehiclePool.Vehicles[tourAndVehicle.vehicle];

            var index = -1;
            var previous = -1;
            var first = -1;
            foreach (var v in tourAndVehicle.tour)
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

                yield return BuildRoute(index - 1, previous, index, v);
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
                    throw new Exception(
                        $"Vehicle should match a tour starting at {vehicle.Departure} but the start of the tour is at:" +
                        $"{DirectedHelper.ExtractVisit(first)}");
                }

                yield return BuildRoute(index, previous, 0, first);
            }

            if (!vehicle.Arrival.HasValue &&
                !vehicle.Departure.HasValue &&
                previous != 0)
            {
                yield return BuildRoute(index, previous, 0, first);
            }
        }

        public bool IsDirected { get; } = true;

        public int GetOriginalVisit(int mappedVisit)
        {
            return _weightMatrixAlgorithm.OriginalLocationIndex(mappedVisit);
        }

        public int? GetMappedIndex(int originalVisit)
        {
            var mv = _weightMatrixAlgorithm.CorrectedIndexOf(originalVisit);
            if (mv < 0) return null;
            return mv;
        }

        RouterPoint IModelMapping.GetVisitSnapping(int visit)
        {
            return _weightMatrixAlgorithm.RouterPoints[visit];
        }

        RouterDb IModelMapping.RouterDb => _weightMatrixAlgorithm.Router.Db;

        private Result<Route> BuildRoute(int directedVisit1Index, int directedVisit1, int directedVisit2Index, int directedVisit2)
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
            
            var fromRouterPoint = _weightMatrixAlgorithm.RouterPoints[pairFromId];
            var toRouterPoint = _weightMatrixAlgorithm.RouterPoints[pairToId];

            fromRouterPoint.Attributes.AddOrReplace("order", directedVisit1Index.ToInvariantString());
            toRouterPoint.Attributes.AddOrReplace("order", directedVisit2Index.ToInvariantString());
            
            var localRouteRawResult = _weightMatrixAlgorithm.Router.TryCalculateRaw(_weightMatrixAlgorithm.Profile, weightHandler, pairFromEdgeId, pairToEdgeId, null);
            if (localRouteRawResult.IsError)
            {
                return localRouteRawResult.ConvertError<Route>();
            }

            var localRouteRaw = localRouteRawResult.Value;
            localRouteRaw.StripSource();
            localRouteRaw.StripTarget();

            return _weightMatrixAlgorithm.Router.BuildRoute(_weightMatrixAlgorithm.Profile, weightHandler, fromRouterPoint, toRouterPoint, localRouteRaw);
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