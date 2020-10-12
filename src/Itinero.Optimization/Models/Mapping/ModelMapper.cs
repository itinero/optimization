namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Abstract representation of a model mapper.
    /// </summary>
    public abstract class ModelMapper
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary> 
        /// A delegate to define a call for mapping a model onto a road network.
        /// </summary>
        /// <param name="router">The router to use.</param>
        /// <param name="model">The model to map.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="reasonWhy">The reason why if the mapping fails.</param>
        /// <param name="settings">The routing settings.</param>
        public abstract bool TryMap(RouterBase router, Model model,
            out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string reasonWhy, RoutingSettings<float>? settings = null);
    }
}