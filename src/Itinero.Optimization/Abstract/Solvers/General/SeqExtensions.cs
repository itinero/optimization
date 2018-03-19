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
 
using System;
using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.Abstract.Solvers.General
{
    /// <summary>
    /// Contains extension methods related to sequences.
    /// </summary>
    public static class SeqExtensions
    {
        /// <summary>
        /// Turns the given sequence into a proper string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s">The sequence.</param>
        /// <returns></returns>
        public static string SeqToString<T>(this T[] s)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            if (s.Length > 0)
            {
                stringBuilder.Append(s[0].ToInvariantString());
                for (var i = 1; i < s.Length; i++)
                {
                    stringBuilder.Append(',');
                    stringBuilder.Append(s[i].ToInvariantString());
                }
            }
            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }
    }
}