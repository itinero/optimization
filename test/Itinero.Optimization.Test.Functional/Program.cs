using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Itinero.Optimization.Test.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var problem = TSPHelper.CreateDirectedTSP(0, 0, 10, 10);

            var solution = problem.Solve();
        }
    }
}
