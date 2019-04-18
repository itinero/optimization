using System.Collections.Generic;
using Itinero.Algorithms.Search;
using Itinero.Data.Network;
using Itinero.LocalGeo;
using Itinero.Logging;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Profiles;
using Vehicle = Itinero.Optimization.Models.Vehicles.Vehicle;

namespace Itinero.Optimization.Models.Simplification.ByEdge
{
    /// <summary>
    /// An edge base simplifier.
    /// </summary>
    public static class EdgeBasedSimplifier
    {
        /// <summary>
        /// Returns a simplified model based on the given model. Groups the visits together per edge.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="model">The model to simplify.</param>
        /// <param name="simplified">The simplified model and meta-data about the simplification itself.</param>
        /// <param name="message">The error message if any.</param>
        /// <returns>True if simplification succeeds.</returns>
        public static bool TrySimplify(RouterBase router, Model model, out (Model model, EdgeBasedSimplification details, PresolvedMassResolvingAlgorithm massResolvingAlgorithm) simplified,
            out string message)
        {            
            // Verify if this simplifier can handle this model:
            // - check if the profile is supported.
            // - check for time windows, in this case simplification is not supported.
            // - check for visit costs, in this case simplification is not supported.
            var metric = model.VehiclePool.Vehicles[0].Metric; // this exists because the model was validated.
            var profileName = model.VehiclePool.Vehicles[0].Profile;
            for (var v = 1; v < model.VehiclePool.Vehicles.Length; v++)
            {
                var vehicle = model.VehiclePool.Vehicles[v];
                if (vehicle.Profile != profileName)
                {
                    message =
                        $"Two different vehicle profiles found: {profileName} at index '0' and {vehicle.Profile} at '{v}'.";
                    simplified = (null, null, null);
                    return false;
                }
            }

            foreach (var visit in model.Visits)
            {
                if (visit.TimeWindow != null &&
                    !visit.TimeWindow.IsEmpty)
                {
                    message =
                        $"There is a time window in visit {visit}, cannot simplify!";
                    simplified = (null, null, null);
                    return false;
                }

                if (visit.VisitCosts != null && visit.VisitCosts.Length > 0)
                {
                    message =
                        $"There is a least one visit cost in visit {visit}, cannot simplify!";
                    simplified = (null, null, null);
                    return false;
                }
            }
            
            if (!router.Db.SupportProfile(profileName))
            {
                message =
                    $"The vehicle profile is not supported: '{profileName}'.";
                simplified = (null, null, null);
                return false;
            }
            var profile = router.Db.GetSupportedProfile(profileName);
            if (profile.Metric.ToModelMetric() != metric)
            {
                message =
                    $"The vehicle profile metric '{profile.Metric.ToModelMetric()}' doesn't match what was defined in the vehicle: '{metric}'.";
                simplified = (null, null, null);
                return false;
            }
            
            var locations = new Coordinate[model.Visits.Length];
            for (var i = 0; i < locations.Length; i++)
            {
                locations[i] = new Coordinate()
                {
                    Latitude = model.Visits[i].Latitude,
                    Longitude = model.Visits[i].Longitude
                };
            }
            
            // do mass resolving.
            var massResolvingAlgorithm =
                new MassResolvingAlgorithm(router, new IProfileInstance[] { profile }, locations, null, maxSearchDistance: 1000f);
            massResolvingAlgorithm.Run();
            if (!massResolvingAlgorithm.HasSucceeded)
            {
                message =
                    $"Resolving failed: {massResolvingAlgorithm.ErrorMessage}";
                simplified = (null, null, null);
                return false;
            }
            
            // calculate the best location per edge.
            var perEdge = new Dictionary<uint, (ushort min, ushort max)>();
            foreach (var rp in massResolvingAlgorithm.RouterPoints)
            {
                if (!perEdge.TryGetValue(rp.EdgeId, out var positions))
                {
                    perEdge[rp.EdgeId] = (rp.Offset, rp.Offset);
                    continue;
                }

                if (rp.Offset < positions.min)
                {
                    positions = (rp.Offset, positions.max);
                }

                if (rp.Offset > positions.max)
                {
                    positions = (positions.min, rp.Offset);
                }

                perEdge[rp.EdgeId] = positions;
            }
            
            // build the new visits array.
            var newVisits = new List<Visit>();
            var newVisitRouterPoints = new List<RouterPoint>();
            var visitPerEdge = new Dictionary<uint, int>();
            foreach (var v in perEdge)
            {
                var location = router.Db.Network.LocationOnNetwork(v.Key, (ushort) ((v.Value.min + v.Value.max) / 2));
                newVisits.Add(new Visit()
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    TimeWindow = null,
                    VisitCosts = null
                });
                newVisitRouterPoints.Add(new RouterPoint(location.Latitude, location.Longitude, v.Key, (ushort) ((v.Value.min + v.Value.max) / 2)));
                visitPerEdge[v.Key] = newVisits.Count - 1;
            }
            
            // build a mapping per visit.
            var toMapping = new List<int>();
            for (var v = 0; v < massResolvingAlgorithm.RouterPoints.Count; v++)
            {
                toMapping.Add(visitPerEdge[massResolvingAlgorithm.RouterPoints[v].EdgeId]);
            }
            
            // build the new vehicles array.
            var newVehicles = new Vehicle[model.VehiclePool.Vehicles.Length];
            for (var v = 0; v < newVehicles.Length; v++)
            {
                var vehicle = model.VehiclePool.Vehicles[v];
                var newVehicle = new Vehicle()
                {
                    Arrival = vehicle.Arrival,
                    Departure = vehicle.Departure,
                    CapacityConstraints = null,
                    Metric = vehicle.Metric,
                    Profile = vehicle.Profile,
                    TurnPentalty = vehicle.TurnPentalty
                };
                if (newVehicle.Arrival.HasValue)
                {
                    if (massResolvingAlgorithm.Errors.TryGetValue(newVehicle.Arrival.Value, out var error))
                    {
                        message = $"Arrival location is in error: {error.Code} - {error.Message}.";
                        simplified = (null, null, null);
                        return false;
                    }
                    newVehicle.Arrival = massResolvingAlgorithm.ResolvedIndexOf(newVehicle.Arrival.Value);
                    newVehicle.Arrival = toMapping[newVehicle.Arrival.Value];
                }

                if (newVehicle.Departure.HasValue)
                {
                    if (massResolvingAlgorithm.Errors.TryGetValue(newVehicle.Departure.Value, out var error))
                    {
                        message = $"Departure location is in error: {error.Code} - {error.Message}.";
                        simplified = (null, null, null);
                        return false;
                    }
                    newVehicle.Departure = massResolvingAlgorithm.ResolvedIndexOf(newVehicle.Departure.Value);
                    newVehicle.Departure = toMapping[newVehicle.Departure.Value];
                }

                newVehicles[v] = newVehicle;
            }
            var newVehiclePool = new VehiclePool()
            {
                Vehicles = newVehicles,
                Reusable = model.VehiclePool.Reusable
            };

            var newModel = new Model()
            {
                VehiclePool = newVehiclePool,
                Visits = newVisits.ToArray()
            };
            
            Logger.Log(nameof(EdgeBasedSimplifier), TraceEventType.Verbose,
                $"Simplified model from {model.Visits.Length} visits to {newModel.Visits.Length}!");
            
            simplified = (newModel, new EdgeBasedSimplification(massResolvingAlgorithm, toMapping),
                    new PresolvedMassResolvingAlgorithm(router, new IProfileInstance[] { profile }, newVisitRouterPoints));
            message = string.Empty;
            return true;
        }
    }
}