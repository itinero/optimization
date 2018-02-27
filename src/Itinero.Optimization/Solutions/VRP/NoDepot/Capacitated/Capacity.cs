using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Routing;

namespace Itinero.Optimization.Solutions.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represent the capacity of a vehicle.
    /// </summary>
    public class Capacity
    {
        /// <summary>
        /// The maximum travel time.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// A collection of custom constraints.
        /// </summary>
        public CapacityConstraint[] Constraints { get; set; }

        /// <summary>
        /// Adjusts the capacity and returns a new version.
        /// </summary>
        public Capacity AdjustToMatrix(IWeightMatrixAlgorithm<float> weightMatrix)
        {
            if (this.Constraints == null)
            {
                return new Capacity()
                {
                    Max = this.Max
                };
            }

            var constraints = new CapacityConstraint[this.Constraints.Length];
            for (var i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new CapacityConstraint()
                {
                    Name = this.Constraints[i].Name,
                    Max = this.Constraints[i].Max,
                    Values = weightMatrix.AdjustToMatrix(this.Constraints[i].Values)
                };
            }
            return new Capacity()
            {
                Max = this.Max,
                Constraints = constraints
            };
        }

        /// <summary>
        /// Scales this capacity by the given ratio [0-1] and returns the result.
        /// </summary>
        /// <param name="ratio">The ratio defined in the range [0-1].</param>
        /// <returns></returns>
        public Capacity Scale(float ratio)
        {
            if (this.Constraints == null)
            {
                return new Capacity()
                {
                    Max = this.Max * ratio
                };
            }
            var constraints = new CapacityConstraint[this.Constraints.Length];
            for (var i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new CapacityConstraint()
                {
                    Max = this.Constraints[i].Max * ratio,
                    Values = this.Constraints[i].Values
                };
            }
            return new Capacity()
            {
                Max = this.Max * ratio,
                Constraints = constraints
            };
        }
}
}