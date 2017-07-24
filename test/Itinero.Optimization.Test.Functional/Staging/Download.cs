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

using System.IO;
using System.Net.Http;

namespace Itinero.Optimization.Test.Functional.Staging
{

    /// <summary>
    /// Downloads all data needed for testing.
    /// </summary>
    public static class Download
    {
        /// <summary>
        /// Downloads a file if it doesn't exist yet.
        /// </summary>
        public static void ToFile(string queryName)
        {
            var filename = queryName + ".osm";
            var queryFileName = Path.Combine("Staging", queryName + ".txt");

            if (!File.Exists(queryFileName))
            {
                throw new System.Exception("Cannot stage data for query, query not found!");
            }

            var query = File.ReadAllText(queryFileName);
            if (!File.Exists(filename))
            {
                var client = new HttpClient();
                var content = new StringContent("data=" + query);
                var response = client.PostAsync(@"http://overpass-api.de/api/interpreter", content);
                using (var stream = response.GetAwaiter().GetResult().Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                using (var outputStream = File.OpenWrite(filename))
                {
                    stream.CopyTo(outputStream);
                }
            }
        }
    }
}