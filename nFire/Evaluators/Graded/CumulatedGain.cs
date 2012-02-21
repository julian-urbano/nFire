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
    /// An evaluator for cumulated gain and cumulated gain after k documents retrieved (cumulated-gain@k).
    /// For cumulated-gain@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class CumulatedGain :
        IEvaluator<double, ISetResult>,
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "CG" or "CG@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "CG";
                else return "CG@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Cumulated Gain" or "Cumulated Gain at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Cumulated Gain";
                else return "Cumulated Gain at " + this.Cutoff;
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
        /// Gets and sets the cut-off k for cumulated-gain@k. If null, cumulated gain is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for cumulated gain and cumulated-gain@k.
        /// </summary>
        /// <param name="minScore">The minimum gain a judgment must have to contribute to the score.</param>
        /// <param name="cutoff">The cut-off k for cumulated-gain@k.</param>
        public CumulatedGain(double minScore = 0, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the CG score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The CG score.</returns>
        public double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return this.Evaluate(groundTruth, systemRun, this.MinScore, systemRun.Count);
        }
        /// <summary>
        /// Computes the CG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The CG@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return this.Evaluate(groundTruth, systemRun,this.MinScore, cut);
        }

        /// <summary>
        /// Computes the CG@k score of the specified run according to the specified ground truth, with the specified minimum relevance score and for the specified cut-off.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <param name="minScore">The minimum gain.</param>
        /// <param name="cutOff">The cut-off.</param>
        /// <returns>The CG@k score.</returns>
        protected double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun, double minScore, int cutOff)
        {
            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth)
                gains.Add(doc.Document.Id, doc.Score);

            double cg = 0;
            for (int i = 0; i < Math.Min(systemRun.Count, cutOff); i++) {
                double g;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out g) && g>= minScore)
                    cg += g;
            }

            if (cutOff == 0) return 0;
            return cg;
        }
    }
}
