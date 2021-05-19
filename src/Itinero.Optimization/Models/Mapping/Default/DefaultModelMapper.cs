using System.Threading;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.LocalGeo;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Profiles;

namespace Itinero.Optimization.Models.Mapping.Default
{
    /// <summary>
    /// A default model mapper.
    /// </summary>
    public class DefaultModelMapper : ModelMapper
    {
        /// <inhertitdoc/>
        public override string Name { get; } = "Default";

        /// <inhertitdoc/>
        public override bool TryMap(RouterBase router, Model model, out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string message, RoutingSettings<float> settings = null)
        {
            // Verify if this mapper can handle this model:
            // - check if there are any vehicles with a turn-cost.
            // - check if there is only one metric defined.
            // - check if there is only one profile defined.
            // - check if the profile is supported.
            // - check if metrics match.
            var metric = model.VehiclePool.Vehicles[0].Metric; // this exists because the model was validated.
            var profileName = model.VehiclePool.Vehicles[0].Profile;
            for (var v = 0; v < model.VehiclePool.Vehicles.Length; v++)
            {
                var vehicle = model.VehiclePool.Vehicles[v];
                if (vehicle.Metric != metric)
                {
                    message =
                        $"Two different vehicle metrics found: {metric} at index '0' and {vehicle.Metric} at '{v}'.";
                    mappings = (null, null);
                    return false;
                }
                if (vehicle.Profile != profileName)
                {
                    message =
                        $"Two different vehicle profiles found: {profileName} at index '0' and {vehicle.Profile} at '{v}'.";
                    mappings = (null, null);
                    return false;
                }

                if (vehicle.TurnPentalty != 0)
                {
                    message =
                        $"A vehicle was found with a turn penalty: {vehicle.TurnPentalty} at index '{v}'.";
                    mappings = (null, null);
                    return false;
                }
            }
            
            if (!router.Db.SupportProfile(profileName))
            {
                message =
                    $"The vehicle profile is not supported: '{profileName}'.";
                mappings = (null, null);
                return false;
            }
            var profile = router.Db.GetSupportedProfile(profileName);
            if (profile.Metric.ToModelMetric() != metric)
            {
                message =
                    $"The vehicle profile metric '{profile.Metric.ToModelMetric()}' doesn't match what was defined in the vehicle: '{metric}'.";
                mappings = (null, null);
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
                new MassResolvingAlgorithm(router, new IProfileInstance[] { profile }, locations, null, maxSearchDistance: 250f);
            massResolvingAlgorithm.Run();
            if (!massResolvingAlgorithm.HasSucceeded)
            {
                message =
                    $"Resolving failed: {massResolvingAlgorithm.ErrorMessage}";
                mappings = (null, null);
                return false;
            }

            var weightMatrixAlgorithm = new WeightMatrixAlgorithm(router, profile, massResolvingAlgorithm, settings);
            weightMatrixAlgorithm.Run();
            if (!weightMatrixAlgorithm.HasSucceeded)
            {
                message =
                    $"Calculating weight matrix failed: {weightMatrixAlgorithm.ErrorMessage}";
                mappings = (null, null);
                return false;
            }
            
            // try to adjust the vehicle pool to the mapping, if any of the defined departure or arrival points cannot be mapped then this will fail.
            if (!weightMatrixAlgorithm.TryToMap(model.VehiclePool, out var mappedVehiclePool, out message))
            {
                mappings = (null, null);
                return false;
            }
            
            // build mapped model.
            var mappedModel = new MappedModel()
            {
                Visits = weightMatrixAlgorithm.AdjustToMapping(model.Visits),
                VehiclePool = mappedVehiclePool,
                TravelCosts = new []
                {
                    new TravelCostMatrix()
                    {
                        Costs = weightMatrixAlgorithm.Weights,
                        Directed = false,
                        Metric = metric
                    }
                }
            };
            
            // build mapping.
            var modelMapping = new DefaultModelMapping(mappedModel, weightMatrixAlgorithm);

            mappings = (mappedModel, modelMapping);
            message = string.Empty;
            return true;
        }
        
        private static readonly ThreadLocal<DefaultModelMapper> DefaultLazy = new ThreadLocal<DefaultModelMapper>(() => new DefaultModelMapper());
        
        /// <summary>
        /// Gets the default default model mapper.
        /// </summary>
        public static DefaultModelMapper Default => DefaultLazy.Value;
    }
}