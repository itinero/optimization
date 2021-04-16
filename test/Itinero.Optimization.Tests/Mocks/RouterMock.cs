using System;
using System.Collections.Generic;
using System.Threading;
using Itinero.Algorithms;
using Itinero.Algorithms.Search;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.LocalGeo;
using Itinero.Profiles;

namespace Itinero.Optimization.Tests.Mocks
{
    public class RouterMock : RouterBase
    {
        public override Result<RouterPoint> TryResolve(IProfileInstance[] profiles, float latitude, float longitude, Func<RoutingEdge, bool> isBetter,
            float searchDistanceInMeter, ResolveSettings settings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<bool> TryCheckConnectivity(IProfileInstance profile, RouterPoint point, float radiusInMeter, bool? forward,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<EdgePath<T>> TryCalculateRaw<T>(IProfileInstance profile, WeightHandler<T> weightHandler, RouterPoint source, RouterPoint target,
            RoutingSettings<T> settings, CancellationToken cancellationToken)
        {
            return new Result<EdgePath<T>>(new EdgePath<T>(0));
        }

        public override Result<EdgePath<T>> TryCalculateRaw<T>(IProfileInstance profileInstance, WeightHandler<T> weightHandler, RouterPoint source,
            bool? sourceForward, RouterPoint target, bool? targetForward, RoutingSettings<T> settings,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<EdgePath<T>> TryCalculateRaw<T>(IProfileInstance profile, WeightHandler<T> weightHandler, long sourceDirectedEdge,
            long targetDirectedEdge, RoutingSettings<T> settings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<EdgePath<T>[][]> TryCalculateRaw<T>(IProfileInstance profile, WeightHandler<T> weightHandler, RouterPoint[] sources,
            RouterPoint[] targets, RoutingSettings<T> settings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<T[][]> TryCalculateWeight<T>(IProfileInstance profile, WeightHandler<T> weightHandler, RouterPoint[] sources,
            RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets, RoutingSettings<T> settings,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Result<Route> BuildRoute<T>(IProfileInstance profile, WeightHandler<T> weightHandler, RouterPoint source, RouterPoint target,
            EdgePath<T> path, CancellationToken cancellationToken)
        {
            return new Result<Route>(new Route()
            {
                Shape = new[]
                {
                    new Coordinate(source.Latitude, source.Longitude),
                    new Coordinate(target.Latitude, target.Longitude)
                }
            });
        }

        public override RouterDb Db { get; }
    }
}