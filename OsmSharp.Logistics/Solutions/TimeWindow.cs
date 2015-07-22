// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace OsmSharp.Logistics.Solutions
{
    /// <summary>
    /// Represents a timewindow.
    /// </summary>
    public struct TimeWindow
    {
        /// <summary>
        /// The minimum time in seconds.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// The maximum time in seconds.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Returns true if this window is valid at the given seconds.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public bool IsValidAt(double seconds)
        {
            return this.Min <= seconds && this.Max >= seconds;
        }

        /// <summary>
        /// Returns the minimum difference.
        /// </summary>
        /// <param name="seconds">The time.</param>
        /// <returns></returns>
        public double MinDiff(double seconds)
        {
            if(this.Min <= seconds && this.Max >= seconds)
            { // the time is within the window, no difference.
                return 0;
            }
            if(seconds < this.Min)
            { // time window too late.
                return this.Min - seconds;
            }
            return seconds - this.Max;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}]",
                this.Min, this.Max);
        }
    }
}