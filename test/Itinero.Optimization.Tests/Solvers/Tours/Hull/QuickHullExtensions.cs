using System.Collections.Generic;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Tours.Hull;

namespace Itinero.Optimization.Tests.Solvers.Tours.Hull
{
    public static class QuickHullExtensions
    {
        public static Polygon ToPolygon(this TourHull tourHull)
        {
            var polygon = new Polygon()
            {
                ExteriorRing = new List<Coordinate>()
            };
            for (var i = 0; i < tourHull.Count; i++)
            {
                polygon.ExteriorRing.Add(tourHull[i].location);
            }

            polygon.ExteriorRing.Add(tourHull[0].location);

            return polygon;
        }
    }
}