/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

namespace Itinero.Optimization.Models.TimeWindows
{
    /// <summary>
    /// Represents a timewindow.
    /// </summary>
    public class TimeWindow
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
        /// Returns a default timewindow with unlimited arrival/departure times.
        /// </summary>
        public static TimeWindow Unlimited => new TimeWindow()
        {
            Max = float.MaxValue,
            Min = float.MinValue
        };

        /// <summary>
        /// Returns true if this window is the default, unlimited window.
        /// </summary>
        public bool IsUnlimited => this.Max == float.MaxValue &&
                                   this.Min == float.MinValue;

        /// <summary>
        /// Returns true if this timewindows is considered empty or 'to be ignored'.
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