using System;

namespace Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition
{
    /// <summary>
    /// Settings for visit prevention for visits in a give position.
    /// </summary>
    public class VisitPositionRewriterSettings
    {
        /// <summary>
        /// Prevents visits on the left when true, on the right when false.
        /// </summary>
        public bool Left { get; set; } = true;

        /// <summary>
        /// The tolerance value of the angle. When 90 degrees any pickup will be prevented on left/right.
        /// </summary>
        public double Tolerance { get; set; } = 90;

        /// <summary>
        /// Calculates the angle relative to the direction of the edge the router point is on.
        /// </summary>
        public Func<RouterPoint, double?>? AngleFunc { get; set; } = null;
    }
}