using System.Collections.Generic;
using Itinero.Optimization.Models.Mapping;

namespace Itinero.Optimization
{
    /// <summary>
    /// The result after an optimization request.
    /// </summary>
    public class OptimizerResult
    {
        /// <summary>
        /// Creates a new result.
        /// </summary>
        /// <param name="mappedModel">The mapped model.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="solution">The solution.</param>
        public OptimizerResult(MappedModel mappedModel, IModelMapping mapping, 
            IEnumerable<(int vehicle, IEnumerable<int> tour)> solution)
        {
            this.MappedModel = mappedModel;
            this.Mapping = mapping;
            this.Solution = solution;
        }
        
        /// <summary>
        /// Creates a new result.
        /// </summary>
        /// <param name="mappedModel">The mapped model.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="errorMessage">The error message.</param>
        public OptimizerResult(MappedModel mappedModel, IModelMapping mapping, string errorMessage)
        {
            this.MappedModel = mappedModel;
            this.Mapping = mapping;
            this.ErrorMessage = errorMessage;
        }
        
        /// <summary>
        /// Creates a new result.
        /// </summary>
        /// <param name="errorMessage"></param>
        public OptimizerResult(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// The mapped model.
        /// </summary>
        public MappedModel? MappedModel { get; }
        
        /// <summary>
        /// The mapping between the original model and the mapped model.
        /// </summary>
        public IModelMapping? Mapping { get; }
        
        /// <summary>
        /// The solution.
        /// </summary>
        public IEnumerable<(int vehicle, IEnumerable<int> tour)>? Solution { get; }
        
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Returns true if this result represents an error.
        /// </summary>
        public bool IsError => this.Solution == null;
    }
}