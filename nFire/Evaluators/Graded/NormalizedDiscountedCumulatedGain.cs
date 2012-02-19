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
    /// An evaluator for normalized discounted cumulated gain and normalized discounted cumulated gain after k documents retrieved (normalized-discounted-cumulated-gain@k).
    /// For normalized-discounted-cumulated-gain@k it is assumed that the results in the run are ordered by rank.
    /// <remarks>This implementation is based on the discount function in the original paper by Järvelin and Kekäläinen in ACM TOIS, where documents ranked above the log base are not discounted.</remarks>
    /// </summary>
    public class NormalizedDiscountedCumulatedGain :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "nDCG" or "nDCG@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "nDCG";
                else return "nDCG@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Normalized Discounted Cumulated Gain" or "Normalized Discounted Cumulated Gain at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Normalized Discounted Cumulated Gain";
                else return "Normalized Discounted Cumulated Gain at " + this.Cutoff;
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
        /// Gets and sets the cut-off k for normalized-discounted-cumulated-gain@k. If null, average gain is computed, with not cut-off.
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
        /// Gets and sets the auxiliary DCG evaluator to compute nDCG;
        /// </summary>
        protected DiscountedCumulatedGain DCG
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for normalized discounted cumulated gain and normalized-discounted-cumulated-gain@k.
        /// </summary>
        /// <param name="minScore">The minimum gain a judgment must have to contribute to the score.</param>
        /// <param name="logBase">The base of the logarithm for the discount function.</param>
        /// <param name="cutoff">The cut-off k for normalized-discounted-cumulated-gain@k.</param>
        public NormalizedDiscountedCumulatedGain(double minScore=1, double logBase = 2, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.LogBase = 2;
            this.Cutoff = cutoff;
            this.DCG = new DiscountedCumulatedGain();
        }

        /// <summary>
        /// Computes the nDCG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The nDCG@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            this.DCG.LogBase = this.LogBase;
            this.DCG.Cutoff = this.Cutoff;

            double dcg = this.DCG.Evaluate(groundTruth, systemRun);
            nFire.Base.Run<IListResult> ideal = new Base.Run<IListResult>(groundTruth.Query, groundTruth.System);
            foreach (IListResult res in groundTruth.OrderByDescending(r => r.Score)) {
                ideal.Add(res);
            }
            double idcg = this.DCG.Evaluate(groundTruth, ideal);

            return dcg / idcg;
        }
    }
}
