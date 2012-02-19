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
using System.Text;

using nFire.Core;
using nFire.Evaluators.Graded;

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// An evaluator for discounted cumulated gain and discounted cumulated gain after k documents retrieved (discounted-cumulated-gain@k).
    /// For discounted-cumulated-gain@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class DiscountedCumulatedGain :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "DCG" or "DCG@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "DCG";
                else return "DCG@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Discounted Cumulated Gain" or "Discounted Cumulated Gain at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Discounted Cumulated Gain";
                else return "Discounted Cumulated Gain at " + this.Cutoff;
            }
        }


        /// <summary>
        /// Gets and sets the minimum gain a judgment must have to contribute to the score.
        /// </summary>
        public double MinScore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the cut-off k for discounted-cumulated-gain@k. If null, average gain is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the base of the logarithm for the discount function.
        /// </summary>
        public double LogBase
        {
            get;
            set;
        }
        
        /// <summary>
        /// Creates an evaluator for discounted cumulated gain and discounted-cumulated-gain@k.
        /// </summary>
        /// <param name="minScore">The minimum gain a judgment must have to contribute to the score.</param>
        /// <param name="logBase">The base of the logarithm for the discount function.</param>
        /// <param name="cutoff">The cut-off k for discounted-cumulated-gain@k.</param>
        public DiscountedCumulatedGain(double minScore=1, double logBase = 2, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.LogBase = 2;
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the DCG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The DCG@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;

            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth)
                gains.Add(doc.Document.Id, doc.Score);

            double dcg = 0;
            for (int i = 0; i < Math.Min(systemRun.Count, cut); i++) {
                double g;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out g) && g>=this.MinScore) {
                    if (i + 1 < this.LogBase) dcg += g;
                    else 
                        dcg += g / Math.Log(i+1, this.LogBase);
                } else {
                    // Unjudged document
                }
            }

            return dcg;
        }
    }
}
