using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Optimization.Models.Mapping;
using Itinero.Profiles;

namespace Itinero.Optimization.Tests.Mocks
{
    internal class WeightMatrixAlgorithmMock : IWeightMatrixAlgorithm<float>
    {
        public WeightMatrixAlgorithmMock(MappedModel mappedModel)
        {
            this.RouterPoints = new List<RouterPoint>(mappedModel.Visits.Select(
                x => new RouterPoint(x.Latitude, x.Longitude, 1, 1)));
        }
        
        public void Run()
        {
            this.HasRun = true;
            this.HasSucceeded = true;
        }

        public void Run(CancellationToken cancellationToken)
        {
            this.Run();
        }

        public bool HasRun { get; private set; }
        public bool HasSucceeded { get; private set; }
        public string ErrorMessage { get; } = string.Empty;
        
        public int OriginalIndexOf(int weightIdx)
        {
            return weightIdx;
        }

        public int CorrectedIndexOf(int resolvedIdx)
        {
            return resolvedIdx;
        }

        public IMassResolvingAlgorithm MassResolver { get; }

        public IProfileInstance Profile { get; } = new ProfileInstanceMock();
        
        public float[][] Weights { get; }

        public RouterBase Router { get; } = new RouterMock();
        
        public List<RouterPoint> RouterPoints { get; }

        public Dictionary<int, RouterPointError> Errors { get; } = new Dictionary<int, RouterPointError>();
    }
}