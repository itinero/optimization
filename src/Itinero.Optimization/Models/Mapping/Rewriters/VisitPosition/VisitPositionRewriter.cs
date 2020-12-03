using System;
using System.Linq;
using Itinero.Optimization.Models.Visits.Costs;

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
            var perVisitSettings = _settings.SettingsPerVisit;

            // get travel costs.
            var travelCosts = model.TravelCosts.FirstOrDefault(x =>
                x.Metric == Itinero.Optimization.Models.Metrics.Time);
            if (travelCosts == null) throw new ArgumentException($"No travel costs found with {nameof(travelCosts.Metric)} {Itinero.Optimization.Models.Metrics.Time}");
            
            if (!travelCosts.Directed) throw new ArgumentOutOfRangeException(nameof(mapping), 
                "Cannot prevent visits with positions using an undirected mapping.");

            // figure out what directions to prevent per visit.
            // an array with visits to prevent:
            // - null:  nothing to prevent.
            // - false: backward to prevent.
            // - true:  forward to prevent.
            var prevent = new bool?[model.Visits.Length];
            for (var v = 0; v < model.Visits.Length; v++)
            {
                // get settings and any per-visit settings.
                var visitAngleFunc = angleFunc;
                var preventLeft = _settings.Left;
                if (perVisitSettings != null)
                {
                    var visitSettings = perVisitSettings(v);
                    if (visitSettings == null) continue;

                    visitAngleFunc = visitSettings.AngleFunc ?? angleFunc;
                }
                
                // don't prevent by default.
                prevent[v] = null;
                
                // calculate angle for visit relative to edge direction.
                var vSnap = mapping.GetVisitSnapping(v);
                var angle = visitAngleFunc(vSnap);
                if (angle == null)
                {
                    // unclear on angle, cannot be prevented.
                    continue;
                }

                // determine left/right.
                var isLeft = !(angle > 0 && angle < 180);
                if (isLeft)
                {
                    // visit is on the left, prevent forward.
                    prevent[v] = preventLeft;
                }
                else 
                {
                    // visit is on the right, prevent backward.
                    prevent[v] = !preventLeft;
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
            for (var visit = 0; visit < model.Visits.Length; visit++)
            {
                // null, nothing to do
                // false, prevent backward
                // true, prevent forward.
                var visitPrevent = prevent[visit];
                if (visitPrevent == null) continue;

                for (var otherVisit = 0; otherVisit < model.Visits.Length; otherVisit++)
                {
                    //if (v1 == v2) continue;
                    
                    // treat otherVisit as from.
                    var bb = travelCosts.Costs[otherVisit * 2 + 0][visit * 2 + 0];
                    var bf = travelCosts.Costs[otherVisit * 2 + 0][visit * 2 + 1];
                    var fb = travelCosts.Costs[otherVisit * 2 + 1][visit * 2 + 0];
                    var ff = travelCosts.Costs[otherVisit * 2 + 1][visit * 2 + 1];

                    if (!visitPrevent.Value)
                    { // prevent backward.
                        bb = float.MaxValue;
                        fb = float.MaxValue;
                    }
                    else
                    { // prevent forward.
                        bf = float.MaxValue;
                        ff = float.MaxValue;
                    }
                    
                    if (ff >= float.MaxValue &&
                        fb >= float.MaxValue &&
                        bf >= float.MaxValue &&
                        bb >= float.MaxValue)
                    { // not reachable anymore, don't prevent direction.
                        
                    }
                    else
                    { // still reachable, prevent direction.
                        travelCosts.Costs[otherVisit * 2 + 0][visit * 2 + 0] = bb;
                        travelCosts.Costs[otherVisit * 2 + 0][visit * 2 + 1] = bf;
                        travelCosts.Costs[otherVisit * 2 + 1][visit * 2 + 0] = fb;
                        travelCosts.Costs[otherVisit * 2 + 1][visit * 2 + 1] = ff;
                    }

                    // treat otherVisit as to.
                    bb = travelCosts.Costs[visit * 2 + 0][otherVisit * 2 + 0];
                    bf = travelCosts.Costs[visit * 2 + 0][otherVisit * 2 + 1];
                    fb = travelCosts.Costs[visit * 2 + 1][otherVisit * 2 + 0];
                    ff = travelCosts.Costs[visit * 2 + 1][otherVisit * 2 + 1];
                    if (!visitPrevent.Value)
                    { // prevent backward.
                        bb = float.MaxValue;
                        bf = float.MaxValue;
                    }
                    else
                    { // prevent forward.
                        fb = float.MaxValue;
                        ff = float.MaxValue;
                    }
                    
                    if (ff >= float.MaxValue &&
                        fb >= float.MaxValue &&
                        bf >= float.MaxValue &&
                        bb >= float.MaxValue)
                    { // not reachable anymore, don't prevent direction.
                        
                    }
                    else
                    { // still reachable, prevent direction.
                        travelCosts.Costs[visit * 2 + 0][otherVisit * 2 + 0] = bb;
                        travelCosts.Costs[visit * 2 + 0][otherVisit * 2 + 1] = bf;
                        travelCosts.Costs[visit * 2 + 1][otherVisit * 2 + 0] = fb;
                        travelCosts.Costs[visit * 2 + 1][otherVisit * 2 + 1] = ff;
                    }
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