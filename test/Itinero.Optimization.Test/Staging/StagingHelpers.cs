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
 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Abstract.Tours;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Models;

namespace Itinero.Optimization.Test.Staging
{
    /// <summary>
    /// Some helper methods to setup test-problems.
    /// </summary>
    public static class StagingHelpers
    {
        public static string EmbeddedResourceRoot = "Itinero.Optimization.Test.";

        /// <summary>
        /// Deserializes a model from the given embedded resource.
        /// </summary>
        public static Model GetModel(this string embeddedResourcePath)
        {
            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };

            using (var stream = typeof(NotSoRandomGenerator).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return Model.FromJson(json);
            }
        }

        /// <summary>
        /// Deserializes a model from the given embedded resource.
        /// </summary>
        public static AbstractModel GetAbstractModel(this string embeddedResourcePath)
        {
            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };

            using (var stream = typeof(NotSoRandomGenerator).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return AbstractModel.FromJson(json);
            }
        }

        /// <summary>
        /// Gets a feature collection for the given embedded resource.
        /// </summary>
        public static FeatureCollection GetFeatureCollection(this string embeddedResourcePath)
        {
            using (var stream = typeof(NotSoRandomGenerator).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return json.ToFeatures();
            }
        }

        /// <summary>
        /// Builds a linestring represents the route from the given tour and locations.
        /// </summary>
        public static LineString BuildRoute(this ITour tour, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var coordinates = new List<GeoAPI.Geometries.Coordinate>();
            var first = locations[tour.First];
            coordinates.Add(new GeoAPI.Geometries.Coordinate(first.Longitude, first.Latitude));
            foreach(var pair in tour.Pairs())
            {
                var to = locations[pair.To];
                coordinates.Add(new GeoAPI.Geometries.Coordinate(to.Longitude, to.Latitude));
            }
            
            return new LineString(coordinates.ToArray());
        }

        /// <summary>
        /// Builds the the routes from the given tours and locations.
        /// </summary>
        public static FeatureCollection BuildRoutes(this IMultiTour tours, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var features = new FeatureCollection();

            for (var t = 0; t < tours.Count; t++)
            {
                var lineString = tours.Tour(t).BuildRoute(locations);
                var attributes = new AttributesTable();
                attributes.Add("tour_id", t);
                features.Add(new Feature(lineString, attributes));
            }

            return features;
        }

        /// <summary>
        /// Builds a test problem and default solution (as-the-crow-flies distances) from the given lines.
        /// </summary>
        /// <param name="features">The linestring(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The weights (as-the-crow-flies distances) between the points in the linestring(s).</returns>
        public static float[][] BuildMatrix(this FeatureCollection features, out List<Itinero.LocalGeo.Coordinate> locations)
        {
            List<ITour> tours;
            IAttributesTable attributes;
            return features.BuildMatrix(out tours, out locations, out attributes);
        }

        /// <summary>
        /// Builds a test problem and default solution (as-the-crow-flies distances) from the given lines.
        /// </summary>
        /// <param name="features">The linestring(s).</param>
        /// <param name="tours">The predefined tour(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The weights (as-the-crow-flies distances) between the points in the linestring(s).</returns>
        public static float[][] BuildMatrix(this FeatureCollection features, out List<ITour> tours, 
            out List<Itinero.LocalGeo.Coordinate> locations, out IAttributesTable attributes)
        {
            tours = new List<ITour>();
            locations = new List<Itinero.LocalGeo.Coordinate>();
            attributes = null;

            foreach (var feature in features.Features)
            {
                var multiLineString = feature.Geometry as MultiLineString;
                if (multiLineString == null)
                { // ignore geometries that are not multilinestrings.
                    continue;
                }

                attributes = feature.Attributes;
                if (multiLineString != null)
                {
                    foreach (var geometry in multiLineString.Geometries)
                    {
                        var lineString = geometry as LineString;
                        var rawTour = new List<int>();
                        foreach (var coord in lineString.Coordinates)
                        {
                            var locationId = locations.AddAndCheckDuplicates(coord);
                            rawTour.Add(locationId);
                        }
                        var last = rawTour[rawTour.Count - 1];
                        if (rawTour[0] == last)
                        {
                            rawTour.RemoveAt(rawTour.Count - 1);
                        }
                        tours.Add(new Tour(rawTour, last));
                    }
                }
            }

            var weights = new float[locations.Count][];
            for (var x = 0; x < locations.Count; x++)
            {
                weights[x] = new float[locations.Count];
                for (var y = 0; y < locations.Count; y++)
                {
                    weights[x][y] = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(
                        locations[x],
                        locations[y]
                    );
                }
            }
            return weights;
        }

        /// <summary>
        /// Builds visit costs from points in the given feature collection.
        /// </summary>
        /// <param name="features">The feature(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The visit costs as defined in the attributes.</returns>
        public static List<VisitCosts> BuildVisitCosts(this FeatureCollection features, List<Itinero.LocalGeo.Coordinate> locations)
        {
            var visitCosts = new List<VisitCosts>();
            foreach (var feature in features.Features)
            {
                var point = feature.Geometry as Point;
                if (point == null)
                { // ignore geometries that are not multilinestrings.
                    continue;
                }

                var attributes = feature.Attributes;
                if (attributes.TryGetValueString("type", out string type) &&
                    type == "visitcost")
                {
                    var visit = locations.GetExistingLocation(new Itinero.LocalGeo.Coordinate(
                        (float)point.Coordinate.Y, (float)point.Coordinate.X));
                    if (visit < 0)
                    {
                        continue;
                    }

                    foreach (var key in feature.Attributes.GetNames())
                    {
                        if (key == "type")
                        {
                            continue;
                        }
                        
                        if (attributes.TryGetValueSingle(key, out float cost))
                        {
                            var visitCost = visitCosts.FirstOrDefault(x => x.Name == key);
                            if (visitCost == null)
                            {
                                visitCost = new VisitCosts()
                                {
                                    Name = key,
                                    Costs = new float[locations.Count]
                                };
                                visitCosts.Add(visitCost);
                            }
                            visitCost.Costs[visit] = cost;
                        }
                    }
                }
            }

            return visitCosts;
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, GeoAPI.Geometries.Coordinate location, int meters = 10)
        {
            return locations.AddAndCheckDuplicates(new Itinero.LocalGeo.Coordinate((float)location.Y, (float)location.X), meters);
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, Itinero.LocalGeo.Coordinate location, int meters = 10)
        {
            var locationId = locations.GetExistingLocation(location, meters);
            if (locationId >= 0)
            {
                return locationId;
            }
            locations.Add(location);
            return locations.Count - 1;
        }

        /// <summary>
        /// Gets a location nearby. Returns the existing ID or -1.
        /// </summary>
        public static int GetExistingLocation(this List<Itinero.LocalGeo.Coordinate> locations, Itinero.LocalGeo.Coordinate location, int meters = 10)
        {
            for (var i = 0; i  < locations.Count; i++)
            {
                if (Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(locations[i], location) < meters)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns true if the two given tour's bounding boxes overlap.
        /// </summary>
        public static bool ToursOverlap(this List<Itinero.LocalGeo.Coordinate> locations, ITour tour1, ITour tour2)
        {
            var box1 = locations.BoundingBox(tour1);
            var box2 = locations.BoundingBox(tour2);

            return box1.Overlaps(box2);
        }

        /// <summary>
        /// Calculates the boundingbox around the given tour.
        /// </summary>
        /// <param name="algorithm">The weight matrix algorithm.</param>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public static LocalGeo.Box BoundingBox(this List<Itinero.LocalGeo.Coordinate> locations, ITour tour)
        {
            LocalGeo.Box? box = null;
            foreach (var visit in tour)
            {
                var visitLocation = locations[visit];
                if (box == null)
                {
                    box = new LocalGeo.Box(visitLocation, visitLocation);
                }
                else
                {
                    box = box.Value.ExpandWith(visitLocation.Latitude, visitLocation.Longitude);
                }
            }
            return box.Value;
        }

        /// <summary>
        /// Builds a feature for the given tour.
        /// </summary>
        public static Feature BuildFeature(this ITour tour, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var coordinates = new List<Coordinate>();
            foreach (var visit in tour)
            {
                coordinates.Add(new Coordinate(locations[visit].Longitude, 
                    locations[visit].Latitude));
            }
            if (tour.IsClosed())
            {
                coordinates.Add(new Coordinate(locations[tour.Last.Value].Longitude, 
                    locations[tour.Last.Value].Latitude));
            }

            var attributes = new AttributesTable();
            return new Feature(new LineString(
                coordinates.ToArray()), attributes);
        }

        /// <summary>
        /// Builds a features for the given tours.
        /// </summary>
        public static FeatureCollection BuildFeatures(this IEnumerable<ITour> tours, IList<Itinero.LocalGeo.Coordinate> locations)
        {
            var features = new FeatureCollection();
            var tourIdx = 0;
            foreach (var tour in tours)
            {
                var feature = tour.BuildFeature(locations);
                feature.Attributes.AddAttribute("tour", tourIdx);
                features.Features.Add(feature);
                tourIdx++;
            }
            return features;
        }
    }
}