using System;
using System.Collections.Generic;
using System.IO;
using Itinero.IO.Json;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Contains extension methods on top if the read only tour interface.
    /// </summary>
    public static class IReadOnlyTourExtensions
    {
        /// <summary>
        /// Returns true if the given route is closed.
        /// </summary>
        public static bool IsClosed(this IReadOnlyTour tour)
        {
            return tour.Last.HasValue &&
                   tour.Last.Value == tour.First;
        }
        
        /// <summary>
        /// Gets an enumerable enumerating pairs in the tour enumerable.
        /// </summary>
        /// <param name="tourEnumerable">A tour enumerable.</param>
        /// <returns>An enumerable enumerating pairs.</returns>
        public static IEnumerable<Pair> Pairs(this IReadOnlyTour tourEnumerable)
        {
            return new PairEnumerable<IReadOnlyTour>(tourEnumerable, tourEnumerable.IsClosed());
        }
        
        /// <summary>
        /// Gets an enumerable enumerating quadruples in the tour.
        /// </summary>
        /// <param name="tour">A tour.</param>
        /// <param name="wrapAround">When true, and the tour is closed, wrap around start.</param>
        /// <param name="includePartials">When true, and the tour is open, wrap around start but fills open entries with Tour.NOT_SET.</param>
        /// <returns>An enumerable enumerating quadruples.</returns>
        public static IEnumerable<Quad> Quadruplets(this IReadOnlyTour tour, bool wrapAround = true, bool includePartials = false)
        {
            if (tour.IsClosed())  return new QuadEnumerable(tour, wrapAround, includePartials);
            
            return new QuadEnumerable(tour, false, includePartials);
        }
        
        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <returns>The total weight.</returns>
        internal static float Weight(this IReadOnlyTour tour, Func<int, int, float> weightsFunc)
        {
            var weight = 0f;
            var previous = Tour.NOT_SET;
            var first = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                if (previous == Tour.NOT_SET)
                {
                    first = visit;
                    previous = visit;
                    continue;
                }

                weight += weightsFunc(previous, visit);
                previous = visit;
            }

            var closed = tour.IsClosed();
            if (closed &&
                first != Tour.NOT_SET)
            {
                weight += weightsFunc(previous, first);
            }

            return weight;
        }

        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightsFunc">The weight function.</param>
        /// <param name="visitCount">The number of visits found.</param>
        /// <returns>The total weight.</returns>
        internal static float Weight(this IReadOnlyTour tour, Func<int, int, float> weightsFunc, out int visitCount)
        {
            visitCount = 0;
            var weight = 0f;
            var previous = Tour.NOT_SET;
            var first = Tour.NOT_SET;
            foreach (var visit in tour)
            {
                visitCount++;
                if (previous == Tour.NOT_SET)
                {
                    first = visit;
                    previous = visit;
                    continue;
                }

                weight += weightsFunc(previous, visit);
                previous = visit;
            }

            var closed = tour.IsClosed();
            if (closed &&
                first != Tour.NOT_SET)
            {
                weight += weightsFunc(previous, first);
            }

            return weight;
        }

        /// <summary>
        /// Enumerates all visits starting at the given vist.
        /// </summary>
        public static IEnumerable<int> From(this IReadOnlyTour tour, int visit)
        {
            var started = false;
            foreach (var v in tour)
            {
                if (!started)
                {
                    if (v != visit)
                    {
                        continue;
                    }

                    started = true;
                }

                yield return v;
            }

            foreach (var v in tour)
            {
                if (v == visit)
                {
                    break;
                }

                yield return v;
            }
        }

        /// <summary>
        /// Returns a geojson linestring representing the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="locationFunc">The location function.</param>
        /// <param name="description">The description for the tour if any.</param>
        /// <returns>A geojson feature collection with one linestring.</returns>
        public static string ToGeoJson(this IReadOnlyTour tour, Func<int, Coordinate> locationFunc, string description = null)
        {
            var stringWriter = new StringWriter();
            var jsonWriter = new JsonWriter(stringWriter);
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "FeatureCollection", true, false);
            jsonWriter.WritePropertyName("features", false);
            jsonWriter.WriteArrayOpen();

            // write linestring.
            if (tour.Count > 1)
            {
                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Feature", true, false);
                jsonWriter.WriteProperty("name", "ShapeMeta", true, false);
                jsonWriter.WritePropertyName("geometry", false);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "LineString", true, false);
                jsonWriter.WritePropertyName("coordinates", false);
                jsonWriter.WriteArrayOpen();

                foreach (var visit in tour)
                {
                    var coordinate = locationFunc(visit);

                    jsonWriter.WriteArrayOpen();
                    jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
                    jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
                    if (coordinate.Elevation.HasValue)
                    {
                        jsonWriter.WriteArrayValue(coordinate.Elevation.Value.ToInvariantString());
                    }

                    jsonWriter.WriteArrayClose();
                }

                jsonWriter.WriteArrayClose();
                jsonWriter.WriteClose();

                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteOpen();

                if (description != null)
                {
                    jsonWriter.WriteProperty("tour", description, true, true);
                }

                jsonWriter.WriteClose();

                jsonWriter.WriteClose();
            }

            // write points per visit.
            var i = 0;
            foreach (var visit in tour)
            {
                var location = locationFunc(visit);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Feature", true, false);
                jsonWriter.WritePropertyName("geometry", false);

                jsonWriter.WriteOpen();
                jsonWriter.WriteProperty("type", "Point", true, false);
                jsonWriter.WritePropertyName("coordinates", false);
                jsonWriter.WriteArrayOpen();
                jsonWriter.WriteArrayValue(location.Longitude.ToInvariantString());
                jsonWriter.WriteArrayValue(location.Latitude.ToInvariantString());
                if (location.Elevation.HasValue)
                {
                    jsonWriter.WriteArrayValue(location.Elevation.Value.ToInvariantString());
                }

                jsonWriter.WriteArrayClose();
                jsonWriter.WriteClose();

                jsonWriter.WritePropertyName("properties");
                jsonWriter.WriteOpen();

                jsonWriter.WriteProperty("visit", visit.ToString(), false, false);
                jsonWriter.WriteProperty("index", i.ToString(), false, false);
                if (description != null)
                {
                    jsonWriter.WriteProperty("tour", description, true, true);
                }

                jsonWriter.WriteClose();

                jsonWriter.WriteClose();
                i++;
            }

            // write feature array close and feature collection close.
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            return stringWriter.ToInvariantString();
        }
    }
}