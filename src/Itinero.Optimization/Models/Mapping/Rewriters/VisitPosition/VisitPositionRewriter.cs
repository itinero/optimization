using System;
using System.Linq;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Profiles;

namespace Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition
{
    /// <summary>
    /// A rewriter to prevent/enforce visit with a given position.
    /// </summary>
    public class VisitPositionRewriter : IMappedModelRewriter
    {
        private readonly VisitPositionRewriterSettings _settings;

        /// <summary>
        /// Creates a new rewriter.
        /// </summary>
        /// <param name="settings">The rewriter settings.</param>
        public VisitPositionRewriter(VisitPositionRewriterSettings? settings = null)
        {
            _settings = settings ??= new VisitPositionRewriterSettings();
        }

        /// <inheritdoc/>
        public string Name => "VISIT_POS";
        
        /// <inheritdoc/>
        public MappedModel Rewrite(MappedModel model, IModelMapping mapping)
        {
            var angleFunc = _settings.AngleFunc ?? (p => p.RelativeAngle(mapping.RouterDb));

            // get travel costs.
            var travelCosts = model.TravelCosts.FirstOrDefault(x =>
                x.Metric == Itinero.Optimization.Models.Metrics.Time);
            if (travelCosts == null) throw new ArgumentException($"No travel costs found with {nameof(travelCosts.Metric)} {Itinero.Optimization.Models.Metrics.Time}");
            
            if (!travelCosts.Directed) throw new ArgumentOutOfRangeException(nameof(mapping), 
                "Cannot prevent visits with positions using an undirected mapping.");

            // figure out what directions to prevent per visit.
            var prevent = new bool[model.Visits.Length * 2];
            for (var v = 0; v < model.Visits.Length; v++)
            {
                // don't prevent by default.
                prevent[v * 2 + 0] = false;
                prevent[v * 2 + 1] = false;
                
                // calculate angle for visit relative to edge direction.
                var vSnap = mapping.GetVisitSnapping(v);
                var angle = angleFunc(vSnap);
                if (angle == null)
                {
                    // unclear on angle, cannot be prevented.
                    continue;
                }

                // determine left/right.
                var isLeft = angle > 0 && angle < 180;
                if (isLeft)
                {
                    // visit is on the left.
                    // prevent forward if left is to be prevented.
                    prevent[v * 2 + 0] = _settings.Left;
                    // prevent backward if right is to be prevented.
                    prevent[v * 2 + 1] = !_settings.Left;
                }
                else 
                {
                    // visit is on the right.
                    // prevent forward if right is to be prevented.
                    prevent[v * 2 + 0] = !_settings.Left;
                    // prevent backward if left is to be prevented.
                    prevent[v * 2 + 1] = _settings.Left;
                }
            }

            // clone travel costs matrix.
            var rewrittenTravelCosts = new TravelCostMatrix()
            {
                Costs = new float[travelCosts.Costs.Length][],
                Metric = Metrics.Time,
                Directed = true
            };
            for (var i = 0; i < travelCosts.Costs.Length; i++)
            {
                rewrittenTravelCosts.Costs[i] = new float[travelCosts.Costs[i].Length];
                for (var j = 0; j < travelCosts.Costs[i].Length; j++)
                {
                    rewrittenTravelCosts.Costs[i][j] = travelCosts.Costs[i][j];
                }
            }
            travelCosts = rewrittenTravelCosts;

            // rewrite costs.
            for (var v1 = 0; v1 < model.Visits.Length; v1++)
            {
                var v1f = prevent[v1 * 2 + 0];
                var v1b = prevent[v1 * 2 + 1];
                for (var v2 = 0; v2 < model.Visits.Length; v2++)
                {
                    if (v1 == v2) continue;
                    
                    var v2f = prevent[v2 * 2 + 0];
                    var v2b = prevent[v2 * 2 + 1];

                    var ff = travelCosts.Costs[v1 * 2 + 0][v2 * 2 + 0];
                    var fb = travelCosts.Costs[v1 * 2 + 0][v2 * 2 + 1];
                    var bf = travelCosts.Costs[v1 * 2 + 1][v2 * 2 + 0];
                    var bb = travelCosts.Costs[v1 * 2 + 1][v2 * 2 + 1];

                    if (v1f)
                    {
                        ff = float.MaxValue;
                        fb = float.MaxValue;
                    }
                    if (v1b)
                    {
                        bf = float.MaxValue;
                        bb = float.MaxValue;
                    }
                    if (v2f)
                    {
                        ff = float.MaxValue;
                        bf = float.MaxValue;
                    }
                    if (v2b)
                    {
                        fb = float.MaxValue;
                        bb = float.MaxValue;
                    }
                    
                    if (ff >= float.MaxValue &&
                        fb >= float.MaxValue &&
                        bf >= float.MaxValue &&
                        bb >= float.MaxValue)
                    {
                        continue;
                    }
                    
                    travelCosts.Costs[v1 * 2 + 0][v2 * 2 + 0] = ff;
                    travelCosts.Costs[v1 * 2 + 0][v2 * 2 + 1] = fb;
                    travelCosts.Costs[v1 * 2 + 1][v2 * 2 + 0] = bf;
                    travelCosts.Costs[v1 * 2 + 1][v2 * 2 + 1] = bb;
                }
            }

            // replace the new travel costs.
            var travelCostMatrices = new TravelCostMatrix[model.TravelCosts.Length];
            for (var t = 0; t < travelCostMatrices.Length; t++)
            {
                var tc = model.TravelCosts[t];
                travelCostMatrices[t] = tc;
                if (tc.Metric == Metrics.Time)
                {
                    travelCostMatrices[t] = travelCosts;
                }
            }
            
            // return the rewritten model.
            var rewrittenModel = new MappedModel()
            {
                TravelCosts = travelCostMatrices,
                Visits = model.Visits,
                VehiclePool = model.VehiclePool
            };

            return rewrittenModel;
        }
    }
}