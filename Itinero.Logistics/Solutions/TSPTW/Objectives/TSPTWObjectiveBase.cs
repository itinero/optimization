using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itinero.Logistics.Solutions.TSPTW.Objectives
{
    /// <summary>
    /// Abstract base class for TSPTW objectives.
    /// </summary>
    public abstract class TSPTWObjectiveBase : ITSPTWObjective
    {
        /// <summary>
        /// Returns the name of this objective.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Calculates the fitness of a TSP solution.
        /// </summary>
        /// <returns></returns>
        public double Calculate(TSP.ITSP problem, Routes.IRoute solution)
        {
            return this.Calculate(problem as ITSPTW, solution);
        }

        /// <summary>
        /// Calculates the fitness of a TSP solution.
        /// </summary>
        /// <returns></returns>
        public abstract double Calculate(ITSPTW problem, Routes.IRoute solution);

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public abstract bool ShiftAfter(ITSPTW problem, Routes.IRoute route, int customer, int before, out double difference);
        
        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public abstract double IfShiftAfter(ITSPTW problem, Routes.IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter);

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public bool ShiftAfter(TSP.ITSP problem, Routes.IRoute route, int customer, int before, out double difference)
        {
            return this.ShiftAfter(problem as ITSPTW, route, customer, before, out difference);
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public double IfShiftAfter(TSP.ITSP problem, Routes.IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            return this.IfShiftAfter(problem, route, customer, before, oldBefore, oldAfter, newAfter);
        }
    }
}
