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

namespace nFire.Evaluators.Binary
{
    /// <summary>
    /// An evaluator for recall and recall after k documents retrieved (recall@k).
    /// For recall@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class Recall :
        IEvaluator<double,ISetResult>,
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "R" or "R@k".
        /// </summary>
        public string ShortName
        {
            get {
                if (this.Cutoff == null) return "R";
                else return "R@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Recall" or "Recall at k".
        /// </summary>
        public string FullName
        {
            get {
                if (this.Cutoff == null) return "Recall";
                else return "Recall at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the minimum score a judgment must have to be considered relevant.
        /// </summary>
        public double MinScore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the cut-off k for recall@k. If null, recall is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for recall and recall@k.
        /// </summary>
        /// <param name="minScore">The minimum score a judgment must have to be considered relevant.</param>
        /// <param name="cutoff">The cut-off k for recall@k.</param>
        public Recall(double minScore = 1.0, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the R score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The R score.</returns>
        public double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return this.Evaluate(groundTruth, systemRun, this.MinScore, systemRun.Count);
        }
        /// <summary>
        /// Computes the R@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The R@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return this.Evaluate(groundTruth, systemRun, this.MinScore, cut);
        }

        /// <summary>
        /// Computes the R@k score of the specified run according to the specified ground truth, with the specified minimum relevance score and for the specified cut-off.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <param name="minScore">The minimum relevance score.</param>
        /// <param name="cutOff">The cut-off.</param>
        /// <returns>The R@k score.</returns>
        protected double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun, double minScore, int cutOff)
        {
            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(r => r.Score >= minScore).Select(r => r.Document.Id));
            HashSet<string> retrieved = new HashSet<string>(systemRun.Take(cutOff).Select(r => r.Document.Id));
            if (relevant.Count() == 0) return 0;
            else return ((double)relevant.Intersect(retrieved).Count()) / relevant.Count();
        }
    }
}
