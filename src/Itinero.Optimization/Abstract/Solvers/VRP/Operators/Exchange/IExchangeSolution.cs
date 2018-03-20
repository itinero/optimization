using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange
{
    /// <summary>
    /// An abstract definition of a solution.
    /// </summary>
    public interface IExchangeSolution
    {
        /// <summary>
        /// Gets the # of tours in this solution.
        /// </summary>
        /// <returns></returns>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets the tour at the given index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns></returns>
        ITour Tour(int i);
    }
}