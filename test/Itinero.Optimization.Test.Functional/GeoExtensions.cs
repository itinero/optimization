/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.IO;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Itinero.Optimization.Test.Functional
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
                var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
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
                var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
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
                var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
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
                var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
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
                var geoJsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
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
        /// Tries to get a value for the given name.
        /// </summary>
        public static bool TryGetValueInt32(this IAttributesTable attributes, string name, out int value)
        {
            object rawValue;
            if (!attributes.TryGetValue(name, out rawValue))
            {
                value = 0;
                return false;
            }
            if (!int.TryParse(rawValue.ToInvariantString(), out value))
            {
                value = 0;
                return false;
            }
            return true;
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