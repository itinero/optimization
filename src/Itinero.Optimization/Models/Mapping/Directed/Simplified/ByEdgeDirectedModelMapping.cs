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
    internal class ByEdgeDirectedModelMapping : IModelMapping
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

        /// <inheritdoc />
        public IEnumerable<Result<Route>> BuildRoutesBetweenVisits((int vehicle, IEnumerable<int> tour) tourAndVehicle)
        {
            // TODO: we need a better more generic way of handling this. Current rules are:
            // - use vehicle departure and arrival to close a tour or not.
            // - close a tour by default when both departure and arrival are null.
            // PROBLEM: this doesn't support the case where we have a vehicle with arrival and departure null and we generate open-tours.
            // SUGGESTED FIX: look at the enumerable differently include all visits, even if that means closing the tour in the enumerable.
            var vehicle = _mappedModel.VehiclePool.Vehicles[tourAndVehicle.vehicle];

            var first = -1;
            var index = 0;
            RouterPoint? previousRouterPoint = null;
            var previous = -1;
            foreach (var v in tourAndVehicle.tour)
            {
                // gets the visits on the current segment.
                var visits = this.FindOriginalVisits(v);

                if (first < 0)
                {
                    var segmentRoutes = this.RoutesAlongOriginalVisits(visits, index);
                    foreach (var segmentRoute in segmentRoutes)
                    {
                        yield return segmentRoute;
                    }
                    first = v;
                }
                else
                {
                    // append route from previous segment to first arrival location in current segment.
                    if (previousRouterPoint != null)
                    {
                        var arrivalRouterPoint = visits[0].routerPoint;
                        yield return this.BuildRoute(previousRouterPoint, index, previous,
                            arrivalRouterPoint, index, v);
                    }

                    // append local segment route if any.
                    var segmentRoutes = this.RoutesAlongOriginalVisits(visits, index);
                    foreach (var segmentRoute in segmentRoutes)
                    {
                        yield return segmentRoute;
                    }
                }

                // keep some data on the previous segment.
                previousRouterPoint = visits[visits.Count - 1].routerPoint;
                previous = v;
                index += visits.Count;
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

        private IEnumerable<Result<Route>> RoutesAlongOriginalVisits(List<(int visit, RouterPoint routerPoint)> routerPoints, int startIndex)
        {
            for (var i = 1; i < routerPoints.Count; i++)
            {
                var localRouteResult = _weightMatrixAlgorithm.Router.TryCalculate(_weightMatrixAlgorithm.Profile,
                    routerPoints[i - 1].routerPoint, routerPoints[i].routerPoint);

                if (localRouteResult.IsError)
                {
                    yield return localRouteResult;
                    continue;
                }
                
                localRouteResult.Value.SetOrderAndIndex(routerPoints[i - 1].visit, (i - 1) + startIndex,
                    routerPoints[i].visit, (i) + startIndex);

                yield return localRouteResult;
            }
        }

        private Result<Route> BuildRoute(RouterPoint visit1RouterPoint, int directedVisit1Index, int directedVisit1, 
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

            return _weightMatrixAlgorithm.Router.BuildRoute(_weightMatrixAlgorithm.Profile, weightHandler, visit1RouterPoint, visit2RouterPoint, localRouteRaw);
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