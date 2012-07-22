// Copyright (C) 2012  Julián Urbano <urbano.julian@gmail.com>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Linq;
using nFire.Core;
using System;

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// An evaluator for Average Dynamic Recall (ADR) and Average Dynamic Recall after k documents retrieved (ADR@k).
    /// It is assumed that the results in the run are ordered by rank.
    /// It is also assumed that relevance levels are sorted in decreasing order.
    /// </summary>
    public class AverageDynamicRecall :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "ADR" or "ADR@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "ADR";
                else return "ADR@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Average Dynamic Recall" or "Average Dynamic Recall at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Average Dynamic Recall";
                else return "Average Dynamic Recall at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the cut-off k for ADR@k. If null, ADR is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for ADR and ADR@k.
        /// </summary>
        /// <param name="cutoff">The cut-off k for ADR@k.</param>
        public AverageDynamicRecall(int? cutoff = null)
        {
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the ADR@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// It is also assumed that relevance levels are sorted in decreasing order.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The ADR@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : Math.Min(systemRun.Count, (int)this.Cutoff);

            IListResult[] sortedRelevant = groundTruth.Where(r => r.Score > 0).OrderByDescending(r => r.Score).ToArray();
            cut = Math.Min(cut, sortedRelevant.Length); // If cut > |R|, sum up to |R| ranks
            string[] retrieved = systemRun.Take(cut).Select(r => r.Document.Id).ToArray();

            double adr = 0;
            for (int i = 0; i < cut; i++) {
                IListResult[] allowed = sortedRelevant;
                if (i < sortedRelevant.Length) {
                    allowed = allowed.Where(r => r.Score >= sortedRelevant.ElementAt(i).Score).ToArray();
                }
                double allowedRetrieved = retrieved.Take(i + 1).Intersect(allowed.Select(r => r.Document.Id)).Count();
                adr += allowedRetrieved / (i + 1);
            }

            return adr / cut;
        }
    }
}
