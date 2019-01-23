namespace Itinero.Optimization.Tests.Functional.Performance
{
    /// <summary>
    /// An object containing feedback from a tested function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PerformanceTestResult<T>
    {
        /// <summary>
        /// Creates a new peformance test result.
        /// </summary>
        /// <param name="result"></param>
        public PerformanceTestResult(T result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public T Result { get; set; }
    }
}