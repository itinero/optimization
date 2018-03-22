using System.Text;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Operators
{
    /// <summary>
    /// Represents a sequence.
    /// </summary>
    public struct Seq
    {
        /// <summary>
        /// Creates a new sequence with the given visits.
        /// </summary>
        /// <param name="visits"></param>
        public Seq(int[] visits)
        {
            this.Visits = visits;
            this.TotalOriginal = 0;
            this.Reversed = false;
            this.BetweenTravelCost = 0;
            this.BetweenVisitCost = 0;
            this.BetweenTravelCostReversed = 0;
        }

        /// <summary>
        /// Gets or sets the visits.
        /// </summary>
        /// <returns></returns>
        private int[] Visits { get; set; }

        /// <summary>
        /// The weight including the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float BetweenTravelCost { get; set; }

        /// <summary>
        /// The weight including the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float BetweenTravelCostReversed { get; set; }

        /// <summary>
        /// The visit cost of the visits between the first and last visit.
        /// </summary>
        public float BetweenVisitCost { get; set; }

        /// <summary>
        /// The total original weight.
        /// </summary>
        /// <returns></returns>
        public float TotalOriginal { get; set; }

        /// <summary>
        /// Gets or sets the reversed flag.
        /// </summary>
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets the total cost between the first and last visit.
        /// </summary>
        public float Between
        {
            get
            {
                if (this.Reversed)
                {
                    return this.BetweenTravelCostReversed + this.BetweenVisitCost;
                }
                return this.BetweenTravelCost + this.BetweenVisitCost;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.Visits.Length;
            }
        }

        /// <summary>
        /// Gets the visit at the given index (taking into account the reverse flag).
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int this[int i]
        {
            get
            {
                if (this.Reversed)
                {
                    if (i == 0)
                    {
                        return this.Visits[0];
                    }
                    if (i == this.Visits.Length - 1)
                    {
                        return this.Visits[this.Visits.Length - 1];
                    }
                    return this.Visits[this.Visits.Length - 1 - i];
                }
                return this.Visits[i];
            }
        }

        /// <summary>
        /// Returns a proper description of this sequence.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = new StringBuilder();
            res.Append(this[0]);
            res.Append("->");
            res.Append("[");
            for (var i = 1; i < this.Length - 2; i++)
            {
                res.Append("->");
                res.Append(this[i]);
            }
            res.Append("]");
            res.Append("->");
            res.Append(this[this.Length - 1]);
            return res.ToString();
        }
    }

    /// <summary>
    /// Contains extension methods for weights.
    /// </summary>
    public static class WeightExtensions
    {
        /// <summary>
        /// Calculates the weight of a sequence.
        /// </summary>
        /// <param name="weights">The weight matrix.</param>
        /// <param name="start">The start in the sequence.</param>
        /// <param name="end">The end in the sequence.</param>
        /// <param name="seq">The sequence.</param>
        /// <returns></returns>
        public static float SeqRange(this float[][] weights, int start, int end, Seq seq)
        {
            var res = 0f;
            for (var i = start + 1; i <= end; i++)
            {
                res += weights[seq[i - 1]][seq[i]];
            }
            return res;
        }
    }
}