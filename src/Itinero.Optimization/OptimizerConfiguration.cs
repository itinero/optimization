using System;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers;

namespace Itinero.Optimization
{
    /// <summary>
    /// Represents the configuration for the optimizer.
    /// </summary>
    public class OptimizerConfiguration
    {
        /// <summary>
        /// Creates a new optimizer configuration.
        /// </summary>
        /// <param name="solverRegistry">The solver registry.</param>
        /// <param name="modelMapperRegistry">The model mapper registry.</param>
        /// <param name="settings">The routing settings.</param>
        public OptimizerConfiguration(SolverRegistry solverRegistry = null,
            ModelMapperRegistry modelMapperRegistry = null,
            RoutingSettings<float> settings = null)
        {
            this.SolverRegistry = solverRegistry ?? SolverRegistry.Default;
            this.ModelMapperRegistry = modelMapperRegistry ?? ModelMapperRegistry.Default;
            if (settings == null)
            {
                settings = new RoutingSettings<float>();
                settings.SetMaxSearch(string.Empty, 7200);
            }
            this.RoutingSettings = settings;
        }

        /// <summary>
        /// Gets the solver registry.
        /// </summary>
        internal SolverRegistry SolverRegistry { get; }

        /// <summary>
        /// Gets the model mapper registry.
        /// </summary>
        internal ModelMapperRegistry ModelMapperRegistry { get; }
        
        /// <summary>
        /// Gets the routing settings.
        /// </summary>
        public RoutingSettings<float> RoutingSettings { get; }
        
        private static readonly Lazy<OptimizerConfiguration> DefaultLazy = new Lazy<OptimizerConfiguration>(() => new OptimizerConfiguration());
        
        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        public static OptimizerConfiguration Default => DefaultLazy.Value;
    }
}