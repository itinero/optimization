using System.Collections.Generic;
using Itinero.Algorithms.Search;

namespace Itinero.Optimization.Models.Simplification.ByEdge
{
    /// <summary>
    /// Contains details about a model simplification.
    /// </summary>
    public class EdgeBasedSimplification
    {
        private readonly MassResolvingAlgorithm _massResolvingAlgorithm;
        private readonly List<int> _toMapping;

        /// <summary>
        /// Creates a new simplification.
        /// </summary>
        /// <param name="massResolvingAlgorithm">The resolving algorithm.</param>
        /// <param name="toMapping">The mapping to the original visits.</param>
        public EdgeBasedSimplification(MassResolvingAlgorithm massResolvingAlgorithm, List<int> toMapping)
        {
            _toMapping = toMapping;
            _massResolvingAlgorithm = massResolvingAlgorithm;
        }

        /// <summary>
        /// Gets the mass resolving algorithm.
        /// </summary>
        public MassResolvingAlgorithm MassResolvingAlgorithm => _massResolvingAlgorithm;

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        public IReadOnlyList<int> ToMapping => _toMapping; 
    }
}