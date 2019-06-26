using System;

namespace Itinero.Optimization.Tests.Functional.Performance
{
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
            info.Stop(string.Empty);
        }

        /// <summary>
        /// Tests performance for the given action.
        /// </summary>
        public static void TestPerf(this Action action, string name, int count)
        {
            var info = new PerformanceInfoConsumer(name + " x " + count.ToString(), 10000, count);
            info.Start();
            var message = string.Empty;
            while (count > 0)
            {
                action();
                count--;
            }

            info.Stop(message);
        }

        /// <summary>
        /// Tests performance for the given action.
        /// </summary>
        public static void TestPerf(this Func<string> action, string name)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var message = action();
            info.Stop(message);
        }

        /// <summary>
        /// Tests performance for the given action.
        /// </summary>
        public static void TestPerf(this Func<string> action, string name, int count)
        {
            var info = new PerformanceInfoConsumer(name + " x " + count.ToString(), 10000);
            info.Start();
            var message = string.Empty;
            while (count > 0)
            {
                message = action();
                count--;
            }

            info.Stop(message);
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<PerformanceTestResult<T>> func, string name)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var res = func();
            info.Stop(res.Message);
            return res.Result;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<PerformanceTestResult<T>> func, string name, int count)
        {
            var info = new PerformanceInfoConsumer(name + " x " + count.ToString(), 10000);
            info.Start();
            PerformanceTestResult<T> res = null;
            while (count > 0)
            {
                res = func();
                count--;
            }

            info.Stop(res.Message);
            return res.Result;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static TResult TestPerf<T, TResult>(this Func<T, PerformanceTestResult<TResult>> func, string name, T a)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var res = func(a);
            info.Stop(res.Message);
            return res.Result;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static TResult TestPerf<T, TResult>(this Func<T, PerformanceTestResult<TResult>> func, string name, T a, int count)
        {
            var info = new PerformanceInfoConsumer(name, count);
            info.Start();
            var res = func(a);
            count--;
            while (count > 0)
            {
                res = func(a);
                count--;
            }
            info.Stop(res.Message);
            return res.Result;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<T> func, string name)
        {
            var info = new PerformanceInfoConsumer(name);
            info.Start();
            var res = func();
            info.Stop(string.Empty);
            return res;
        }

        /// <summary>
        /// Tests performance for the given function.
        /// </summary>
        public static T TestPerf<T>(this Func<T> func, string name, int count)
        {
            var res = default(T);
            var info = new PerformanceInfoConsumer(name + " x " + count.ToInvariantString(), count);
            info.Start();
            while (count > 0)
            {
                res = func();
                count--;
            }
            info.Stop(string.Empty);
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
            info.Stop(string.Empty);
            return res;
}
    }
}