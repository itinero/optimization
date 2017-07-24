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
        public static string DataLocal = "overpass.osm";

        /// <summary>
        /// Downloads the data.
        /// </summary>
        public static void DownloadAll()
        {
            if (!File.Exists(Download.DataLocal))
            {
                ToFile(Download.DataLocal);
            }
        }

        /// <summary>
        /// Downloads a file if it doesn't exist yet.
        /// </summary>
        public static void ToFile(string filename)
        {
            if (!File.Exists(filename))
            {
                var client = new HttpClient();
                var content = new StringContent("data=<osm-script>\r\n  <union>\r\n    <query type=\"way\">\r\n      <has-kv k=\"highway\"/>\r\n      <polygon-query bounds=\"51.336388 4.777371 51.330221 4.763638 51.324000 4.765956 51.322444 4.780118 51.324000 4.794280 51.333385 4.793421  51.336388 4.777371\"/>\r\n    </query>\r\n    <query type=\"way\">\r\n      <has-kv k=\"highway\"/>\r\n      <polygon-query bounds=\"51.268989 4.761492 51.249545 4.757673 51.246993 4.770376 51.245650 4.786941 51.246885 4.802863 51.255024 4.816338 51.261604 4.817583 51.269338 4.815137 51.278010 4.799944 51.278735 4.777714 51.268989 4.761492\"/>\r\n    </query>\r\n  </union>\r\n  <print mode=\"body\"/>\r\n  <recurse type=\"down\"/>\r\n  <print mode=\"skeleton\"/>\r\n</osm-script>");

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