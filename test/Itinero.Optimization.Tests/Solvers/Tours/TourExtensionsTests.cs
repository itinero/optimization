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
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Hull;
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours
{
    /// <summary>
    /// Contains tests for the extensions related to tours.
    /// </summary>
    public class TourExtensionsTests
    {
        /// <summary>
        /// The from extension method should enumerate everything until from when the tour is closed.
        /// </summary>
        [Fact]
        public void TourExtensions_FromShouldEnumerateUntilFromWhenClosed()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            Assert.Equal(new[] {0, 1, 2, 3, 4}, tour.From(0));
            Assert.Equal(new[] {1, 2, 3, 4, 0}, tour.From(1));
            Assert.Equal(new[] {2, 3, 4, 0, 1}, tour.From(2));
            Assert.Equal(new[] {3, 4, 0, 1, 2}, tour.From(3));
            Assert.Equal(new[] {4, 0, 1, 2, 3}, tour.From(4));
        }

        [Fact]
        public void TourExtensions_ToGeoJsonShouldBeGeoJson()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.792957305908203,
                    51.26766141261736
                },
                {
                    4.795103073120117,
                    51.26234452724744
                },
                {
                    4.784202575683594,
                    51.258369887982454
                },
                {
                    4.780082702636719,
                    51.2625593628227
                },
                {
                    4.78729248046875,
                    51.263418695082386
                },
                {
                    4.79527473449707,
                    51.266265118440224
                },
                {
                    4.793901443481445,
                    51.26997057527367
                },
                {
                    4.778108596801758,
                    51.27120566115648
                },
                {
                    4.774675369262695,
                    51.266909567178125
                },
                {
                    4.782228469848633,
                    51.254556059682514
                },
                {
                    4.796304702758789,
                    51.257779033886585
                },
                {
                    4.796133041381836,
                    51.263687233118745
                },
                {
                    4.784717559814453,
                    51.26460025070665
                },
                {
                    4.777851104736328,
                    51.26680215968272
                },
                {
                    4.779396057128905,
                    51.270561272663755
                },
                {
                    4.8023128509521475,
                    51.273299861348995
                },
                {
                    4.799652099609375,
                    51.26374094053775
                },
                {
                    4.808921813964844,
                    51.26153888489712
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var tourGeoJson = tour.ToGeoJson((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal("{\"type\":\"FeatureCollection\",\"features\":[{\"type\":\"Feature\",\"name\":\"ShapeMeta\",\"geometry\":{\"type\":\"LineString\",\"coordinates\":[[4.769268,51.2646],[4.792957,51.26766],[4.795103,51.26234],[4.784203,51.25837],[4.780083,51.26256],[4.787292,51.26342],[4.795275,51.26627],[4.793901,51.26997],[4.778109,51.27121],[4.774675,51.26691],[4.782228,51.25455],[4.796305,51.25778],[4.796133,51.26369],[4.784718,51.2646],[4.777851,51.2668],[4.779396,51.27056],[4.802313,51.2733],[4.799652,51.26374],[4.808922,51.26154]]},\"properties\":{}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.769268,51.2646]},\"properties\":{\"visit\":0}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.792957,51.26766]},\"properties\":{\"visit\":1}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.795103,51.26234]},\"properties\":{\"visit\":2}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.784203,51.25837]},\"properties\":{\"visit\":3}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.780083,51.26256]},\"properties\":{\"visit\":4}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.787292,51.26342]},\"properties\":{\"visit\":5}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.795275,51.26627]},\"properties\":{\"visit\":6}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.793901,51.26997]},\"properties\":{\"visit\":7}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.778109,51.27121]},\"properties\":{\"visit\":8}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.774675,51.26691]},\"properties\":{\"visit\":9}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.782228,51.25455]},\"properties\":{\"visit\":10}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.796305,51.25778]},\"properties\":{\"visit\":11}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.796133,51.26369]},\"properties\":{\"visit\":12}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.784718,51.2646]},\"properties\":{\"visit\":13}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.777851,51.2668]},\"properties\":{\"visit\":14}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.779396,51.27056]},\"properties\":{\"visit\":15}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.802313,51.2733]},\"properties\":{\"visit\":16}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.799652,51.26374]},\"properties\":{\"visit\":17}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[4.808922,51.26154]},\"properties\":{\"visit\":18}}]}",
                tourGeoJson);
        }
    }
}