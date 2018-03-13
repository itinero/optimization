using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents a directed weight matrix.
    /// </summary>
    public class WeightMatrixDirected : WeightMatrixBase
    {
        private readonly IDirectedWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;

        /// <summary>
        /// Creates a new weight matrix.
        /// </summary>
        /// <param name="weightMatrixAlgorithm"></param>
        public WeightMatrixDirected(IDirectedWeightMatrixAlgorithm<float> weightMatrixAlgorithm)
        {
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        /// <summary>
        /// Gets the main metric.
        /// </summary>
        /// <returns></returns>
        public override string Metric => _weightMatrixAlgorithm.Profile.Profile.Metric.ToModelMetric();

        /// <summary>
        /// Adjusts the items to the weight matrix removing invalid or unresolved visits.
        /// </summary>
        /// <param name="array">The array to adjust.</param>
        /// <returns></returns>
        public override T[] AdjustToMatrix<T>(T[] array)
        {
            return _weightMatrixAlgorithm.AdjustToMatrix(array);
        }

        /// <summary>
        /// Builds a real-world route based on the given tour.
        /// </summary>
        public override Route BuildRoute(ITour tour)
        {
            return _weightMatrixAlgorithm.BuildRoute(tour);
        }

        /// <summary>
        /// Builds travel costs matrices based on the data in this weight matrix.
        /// </summary>
        /// <returns></returns>
        public override TravelCostMatrix[] BuildTravelCostMatrices()
        {
            return new TravelCostMatrix[]
            {
                new TravelCostMatrix()
                {
                    Costs = _weightMatrixAlgorithm.Weights,
                    Name = _weightMatrixAlgorithm.Profile.Profile.Metric.ToModelMetric(),
                    Directed = true
                }
            };
        }

        /// <summary>
        /// Gets the index in the matrix of the original location.
        /// </summary>
        /// <param name="original">The index in the original location array.</param>
        /// <returns></returns>
        public override int? WeightIndexNullable(int? original)
        {
            if (original == null)
            {
                return null;
            }
            return _weightMatrixAlgorithm.WeightIndex(original.Value);
        }

        /// <summary>
        /// Returns true if the two tours geographically overlaps.
        /// </summary>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public override bool Overlaps(ITour tour1, ITour tour2)
        {
            return _weightMatrixAlgorithm.ToursOverlap(tour1, tour2);
        }
    }
}