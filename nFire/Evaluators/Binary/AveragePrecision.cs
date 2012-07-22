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

using System;
using System.Collections.Generic;
using System.Linq;
using nFire.Core;

namespace nFire.Evaluators.Binary
{
    /// <summary>
    /// An evaluator for Average Precision (AP) and Average Precision after k documents retrieved (AP@k).
    /// For AP@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class AveragePrecision :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "AP" or "AP@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "AP";
                else return "AP@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Average Precision" or "Average Precision at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Average Precision";
                else return "Average Precision at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the minimum relevance score a document must have to be considered relevant.
        /// </summary>
        public double MinScore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the cut-off k for AP@k. If null, AP is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for AP and AP@k.
        /// </summary>
        /// <param name="minScore">The minimum relevance score a document must have to be considered relevant.</param>
        /// <param name="cutoff">The cut-off k for AP@k.</param>
        public AveragePrecision(double minScore = 1.0, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the AP@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The AP@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cutoff = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;

            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(j => j.Score >= this.MinScore).Select(j => j.Document.Id));

            double ap = 0;
            double relevantCount = 0;
            for (int i = 0; i < cutoff && i < systemRun.Count; i++) {
                if (relevant.Contains(systemRun.ElementAt(i).Document.Id)) {
                    relevantCount++;
                    ap += (relevantCount / (i + 1));
                }
            }

            return ap / relevant.Count; // In NTCIREVAL it is divided by min(R, cutoff) to ensure the upper bound is 1. We follow TREC's implementation instead.
        }
    }
}
