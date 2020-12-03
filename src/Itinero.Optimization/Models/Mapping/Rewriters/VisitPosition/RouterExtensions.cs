using Itinero.Data.Network;
using Itinero.LocalGeo;
using Itinero.Navigation.Directions;

namespace Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition
{
    internal static class RouterExtensions
    {
        /// <summary>
        /// Returns the angle at the snapped location with the edge snapped to and the original location.
        /// </summary>
        /// <param name="point">The router point.</param>
        /// <param name="routerDb">The router db.</param>
        /// <param name="forward">Go with or against the edge direction.</param>
        /// <param name="distance">The distance in meter to go back.</param>
        /// <returns>The angle.</returns>
        internal static double? RelativeAngle(this RouterPoint point, RouterDb routerDb, bool? forward = true, float distance = 20)
        {
            var edge = routerDb.Network.GetEdge(point.EdgeId);
            var edgeLength = edge.Data.Distance;
            var distanceOffset = (distance / edgeLength) * ushort.MaxValue;

            ushort offset = 0;
            if (distanceOffset <= ushort.MaxValue)
            { // not the entire edge.
                offset = (ushort)System.Math.Max(0,
                    point.Offset - distanceOffset);
            }
            
            var fromLocation = routerDb.LocationOnNetwork(point.EdgeId, offset);
            var originalLocation = point.Location();
            var snapLocation = point.LocationOnNetwork(routerDb);

            if (Coordinate.DistanceEstimateInMeter(fromLocation, snapLocation) < 1) return null;
            if (Coordinate.DistanceEstimateInMeter(originalLocation, snapLocation) < 1) return null;
            
            // calculate angle at snapLocation for fromLocation ----> snapLocation ----> originalLocation
            // angle is clockwise
            var angleRadians = DirectionCalculator.Angle(fromLocation, snapLocation, originalLocation);
            return angleRadians.ToDegrees().NormalizeDegrees();
        }
    }
}