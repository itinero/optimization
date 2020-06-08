using System.Collections.Generic;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.TSP_TW;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW
{
    public class TSPTWProblemTests
    {
        /// <summary>
        /// Tests a 'fixed' TSP-TW problem, weights should be identical.
        /// </summary>
        [Fact]
        public void TSPTWProblem_FixedProblemWeightsShouldBeIntact()
        {
            var problem = new TSPTWProblem(3, 1, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);
            
            Assert.Equal(3, problem.First);
            Assert.True(problem.Last.HasValue);
            Assert.Equal(1, problem.Last.Value);
            Assert.Equal(0, problem.Weight(0, 0));
            Assert.Equal(1, problem.Weight(0, 1));
            Assert.Equal(2, problem.Weight(0, 2));
            Assert.Equal(3, problem.Weight(0, 3));
            Assert.Equal(4, problem.Weight(1, 0));
            Assert.Equal(0, problem.Weight(1, 1));
            Assert.Equal(5, problem.Weight(1, 2));
            Assert.Equal(6, problem.Weight(1, 3));
            Assert.Equal(7, problem.Weight(2, 0));
            Assert.Equal(8, problem.Weight(2, 1));
            Assert.Equal(0, problem.Weight(2, 2));
            Assert.Equal(9, problem.Weight(2, 3));
            Assert.Equal(10, problem.Weight(3, 0));
            Assert.Equal(11, problem.Weight(3, 1));
            Assert.Equal(12, problem.Weight(3, 2));
            Assert.Equal(0, problem.Weight(3, 3));
        }
        
        /// <summary>
        /// Tests a 'closed' TSP-TW problem, weights should be identical.
        /// </summary>
        [Fact]
        public void TSPTWProblem_ClosedProblemWeightsShouldBeIntact()
        {
            var problem = new TSPTWProblem(1, 1, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);
            
            Assert.Equal(1, problem.First);
            Assert.True(problem.Last.HasValue);
            Assert.Equal(1, problem.Last.Value);
            Assert.Equal(0, problem.Weight(0, 0));
            Assert.Equal(1, problem.Weight(0, 1));
            Assert.Equal(2, problem.Weight(0, 2));
            Assert.Equal(3, problem.Weight(0, 3));
            Assert.Equal(4, problem.Weight(1, 0));
            Assert.Equal(0, problem.Weight(1, 1));
            Assert.Equal(5, problem.Weight(1, 2));
            Assert.Equal(6, problem.Weight(1, 3));
            Assert.Equal(7, problem.Weight(2, 0));
            Assert.Equal(8, problem.Weight(2, 1));
            Assert.Equal(0, problem.Weight(2, 2));
            Assert.Equal(9, problem.Weight(2, 3));
            Assert.Equal(10, problem.Weight(3, 0));
            Assert.Equal(11, problem.Weight(3, 1));
            Assert.Equal(12, problem.Weight(3, 2));
            Assert.Equal(0, problem.Weight(3, 3));
        }
        
        /// <summary>
        /// Tests a 'open' TSP-TW problem, weights should be identical.
        /// </summary>
        [Fact]
        public void TSPTWProblem_OpenProblemWeightsShouldBeIntact()
        {
            var problem = new TSPTWProblem(3, null, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);
            
            Assert.Equal(3, problem.First);
            Assert.Null(problem.Last);
            Assert.Equal(0, problem.Weight(0, 0));
            Assert.Equal(1, problem.Weight(0, 1));
            Assert.Equal(2, problem.Weight(0, 2));
            Assert.Equal(3, problem.Weight(0, 3));
            Assert.Equal(4, problem.Weight(1, 0));
            Assert.Equal(0, problem.Weight(1, 1));
            Assert.Equal(5, problem.Weight(1, 2));
            Assert.Equal(6, problem.Weight(1, 3));
            Assert.Equal(7, problem.Weight(2, 0));
            Assert.Equal(8, problem.Weight(2, 1));
            Assert.Equal(0, problem.Weight(2, 2));
            Assert.Equal(9, problem.Weight(2, 3));
            Assert.Equal(10, problem.Weight(3, 0));
            Assert.Equal(11, problem.Weight(3, 1));
            Assert.Equal(12, problem.Weight(3, 2));
            Assert.Equal(0, problem.Weight(3, 3));
        }
        
        /// <summary>
        /// Tests the weights of the 'closed' equivalent of an a 'open' TSP-TW problem.
        /// </summary>
        [Fact]
        public void TSPTWProblem_ClosedEquivalentForOpenProblemWeights()
        {
            var problem = new TSPTWProblem(2, null, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);
            
            // the closed equivalent of an open problem:
            // - the first is taken.
            // - the first is used as last.
            // - all weights to first are set to '0'.
            // - the rest of the weights are untouched.
            
            // the resulting tours are the exact expect solution after:
            // - remove the link last -> first.
            problem = problem.ClosedEquivalent; 

            // check visits, should be everything except 'last'.
            var visits = new HashSet<int>(problem.Visits);
            Assert.Equal(4, visits.Count);
            Assert.Contains(0, visits);
            Assert.Contains(1, visits);
            Assert.Contains(2, visits);
            Assert.Contains(3, visits);
            
            // check contains on problem.
            Assert.True(problem.Contains(0));
            Assert.True(problem.Contains(1));
            Assert.True(problem.Contains(2));
            Assert.True(problem.Contains(3));
              
            // check first/last.
            Assert.Equal(2, problem.First);
            Assert.Equal(2, problem.Last);
            
            // check counts/weight size.
            Assert.Equal(4, problem.WeightsSize);
            Assert.Equal(4, problem.Count);
            
            // check weights, all should be normal except x -> first should be 0.
            Assert.Equal(0, problem.Weight(0, 0));
            Assert.Equal(1, problem.Weight(0, 1));
            Assert.Equal(0, problem.Weight(0, 2)); // weight overriden.
            Assert.Equal(3, problem.Weight(0, 3));
            Assert.Equal(4, problem.Weight(1, 0));
            Assert.Equal(0, problem.Weight(1, 1));
            Assert.Equal(0, problem.Weight(1, 2)); // weight overriden.
            Assert.Equal(6, problem.Weight(1, 3));
            Assert.Equal(7, problem.Weight(2, 0));
            Assert.Equal(8, problem.Weight(2, 1));
            Assert.Equal(0, problem.Weight(2, 2)); // weight overriden.
            Assert.Equal(9, problem.Weight(2, 3));
            Assert.Equal(10, problem.Weight(3, 0));
            Assert.Equal(11, problem.Weight(3, 1));
            Assert.Equal(0, problem.Weight(3, 2)); // weight overriden.
            Assert.Equal(0, problem.Weight(3, 3));
        }

        /// <summary>
        /// Tests the windows of the 'closed' equivalent of an an 'open' TSP-TW problem.
        /// </summary>
        [Fact]
        public void TSPTWProblem_ClosedEquivalentForOpenProblemWindows()
        {
            var problem = new TSPTWProblem(2, null, new[]
            {
                new float[] {0, 1, 2, 3},
                new float[] {4, 0, 5, 6},
                new float[] {7, 8, 0, 9},
                new float[] {10, 11, 12, 0}
            }, new []
            {
                new TimeWindow()
                {
                    Times = new[] {0f, 1f}
                },
                new TimeWindow()
                {
                    Times = new[] {2f, 3f}
                },
                new TimeWindow()
                {
                    Times = new[] {4f, 5f}
                },
                new TimeWindow()
                {
                    Times = new[] {6f, 7f}
                }
            });
            
            // the stuff happens with the weights here but the timewindows should stay identical.
            var original = problem;
            problem = problem.ClosedEquivalent; 
            
            Assert.NotNull(problem.Windows);
            Assert.Equal(4, problem.Windows.Length);
            Assert.Equal(original.Windows[0].Min, problem.Windows[0].Min);
            Assert.Equal(original.Windows[0].Max, problem.Windows[0].Max);
            Assert.Equal(original.Windows[1].Min, problem.Windows[1].Min);
            Assert.Equal(original.Windows[1].Max, problem.Windows[1].Max);
            Assert.Equal(original.Windows[2].Min, problem.Windows[2].Min);
            Assert.Equal(original.Windows[2].Max, problem.Windows[2].Max);
            Assert.Equal(original.Windows[3].Min, problem.Windows[3].Min);
            Assert.Equal(original.Windows[3].Max, problem.Windows[3].Max);
        }

        /// <summary>
        /// Tests the weights of the 'closed' equivalent of an a 'fixed' TSP-TW problem.
        /// </summary>
        [Fact]
        public void TSPTWProblem_ClosedEquivalentForFixedProblemWeights()
        {
            var problem = new TSPTWProblem(2, 1, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);
            
            // the closed equivalent of a fixed problem:
            // - the first is taken as first.
            // - the first is used as last.
            // - the old last is removed from the visits.
            // - the weights to first are replaced by the weights to the old last.
            // - the rest of the weights are untouched.
            
            // the resulting tours are the exact expected solution after:
            // - replacing the last link first -> first with first -> old last.
            problem = problem.ClosedEquivalent;

            // check visits, should be everything except 'last'.
            var visits = new HashSet<int>(problem.Visits);
            Assert.Equal(3, visits.Count);
            Assert.Contains(0, visits);
            Assert.DoesNotContain(1, visits);
            Assert.Contains(2, visits);
            Assert.Contains(3, visits);
            
            // check contains on problem.
            Assert.True(problem.Contains(0));
            Assert.False(problem.Contains(1));
            Assert.True(problem.Contains(2));
            Assert.True(problem.Contains(3));
              
            // check first/last.
            Assert.Equal(2, problem.First);
            Assert.Equal(2, problem.Last);
            
            // check counts/weight size.
            Assert.Equal(4, problem.WeightsSize);
            Assert.Equal(3, problem.Count); // last was removed, only three left.
            
            // check weights, all should be normal except x -> first should be x -> old last.
            Assert.Equal(0, problem.Weight(0, 0));
            Assert.Equal(1, problem.Weight(0, 1)); // should not be used.
            Assert.Equal(1, problem.Weight(0, 2));
            Assert.Equal(3, problem.Weight(0, 3));
            Assert.Equal(4, problem.Weight(1, 0));
            Assert.Equal(0, problem.Weight(1, 1)); // should not be used.
            Assert.Equal(0, problem.Weight(1, 2));
            Assert.Equal(6, problem.Weight(1, 3));
            Assert.Equal(7, problem.Weight(2, 0));
            Assert.Equal(8, problem.Weight(2, 1)); // should not be used.
            Assert.Equal(8, problem.Weight(2, 2)); 
            Assert.Equal(9, problem.Weight(2, 3));
            Assert.Equal(10, problem.Weight(3, 0));
            Assert.Equal(11, problem.Weight(3, 1)); // should not be used.
            Assert.Equal(11, problem.Weight(3, 2));
            Assert.Equal(0, problem.Weight(3, 3));
        } 

        /// <summary>
        /// Tests the windows of the 'closed' equivalent of an a 'fixed' TSP-TW problem.
        /// </summary>
        [Fact]
        public void TSPTWProblem_ClosedEquivalentForFixedProblemWindows()
        {
            var problem = new TSPTWProblem(2, 1, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new []
            {
                new TimeWindow()
                {
                    Times = new[] {0f, 1f}
                },
                new TimeWindow()
                {
                    Times = new[] {2f, 3f}
                },
                new TimeWindow()
                {
                    Times = new[] {4f, 5f}
                },
                new TimeWindow()
                {
                    Times = new[] {6f, 7f}
                }
            });
            
            // the stuff happens with the weights here but the timewindows should stay identical.
            var original = problem;
            problem = problem.ClosedEquivalent; 
            
            Assert.NotNull(problem.Windows);
            Assert.Equal(4, problem.Windows.Length);
            Assert.Equal(original.Windows[0].Min, problem.Windows[0].Min);
            Assert.Equal(original.Windows[0].Max, problem.Windows[0].Max);
            Assert.Equal(original.Windows[1].Min, problem.Windows[1].Min);
            Assert.Equal(original.Windows[1].Max, problem.Windows[1].Max);
            Assert.Equal(original.Windows[2].Min, problem.Windows[2].Min);
            Assert.Equal(original.Windows[2].Max, problem.Windows[2].Max);
            Assert.Equal(original.Windows[3].Min, problem.Windows[3].Min);
            Assert.Equal(original.Windows[3].Max, problem.Windows[3].Max);
        }
    }
}