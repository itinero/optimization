using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators
{
    /// <summary>
    /// An intratour-operator which does do nothing.
    /// Usefull for testing
    /// </summary>
    public class NoIntraTourOperator<TWeight, TProblem, TObjective, TSolution, TFitness>: IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
    where TWeight : struct
    where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <inheritdoc />
        public string Name
        {
            get => "NoIntraTourOperator";
        }
        /// <summary>
        /// Returns true; everything is supported
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            delta = objective.Zero;
            return false;
        }
    }
}