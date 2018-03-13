using System;
using Itinero.Algorithms;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Abstract definition of a weight matrix.
    /// </summary>
    public abstract class WeightMatrixBase
    {
        /// <summary>
        /// Gets the main metric.
        /// </summary>
        /// <returns></returns>
        public abstract string Metric
        {
            get;
        }

        /// <summary>
        /// Gets the index in the matrix of the original location.
        /// </summary>
        /// <param name="original">The index in the original location array.</param>
        /// <returns></returns>
        public abstract int? WeightIndexNullable(int? original);

        /// <summary>
        /// Builds travel costs matrices based on the data in this weight matrix.
        /// </summary>
        /// <returns></returns>
        public abstract Abstract.Models.Costs.TravelCostMatrix[] BuildTravelCostMatrices();

        /// <summary>
        /// Adjusts the items to the weight matrix removing invalid or unresolved visits.
        /// </summary>
        /// <param name="array">The array to adjust.</param>
        /// <returns></returns>
        public abstract T[] AdjustToMatrix<T>(T[] array);

        /// <summary>
        /// Builds a real-world route based on the given tour.
        /// </summary>
        public abstract Route BuildRoute(ITour tour);

        /// <summary>
        /// Returns true if the two tours geographically overlaps.
        /// </summary>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public abstract bool Overlaps(ITour tour1, ITour tour2);
    }
}