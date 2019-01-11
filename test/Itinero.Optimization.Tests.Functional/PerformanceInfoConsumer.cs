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
using System.Diagnostics;
using System.Linq;

namespace Itinero.Optimization.Tests.Functional
{
    /// <summary>
    /// A class that consumes perfomance information.
    /// </summary>
    public class PerformanceInfoConsumer
    {
        /// <summary>
        /// Holds the name of this consumer.
        /// </summary>
        private string _name;

        /// <summary>
        /// Holds the memory usage timer.
        /// </summary>
        private System.Threading.Timer _memoryUsageTimer;

        /// <summary>
        /// Holds the memory usage log.
        /// </summary>
        private List<double> _memoryUsageLog = new List<double>();

        /// <summary>
        /// Holds the time spent on logging memory usage.
        /// </summary>
        private long _memoryUsageLoggingDuration = 0;

        /// <summary>
        /// Creates the a new performance info consumer.
        /// </summary>
        /// <param name="name"></param>
        public PerformanceInfoConsumer(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates the a new performance info consumer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="memUseLoggingInterval"></param>
        public PerformanceInfoConsumer(string name, int memUseLoggingInterval)
        {
            _name = name;
            _memoryUsageTimer = new System.Threading.Timer(LogMemoryUsage, null, memUseLoggingInterval, memUseLoggingInterval);
        }

        /// <summary>
        /// Called when it's time to log memory usage.
        /// </summary>
        /// <param name="state"></param>
        private void LogMemoryUsage(object state)
        {
            long ticksBefore = DateTime.Now.Ticks;
            lock (_memoryUsageLog)
            {
                //GC.Collect();
                var p = Process.GetCurrentProcess();
                _memoryUsageLog.Add(System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4));

                _memoryUsageLoggingDuration = _memoryUsageLoggingDuration + (DateTime.Now.Ticks - ticksBefore);
            }
        }

        /// <summary>
        /// Creates a new performance consumer.
        /// </summary>
        /// <param name="key"></param>
        public static PerformanceInfoConsumer Create(string key)
        {
            return new PerformanceInfoConsumer(key);
        }

        /// <summary>
        /// Holds the ticks when started.
        /// </summary>
        private long? _ticks;

        /// <summary>
        /// Holds the amount of memory before start.
        /// </summary>
        private long? _memory;

        /// <summary>
        /// Reports the start of the process/time period to measure.
        /// </summary>
        public void Start()
        {
            //GC.Collect();

            var p = Process.GetCurrentProcess();
            _memory = p.PrivateMemorySize64;
            _ticks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        public void Report(string message)
        {
            Itinero.Logging.Logger.Log("PF:" + _name, Itinero.Logging.TraceEventType.Information,
                message);
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Report(string message, params object[] args)
        {
            Itinero.Logging.Logger.Log("PF:" + _name, Itinero.Logging.TraceEventType.Information,
                message, args);
        }

        /// <summary>
        /// Reports the end of the process/time period to measure.
        /// </summary>
        public void Stop(bool report = true)
        {
            if (_memoryUsageTimer != null)
            { // only dispose and stop when there IS a timer.
                _memoryUsageTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _memoryUsageTimer.Dispose();
            }
            if (_ticks.HasValue)
            {
                lock (_memoryUsageLog)
                {
                    var seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value - _memoryUsageLoggingDuration).TotalMilliseconds / 1000.0;

                    //GC.Collect();
                    var p = Process.GetCurrentProcess();
                    var memoryDiff = System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4);

                    if (_memoryUsageLog.Count > 0)
                    { // there was memory usage logging.
                        double max = _memoryUsageLog.Max();
                        if (report)
                        {
                            Itinero.Logging.Logger.Log("PF:" + _name, Itinero.Logging.TraceEventType.Information,
                                string.Format("Ended at at {0}, spent {1}s and {2}MB of memory diff with {3}MB max used.",
                                    new DateTime(_ticks.Value).ToInvariantString(),
                                    seconds, memoryDiff, max));
                        }
                    }
                    else
                    { // no memory usage logged.
                        if (report)
                        {
                            Itinero.Logging.Logger.Log("PF:" + _name, Itinero.Logging.TraceEventType.Information,
                            string.Format("Ended at at {0}, spent {1}s and {2}MB of memory diff.",
                                new DateTime(_ticks.Value).ToInvariantString(),
                                seconds, memoryDiff));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts a new performance consumer and returns the instance that just started.
        /// </summary>
        /// <returns></returns>
        public static PerformanceInfoConsumer StartNew(string name)
        {
            var performance = new PerformanceInfoConsumer(name);
            performance.Start();
            return performance;
        }
    }

    /// <summary>
    /// Extension methods for the performance info class.
    /// </summary>
    public static class PerformanceInfoConsumerExtensions
    {
        /// <summary>
        /// Tests performance for the given action.
        /// </summary>
        public static void TestPerf(this Action action, string name)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            action();
            info.Stop();
        }

        /// <summary>
        /// Tests performance for the given action.
        /// </summary>
        public static void TestPerf(this Action action, string name, int count)
        {
            var info = new PerformanceInfoConsumer(name + " x " + count.ToInvariantString(), 10000);
            info.Start();
            while (count > 0)
            {
                action();
                count--;
            }
            info.Stop();
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<T> func, string name)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var res = func();
            info.Stop();
            return res;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<T> func, string name, int count)
        {
            var res = default(T);
            var info = new PerformanceInfoConsumer(name + " x " + count.ToInvariantString(), 10000);
            info.Start();
            while (count > 0)
            {
                res = func();
                count--;
            }
            info.Stop();
            return res;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static TResult TestPerf<T, TResult>(this Func<T, TResult> func, string name, T a)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var res = func(a);
            info.Stop();
            return res;
        }
    }
}