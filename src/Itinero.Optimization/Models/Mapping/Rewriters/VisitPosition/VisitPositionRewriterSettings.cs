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
        /// Calculates the angle relative to the direction of the edge the router point is on.
        /// </summary>
        public Func<RouterPoint, double?>? AngleFunc { get; set; } = null;

        /// <summary>
        /// Allows to override the settings per visit.
        /// </summary>
        /// <remarks>
        /// When this function is not null the behaviour for each visit is as follows:
        /// - returns null, the visit is ignore and no action is taken.
        /// - return settings, the settings are used to process the visit.
        /// </remarks>
        public Func<int, VisitPositionRewriterSettings?>? SettingsPerVisit { get; set; } = null;
    }
}