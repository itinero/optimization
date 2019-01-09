using System.Collections.Generic;
using Itinero.Algorithms.Search;

namespace Itinero.Optimization.Models.Simplification.ByEdge
{
    public class EdgeBasedSimplification
    {
        private readonly MassResolvingAlgorithm _massResolvingAlgorithm;
        private readonly List<int> _toMapping;

        public EdgeBasedSimplification(MassResolvingAlgorithm massResolvingAlgorithm, List<int> toMapping)
        {
            _toMapping = toMapping;
            _massResolvingAlgorithm = massResolvingAlgorithm;
        }

        public MassResolvingAlgorithm MassResolvingAlgorithm => _massResolvingAlgorithm;

        public List<int> ToMapping => _toMapping; 
    }
}