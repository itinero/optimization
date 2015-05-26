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

namespace OsmSharp.Logistics.Routes
{
    /// <summary>
    /// Represents a pair (or a connection between two adjacent customers).
    /// </summary>
    public struct Pair
    {
        /// <summary>
        /// Creates a new pair.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Pair(int from, int to)
            : this()
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// Returns a description of this edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} -> {1}", this.From, this.To);
        }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                this.To.GetHashCode();
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair)
            {
                return ((Pair)obj).From == this.From &&
                    ((Pair)obj).To == this.To;
            }
            return false;
        }
    }
}