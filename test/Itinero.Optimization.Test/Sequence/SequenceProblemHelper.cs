namespace Itinero.Optimization.Test.Sequence
{
    public static class SequenceProblemHelper
    {
        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        public static Optimization.Sequence.Directed.SequenceDirectedProblem Create(int[] sequence, int size, bool closed, float turnPenalty, float defaultWeight)
        {
            var weights = new float[size * 2][];
            for (int x = 0; x < size * 2; x++)
            {
                weights[x] = new float[size * 2];
                var xDirection = (x % 2);
                for (int y = 0; y < size * 2; y++)
                {
                    var yDirection = (y % 2);
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                    else
                    {
                        if (xDirection == yDirection)
                        {
                            weights[x][y] = defaultWeight;
                        }
                        else
                        {
                            weights[x][y] = defaultWeight + (int)(defaultWeight * 0.1);
                        }
                    }
                }
            }
            return new Optimization.Sequence.Directed.SequenceDirectedProblem(sequence, closed, weights, turnPenalty);
        }
    }
}
