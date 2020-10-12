namespace Itinero.Optimization.Models.Mapping.Rewriters
{
    /// <summary>
    /// A model mapper including a rewriter.
    /// </summary>
    public class ModelMapperWithRewriter : ModelMapper
    {
        private readonly ModelMapper _modelMapper;
        private readonly IMappedModelRewriter _mappedModelRewriter;

        /// <summary>
        /// Creates a new model mapper.
        /// </summary>
        /// <param name="modelMapper">The original model mapper.</param>
        /// <param name="mappedModelRewriter">The rewriter.</param>
        public ModelMapperWithRewriter(ModelMapper modelMapper,
            IMappedModelRewriter mappedModelRewriter)
        {
            _modelMapper = modelMapper;
            _mappedModelRewriter = mappedModelRewriter;
        }

        /// <inheritdoc/>
        public override string Name => $"{_modelMapper.Name}_{_mappedModelRewriter.Name}";

        /// <inheritdoc/>
        public override bool TryMap(RouterBase router, Model model, out (MappedModel mappedModel, IModelMapping modelMapping) mappings,
            out string reasonWhy, RoutingSettings<float>? settings = null)
        {
            if (!_modelMapper.TryMap(router, model, out mappings, out reasonWhy, settings))
            {
                return false;
            }

            var rewritten = _mappedModelRewriter.Rewrite(mappings.mappedModel, mappings.modelMapping);

            mappings = (rewritten, mappings.modelMapping);
            return true;
        }
    }
}