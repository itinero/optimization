namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Abstract representation of a readonly tour built by removing a sequence.
    /// </summary>
    public interface IReadOnlyTourWithoutSequence : IReadOnlyTour
    {
        /// <summary>
        /// Returns true if the removed sequence contains the first visit of the original tour.
        /// </summary>
        bool Wraps { get; }
    }
}