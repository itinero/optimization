namespace Itinero.Optimization.Solvers.Shared
{
    /// <summary>
    /// Represents a time window.
    /// </summary>
    internal struct TimeWindow
    {
        /// <summary>
        /// The minimum time in seconds.
        /// </summary>
        public float Min { get; set; }

        /// <summary>
        /// The maximum time in seconds.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Returns true if this window is valid at the given seconds.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public bool IsValidAt(float seconds)
        {
            return this.Min <= seconds && this.Max >= seconds;
        }

        /// <summary>
        /// Returns the minimum difference.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public float MinDiff(float seconds)
        {
            if (this.Min <= seconds && this.Max >= seconds)
            {
                // the time is within the window, no difference.
                return 0;
            }

            if (seconds < this.Min)
            {
                // time window too late.
                return this.Min - seconds;
            }

            return seconds - this.Max;
        }

        /// <summary>
        /// Returns true if this time windows is considered empty or 'to be ignored'.
        /// </summary>
        public bool IsEmpty => (this.Min == 0 &&
                               this.Max == 0) || 
                                (this.Min == float.MaxValue &&
                                this.Max == float.MinValue);

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{this.Min}, {this.Max}]";
        }
    }
}