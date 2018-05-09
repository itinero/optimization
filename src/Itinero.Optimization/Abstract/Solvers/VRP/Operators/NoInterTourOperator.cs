using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators
{
    /// <summary>
    /// An inter tour operator which does nothing. Used for debugging
    /// </summary>
    /// <typeparam name="TWeight"></typeparam>
    /// <typeparam name="TProblem"></typeparam>
    /// <typeparam name="TObjective"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    /// <typeparam name="TFitness"></typeparam>
    public class NoInterTourOperator<TWeight, TProblem, TObjective, TSolution, TFitness> : IInterTourImprovementOperator
        <TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <summary>
        /// A name for this nothing operator
        /// </summary>
        public string Name => "NoOperation";

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
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            delta = objective.Zero;
            return false;
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, int t1, int t2, out float delta)
        {
            delta = 0f;
            return false;
        }


        /// <summary>
        /// Returns true
        /// </summary>
        public bool IsSymmetric => true;
    }
}