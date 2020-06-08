using System.Collections.Generic;
using System.Text;
using Itinero.Optimization.Models.TimeWindows;

namespace Itinero.Optimization.Solvers.Shared
{
    /// <summary>
    /// Represents a time window.
    /// </summary>
    internal struct TimeWindow
    {
        /// <summary>
        /// Gets or sets the times of the windows.
        /// </summary>
        public IReadOnlyList<float>? Times { get; set; }

        /// <summary>
        /// Gets the earliest time where this window is valid.
        /// </summary>
        public float Min
        {
            get
            {
                if (this.Times == null || this.Times.Count == 0) return 0;

                return this.Times[0];
            }
        }

        /// <summary>
        /// Gets the latest time where this window is valid.
        /// </summary>
        public float Max
        {
            get
            {
                if (this.Times == null || this.Times.Count == 0) return float.MaxValue;

                if (this.Times.Count % 2 == 0) return this.Times[this.Times.Count - 1];

                return float.MaxValue;
            }
        }

        /// <summary>
        /// Returns true if this window is valid at the given seconds.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public bool IsValidAt(float seconds)
        {
            if (this.Times == null || this.Times.Count == 0) return true;
            
            foreach (var range in this.Times.ToRanges())
            {
                if (range.start <= seconds && range.end >= seconds)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the minimum difference.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public float MinDiff(float seconds)
        {
            var minDiff = float.MaxValue;
            if (this.Times == null || this.Times.Count == 0) return minDiff;
            
            foreach (var range in this.Times.ToRanges())
            {
                if (range.start > seconds)
                {
                    var localDiff =  range.start  - seconds;
                    if (minDiff > localDiff) minDiff = localDiff;
                }
                else if (range.end < seconds)
                {
                    var localDiff = seconds - range.end;
                    if (minDiff > localDiff) minDiff = localDiff;
                }
                else
                {
                    return 0;
                }
            }

            return minDiff;
        }

        /// <summary>
        /// Returns true if this time windows is considered empty or 'to be ignored'.
        /// </summary>
        public bool IsEmpty => this.Times == null ||  this.Times.Count == 0;

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Times == null) return "[0, ∞[";
            
            var builder = new StringBuilder();
            for (var t = 0; t < this.Times.Count; t++)
            {
                if (t % 2 == 0)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append('[');
                    builder.Append(this.Times[t].ToInvariantString());
                }
                else
                {
                    builder.Append(',');
                    builder.Append(' ');
                    builder.Append(this.Times[t].ToInvariantString());
                    builder.Append(']');
                }
            }

            if (this.Times.Count % 2 != 0)
            {
                builder.Append(',');
                builder.Append(' ');
                builder.Append('∞');
                builder.Append('[');
            }
            return builder.ToInvariantString();
        }
    }
}