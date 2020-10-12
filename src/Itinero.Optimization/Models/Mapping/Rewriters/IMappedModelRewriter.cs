namespace Itinero.Optimization.Models.Mapping.Rewriters
{
    /// <summary>
    /// Abstract representation of a model rewriter.
    /// </summary>
    public interface IMappedModelRewriter
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Rewrites the given model, reusing the mapping.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns>A rewritten model.</returns>
        MappedModel Rewrite(MappedModel model, IModelMapping mapping);
    }
}