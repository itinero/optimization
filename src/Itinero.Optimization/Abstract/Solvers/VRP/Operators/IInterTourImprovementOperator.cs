using Itinero.Optimization.Algorithms.Solvers;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators
{
    /// <summary>
    /// Abstract definition of an inter-tour improvement operator.
    /// </summary>
    public interface IInterTourImprovementOperator<TWeight, TProblem, TObjective, TSolution, TFitness> : IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct 
    {
        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        /// <returns></returns>
        bool IsSymmetric
        {
            get;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="delta">The difference between the fitness value before and after the operation. The new fitness value can be calculated by subtracting the delta value from the old fitness value. This means a delta > 0 means an improvement in fitness when lower is better.</param>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns></returns>
        bool Apply(TProblem problem, TObjective objective, TSolution solution, 
            int t1, int t2, out float delta);
    }
}