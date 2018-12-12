using System.Linq;
using BenchmarkDotNet.Attributes;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Hull;

namespace Itinero.Optimization.Tests.Benchmarks.Solvers.Tours.Hull
{
    public class QuickHullTests
    {
        private readonly Tour _tour1 = new Tour(Enumerable.Range(0, 19));
        private readonly double[,] _rawLocations1 = {
            {
                4.341386,
                51.8541
            },
            {
                4.342323,
                51.85313
            },
            {
                4.342957,
                51.85265
            },
            {
                4.33011,
                51.85273
            },
            {
                4.316804,
                51.85365
            },
            {
                4.315493,
                51.85392
            },
            {
                4.315526,
                51.85396
            },
            {
                4.315224,
                51.85316
            },
            {
                4.314861,
                51.85225
            },
            {
                4.309917,
                51.85264
            },
            {
                4.311972,
                51.85228
            },
            {
                4.310591,
                51.85429
            },
            {
                4.313155,
                51.85436
            },
            {
                4.312361,
                51.85533
            },
            {
                4.313437,
                51.85707
            },
            {
                4.311151,
                51.85653
            },
            {
                4.310231,
                51.85749
            },
            {
                4.312275,
                51.85863
            },
            {
                4.308912,
                51.85757
            }
        };
        
        [Benchmark]
        public TourHull ConvexHull1()
        {
            return _tour1.ConvexHull((i) => new Coordinate((float) _rawLocations1[i, 1], (float) _rawLocations1[i, 0]));
        }
    }
}