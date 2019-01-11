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
        public OptimizerConfiguration(SolverRegistry solverRegistry = null,
            ModelMapperRegistry modelMapperRegistry = null)
        {
            this.SolverRegistry = solverRegistry ?? SolverRegistry.Default;
            this.ModelMapperRegistry = modelMapperRegistry ?? ModelMapperRegistry.Default;
        }

        /// <summary>
        /// Gets the solver registry.
        /// </summary>
        internal SolverRegistry SolverRegistry { get; }

        /// <summary>
        /// Gets the model mapper registry.
        /// </summary>
        internal ModelMapperRegistry ModelMapperRegistry { get; }
        
        private static readonly Lazy<OptimizerConfiguration> DefaultLazy = new Lazy<OptimizerConfiguration>(() => new OptimizerConfiguration());
        
        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        public static OptimizerConfiguration Default => DefaultLazy.Value;
    }
}