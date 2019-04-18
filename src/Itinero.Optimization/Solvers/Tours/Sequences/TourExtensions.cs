namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Contains extension methods related to tours & sequences.
    /// </summary>
    public static class TourExtensions
    {
        /// <summary>
        /// Inserts the given sequence after the given visit.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="visit">The visit.</param>
        /// <param name="sequence">The sequence.</param>
        public static void InsertAfter(this Tour tour, int visit, Sequence sequence)
        {
            for (var i = 0; i < sequence.Length; i++)
            {
                var next = sequence[i];
                tour.InsertAfter(visit, next);
                visit = next;
            }
        }
    }
}