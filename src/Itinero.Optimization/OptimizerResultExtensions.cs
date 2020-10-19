using System;
using System.Collections.Generic;

namespace Itinero.Optimization
{
    /// <summary>
    /// Extensions methods to handle optimizer results.
    /// </summary>
    public static class OptimizerResultExtensions
    {
        /// <summary>
        /// Gets the routes representing the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The routes.</returns>
        public static IEnumerable<Result<Route>> GetRoutes(this OptimizerResult result)
        {
            if (result.IsError) throw new Exception($"{nameof(result)} in error, check {nameof(result.IsError)}.");
            var mapping = result.Mapping;
            if (mapping == null) throw new Exception($"No mapping found on result even though it's not in error.");
            var solution = result.Solution;
            if (solution == null) throw new Exception($"No mapping found on result even though it's not in error.");
            return mapping.BuildRoutes(solution);
        }
    }
}