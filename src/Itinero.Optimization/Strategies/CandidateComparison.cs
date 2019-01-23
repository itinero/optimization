using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Contains functionality to compare candidates.
    /// </summary>
    internal static class CandidateComparison
    {
        /// <summary>
        /// Compares two candidates.
        /// </summary>
        /// <param name="candidate1">The first candidate.</param>
        /// <param name="candidate2">The second candidate.</param>
        /// <param name="comparison">A custom comparison function, if any.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. 
        ///  The return value has these meanings: 
        ///  Value Meaning Less than zero This instance precedes other in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as other. 
        ///  Greater than zero This instance follows other in the sort order.</returns>
        internal static int Compare<TCandidate>(TCandidate candidate1, TCandidate candidate2, Comparison<TCandidate> comparison = null)
        {
            // first try comparison function.
            if (comparison != null)
            {
                return comparison(candidate1, candidate2);
            }

            switch (candidate1)
            {
                // try generic comparable.
                case IComparable<TCandidate> candidate1ComparableGen:
                    return candidate1ComparableGen.CompareTo(candidate2);
                // try comparable.
                case IComparable candidate1Comparable:
                    return candidate1Comparable.CompareTo(candidate2);
            }

            throw new InvalidOperationException("The two given candidates cannot be compared, no method of comparison found.");
        }
    }
}