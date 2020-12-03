using System.Collections.Generic;
using Itinero.Optimization.Models.Mapping;

namespace Itinero.Optimization.Tests.Models.Mapping.Rewriters.VisitPosition
{
    public class MockModelMapping : IModelMapping
    {
        private RouterDb _routerDb;

        public IEnumerable<Result<Route>> BuildRoutesBetweenVisits((int vehicle, IEnumerable<int> tour) tourAndVehicle)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint GetVisitSnapping(int mappedVisit)
        {
            return new RouterPoint(0, 0, (uint)mappedVisit, 0);
        }

        public int GetOriginalVisit(int mappedVisit)
        {
            throw new System.NotImplementedException();
        }

        public int? GetMappedIndex(int originalVisit)
        {
            throw new System.NotImplementedException();
        }

        RouterDb IModelMapping.RouterDb => _routerDb;

        public IEnumerable<(int visit, string message)> Errors { get; }
    }
}