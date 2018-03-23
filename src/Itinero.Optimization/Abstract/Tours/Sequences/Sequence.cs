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

using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.Abstract.Tours.Sequences
{
    /// <summary>
    /// Represents a sequence.
    /// </summary>
    public struct Sequence
    {
        private readonly int[] _visits;
        private readonly int _start;
        private readonly int _length;

        /// <summary>
        /// Creates a new sequence.
        /// </summary>
        /// <param name="visits">The visits.</param>
        public Sequence(int[] visits)
        {
            _visits = visits;
            _start = 0;
            _length = visits.Length;
        }

        /// <summary>
        /// Creates a new sequence.
        /// </summary>
        /// <param name="visits">The visits.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        public Sequence(int[] visits, int start, int length)
        {
            _visits = visits;
            _start = start;
            _length = length;
        }

        /// <summary>
        /// Gets the # of visits in this sequence.
        /// </summary>
        /// <returns></returns>
        public int Length
        {
            get
            {
                return _length;
            }
        }

        /// <summary>
        /// Gets the first visit.
        /// </summary>
        /// <returns></returns>
        public int First
        {
            get
            {
                return this [0];
            }
        }

        /// <summary>
        /// Gets the last visit.
        /// </summary>
        /// <returns></returns>
        public int Last
        {
            get
            {
                return this [_length - 1];
            }
        }

        /// <summary>
        /// Returns true when this sequence wraps around the beginning of the source sequence.
        /// </summary>
        public bool Wraps
        {
            get
            {
                return _start + _length > _visits.Length;
            }
        }

        /// <summary>
        /// Returns the visit at the given index.
        /// </summary>
        /// <param name="i">The index.</param>
        public int this [int i]
        {
            get
            {
                i = _start + i;
                if (i >= _visits.Length)
                {
                    i -= _visits.Length;
                }
                return _visits[i];
            }
        }

        /// <summary>
        /// Returns a new sequence that's a part of the given sequence.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public Sequence SubSequence(int start, int length)
        {
            start += _start;
            if (start >= _visits.Length)
            {
                start -= _visits.Length;
            }
            return new Sequence(_visits, start, length);
        }

        /// <summary>
        /// Enumerates all sub sequences.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="wrap">Wrap around the beginning of the sequence if true.</param>
        /// <returns></returns>
        public IEnumerable<Sequence> SubSequences(int minLength, int maxLength, bool wrap)
        {
            for (var length = maxLength; length >= minLength; length--)
            {
                var maxStart = _length;
                if (!wrap)
                {
                    maxStart = _length - (length - 1);
                }
                for (var s = 0; s < maxStart; s++)
                {
                    yield return new Sequence(_visits, s, length);
                }
            }
        }

        /// <summary>
        /// Returns a proper description of this sequence.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = new StringBuilder();
            if (this.Length > 0)
            {
                res.Append(this [0]);
                for (var i = 1; i < this.Length; i++)
                {
                    res.Append("->");
                    res.Append(this [i]);
                }
            }
            return res.ToString();
        }
    }
}