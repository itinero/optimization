﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.Models.TimeWindows
{
    /// <summary>
    /// Represents a time window.
    /// </summary>
    /// <remarks>
    /// The times are interpreted as follows:
    /// - When even # of values, each pair is one window.
    /// - When uneven # of values the last value represents a window towards infinity.
    ///
    /// For example:
    /// - no values  : [0, ∞[
    /// - 1 value    : [x, ∞[
    /// - 2 values   : [x, y]
    /// - 3 values   : [x, y], [z, ∞[
    /// - 4 values   : [w, x], [y, z]
    ///
    /// </remarks>
    public class TimeWindow : IEnumerable<(float start, float end)>
    {
        /// <summary>
        /// Creates a new time window.
        /// </summary>
        /// <param name="times">The times.</param>
        public TimeWindow(IReadOnlyList<float>? times = null)
        {
            this.Times = times ?? Array.Empty<float>();
        }
        
        /// <summary>
        /// Gets or sets the times of the windows.
        /// </summary>
        public IReadOnlyList<float> Times { get; set; }

        /// <summary>
        /// Returns true if this time windows is considered empty or 'to be ignored'.
        /// </summary>
        public bool IsEmpty => this.Times == null || 
                               this.Times.Count == 0;

        /// <inheritdoc/>
        public IEnumerator<(float start, float end)> GetEnumerator()
        {
            return this.Times.ToRanges().GetEnumerator();
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.IsEmpty) return "[0, ∞[";
            
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}