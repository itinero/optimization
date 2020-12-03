using System.Linq;
using Itinero.Attributes;
using Itinero.Data.Network.Edges;
using Itinero.Graphs.Geometric.Shapes;
using Itinero.LocalGeo;
using Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition;
using Xunit;

namespace Itinero.Optimization.Tests.Models.Mapping.Rewriters.VisitPosition
{
    public class RouterExtensionsTests
    {
        [Fact]
        public void RelativeAngle_Forward_Left_ShouldReturnLeftAngle()
        {
            // build router db.
            var routerDb = new RouterDb(Itinero.Data.Edges.EdgeDataSerializer.MAX_DISTANCE);
            routerDb.AddSupportedVehicle(Itinero.Osm.Vehicles.Vehicle.Car);
            routerDb.Network.AddVertex(0, 51.26683740279481f, 4.8010870814323425f);
            routerDb.Network.AddVertex(1, 51.26779063481351f, 4.8013392090797420f);
            routerDb.Network.AddEdge(0, 1, new EdgeData()
                {
                    Distance = 108,
                    MetaId = routerDb.EdgeProfiles.Add(new AttributeCollection(
                        new Attribute("name", "Abelshausen Blvd."))),
                    Profile = (ushort)routerDb.EdgeProfiles.Add(new AttributeCollection(
                        new Attribute("highway", "residential")))
                }, new ShapeEnumerable(Enumerable.Empty<Coordinate>()));
            
            var left = new Coordinate(51.2674549919918f, 4.8008617758750916f);
            
            var router = new Router(routerDb);
            var snap = router.Resolve(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), left);

            var angle = snap.RelativeAngle(routerDb);
            Assert.True(angle > 225 && angle < 315);
        }
        
        [Fact]
        public void RelativeAngle_Forward_Right_ShouldReturnRightAngle()
        {
            // build router db.
            var routerDb = new RouterDb(Itinero.Data.Edges.EdgeDataSerializer.MAX_DISTANCE);
            routerDb.AddSupportedVehicle(Itinero.Osm.Vehicles.Vehicle.Car);
            routerDb.Network.AddVertex(0, 51.26683740279481f, 4.8010870814323425f);
            routerDb.Network.AddVertex(1, 51.26779063481351f, 4.8013392090797420f);
            routerDb.Network.AddEdge(0, 1, new EdgeData()
            {
                Distance = 108,
                MetaId = routerDb.EdgeProfiles.Add(new AttributeCollection(
                    new Attribute("name", "Abelshausen Blvd."))),
                Profile = (ushort)routerDb.EdgeProfiles.Add(new AttributeCollection(
                    new Attribute("highway", "residential")))
            }, new ShapeEnumerable(Enumerable.Empty<Coordinate>()));
            
            
            var right = new Coordinate(51.26738450668778f, 4.8015296459198f);
            
            var router = new Router(routerDb);
            var snap = router.Resolve(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), right);

            var angle = snap.RelativeAngle(routerDb);
            Assert.True(angle > 45 && angle < 135);
        }
    }
}