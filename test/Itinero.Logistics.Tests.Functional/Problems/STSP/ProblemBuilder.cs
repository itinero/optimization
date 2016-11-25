using Itinero.LocalGeo;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Itinero.Logistics.Tests.Functional.Problems.STSP
{
    public static class ProblemBuilder
    {
        /// <summary>
        /// Constructs a problem from an embedded GeoJSON string.
        /// </summary>
        /// <returns></returns>
        public static void BuildEmbedded(string embeddedResource, out Coordinate source, out Coordinate[] locations)
        {
            using (var stream = new StreamReader(
              Assembly.GetExecutingAssembly().GetManifestResourceStream(
                  embeddedResource)))
            {
                Build(stream.ReadToEnd(), out source, out locations);
            }
        }

        /// <summary>
        /// Constructs a problem from a GeoJSON string.
        /// </summary>
        /// <returns></returns>
        public static void Build(string geojson, out Coordinate source, out Coordinate[] locations)
        {
            // build stops list from geojson.
            var features = geojson.ToFeatures();

            source = new Coordinate();
            var locationsList = new List<Coordinate>();
            foreach (var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    var point = feature.Geometry as Point;
                    var location = new Coordinate((float)point.Coordinate.Y, (float)point.Coordinate.X);
                    if (feature.Attributes.Contains("type", "source"))
                    {
                        source = location;
                    }
                    else
                    {
                        locationsList.Add(location);
                    }                    
                }
            }

            locations = locationsList.ToArray();
        }
    }
}
