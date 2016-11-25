// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.IO;

namespace Itinero.Logistics.Tests.Functional
{
    /// <summary>
    /// Contains extension methods for NTS.
    /// </summary>
    public static class GeoExtensions
    {
        /// <summary>
        /// Converts this geojson into a feature.
        /// </summary>
        public static Geometry ToGeometry(this string geoJson)
        {
            using (var stream = new JsonTextReader(new StringReader(geoJson)))
            {
                var geoJsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
                return geoJsonSerializer.Deserialize(stream) as Geometry;
            }
        }

        /// <summary>
        /// Converts this geojson into a feature.
        /// </summary>
        public static Feature ToFeature(this string geoJson)
        {
            using (var stream = new JsonTextReader(new StringReader(geoJson)))
            {
                var geoJsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
                return geoJsonSerializer.Deserialize(stream) as Feature;
            }
        }

        /// <summary>
        /// Converts this geojson into a feature collection.
        /// </summary>
        public static FeatureCollection ToFeatures(this string geoJson)
        {
            using (var stream = new JsonTextReader(new StringReader(geoJson)))
            {
                var geoJsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
                var obj = geoJsonSerializer.Deserialize<FeatureCollection>(stream);
                return obj as FeatureCollection;
            }
        }

        /// <summary>
        /// Converts this feature to geojson.
        /// </summary>
        public static string ToGeoJson(this Feature feature)
        {
            using (var stream = new StringWriter())
            {
                var geoJsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
                geoJsonSerializer.Serialize(stream, feature);
                return stream.ToString();
            }
        }

        /// <summary>
        /// Converts this feature collection to geojson.
        /// </summary>
        public static string ToGeoJson(this FeatureCollection features)
        {
            using (var stream = new StringWriter())
            {
                var geoJsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
                geoJsonSerializer.Serialize(stream, features);
                return stream.ToString();
            }
        }

        /// <summary>
        /// Tries to get a value for the given name.
        /// </summary>
        public static bool TryGetValue(this IAttributesTable attributes, string name, out object value)
        {
            var names = attributes.GetNames();
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    value = attributes.GetValues()[i];
                    return true;
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Returns true if the given key/value pair is in the attributes.
        /// </summary>
        public static bool Contains(this IAttributesTable attributes, string name, string value)
        {
            object val;
            if (attributes.TryGetValue(name, out val))
            {
                return val.ToInvariantString().Equals(value);
            }
            return false;
        }

        /// <summary>
        /// Adds all the given features.
        /// </summary>
        public static void AddRange(this FeatureCollection features, FeatureCollection other)
        {
            foreach (var feature in other.Features)
            {
                features.Add(feature);
            }
        }
    }
}
