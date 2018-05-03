using System;
using BenchmarkDotNet.Attributes;

namespace Itinero.Optimization.Tests.Benchmarks.Strategies
{
    /// <summary>
    /// Contains benchmark methods from candidate comparison.
    /// </summary>
    public class CandidateComparisonBenchmark
    {
        private readonly CandidateGenericIComparable Struct1 = new CandidateGenericIComparable(1);
        private readonly CandidateGenericIComparable Struct0 = new CandidateGenericIComparable(0);
        private readonly CandidateGenericIComparable Class1 = new CandidateGenericIComparable(1);
        private readonly CandidateGenericIComparable Class0 = new CandidateGenericIComparable(0);

        [Benchmark]
        public int CompareStructsGenericIComparable()
        {
            return Itinero.Optimization.Strategies.CandidateComparison.Compare(
                Struct0, Struct1);
        }

        [Benchmark]
        public int CompareClassGenericIComparable()
        {
            return Itinero.Optimization.Strategies.CandidateComparison.Compare(
                Class0, Class1);
        }
    }

    class CandidateGenericIComparableClass : IComparable<CandidateGenericIComparable>
    {
        public CandidateGenericIComparableClass(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }

        public int CompareTo(CandidateGenericIComparable other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }

    struct CandidateGenericIComparable : IComparable<CandidateGenericIComparable>
    {
        public CandidateGenericIComparable(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }

        public int CompareTo(CandidateGenericIComparable other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}