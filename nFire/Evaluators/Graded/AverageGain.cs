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
    /// An evaluator for average gain and average gain after k documents retrieved (average-gain@k).
    /// For average-gain@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class AverageGain :
        IEvaluator<double, ISetResult>,
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "AG" or "AG@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "AG";
                else return "AG@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Average Gain" or "Average Gain at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Average Gain";
                else return "Average Gain at " + this.Cutoff;
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
        /// Gets and sets the cut-off k for average-gain@k. If null, average gain is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the auxiliary CG evaluator used to compute AG.
        /// </summary>
        protected CumulatedGain CG
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for average gain and average-gain@k.
        /// </summary>
        /// <param name="minScore">The minimum gain a judgment must have to contribute to the score.</param>
        /// <param name="cutoff">The cut-off k for average-gain@k.</param>
        public AverageGain(double minScore, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
            this.CG = new CumulatedGain();
        }

        /// <summary>
        /// Computes the AG score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The AG score.</returns>
        public double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return this.Evaluate(groundTruth, systemRun, this.MinScore, systemRun.Count);
        }
        /// <summary>
        /// Computes the AG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The AG@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return this.Evaluate(groundTruth, systemRun, this.MinScore, cut);
        }

        /// <summary>
        /// Computes the AG@k score of the specified run according to the specified ground truth, with the specified minimum relevance score and for the specified cut-off.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <param name="minScore">The minimum gain.</param>
        /// <param name="cutOff">The cut-off.</param>
        /// <returns>The AG@k score.</returns>
        protected double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun, double minScore, int cutOff)
        {
            this.CG.MinScore = minScore;
            this.CG.Cutoff = cutOff;
            double cg = this.CG.Evaluate(groundTruth, systemRun);
            return cg / cutOff;
        }
    }
}
