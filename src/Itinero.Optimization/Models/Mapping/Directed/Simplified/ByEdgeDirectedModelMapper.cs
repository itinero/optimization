using System;
using System.Threading;
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Logging;
using Itinero.Optimization.Models.Simplification.ByEdge;
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Models.Mapping.Directed.Simplified
{
    /// <summary>
    /// A model mapper that uses by edge simplification.
    /// </summary>
    public class ByEdgeDirectedModelMapper : ModelMapper
    {
        private readonly DirectedModelMapper _modelMapper;
        
        /// <summary>
        /// Creates a new model mapper.
        /// </summary>
        /// <param name="maxSearchDistance">The max search distance.</param>
        public ByEdgeDirectedModelMapper(float maxSearchDistance = 250)
        {
            _modelMapper = new DirectedModelMapper(maxSearchDistance);
        }
        
        /// <inhertitdoc/>
        public override string Name { get; } = "DirectedSimplifiedByEdge";

        /// <inhertitdoc/>
        public override bool TryMap(RouterBase router, Model model, out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string message)
        {
            if (!EdgeBasedSimplifier.TrySimplify(router, model, out var simplified, out message))
            {
                return _modelMapper.TryMap(router, model, out mappings, out message);
            }

            // use simplified model from now on.
            model = simplified.model;

            // Verify if this mapper can handle this model:
            // - check if there is at least one vehicle with a turn-cost.
            // - check if there is only one metric defined.
            // - check if there is only one profile defined.
            // - check if the profile is supported.
            // - check if metrics match.
            var metric = model.VehiclePool.Vehicles[0].Metric; // this exists because the model was validated.
            var profileName = model.VehiclePool.Vehicles[0].Profile;
            var oneVehicleWithTurnPenalty = model.VehiclePool.Vehicles[0].TurnPentalty > 0;
            for (var v = 1; v < model.VehiclePool.Vehicles.Length; v++)
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

                if (vehicle.TurnPentalty > 0)
                {
                    oneVehicleWithTurnPenalty = true;
                }
            }

            if (!oneVehicleWithTurnPenalty)
            {
                message =
                    $"No vehicle found with a turn penalty..";
                mappings = (null, null);
                return false;
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
            
            Logger.Log(nameof(EdgeBasedSimplifier), TraceEventType.Verbose,
                $"Calculating directed matrix for {simplified.massResolvingAlgorithm.RouterPoints.Count} visits!");
            var weightMatrixAlgorithm = new DirectedWeightMatrixAlgorithm(router, profile, simplified.massResolvingAlgorithm);
            weightMatrixAlgorithm.Run();
            if (!weightMatrixAlgorithm.HasSucceeded)
            {
                message =
                    $"Calculating weight matrix failed: {weightMatrixAlgorithm.ErrorMessage}";
                mappings = (null, null);
                return false;
            }
            Logger.Log(nameof(EdgeBasedSimplifier), TraceEventType.Verbose,
                $"Calculating directed matrix for {simplified.massResolvingAlgorithm.RouterPoints.Count} visits finished");
            
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
                TravelCosts = new TravelCostMatrix[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = weightMatrixAlgorithm.Weights,
                        Directed = false,
                        Metric = metric
                    }
                }
            };
            mappings = (mappedModel, new ByEdgeDirectedModelMapping(mappedModel, weightMatrixAlgorithm, simplified.details));
            return true;
        }
        
        private static readonly ThreadLocal<ByEdgeDirectedModelMapper> DefaultLazy = new ThreadLocal<ByEdgeDirectedModelMapper>(() => new ByEdgeDirectedModelMapper());
        
        /// <summary>
        /// Gets the default default model mapper.
        /// </summary>
        public static ByEdgeDirectedModelMapper Default => DefaultLazy.Value;
    }
}