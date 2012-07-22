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
    /// An evaluator for Precision (P) and Precision after k documents retrieved (P@k).
    /// For P@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class Precision :
        IEvaluator<double, ISetResult>,
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "P" or "P@k".
        /// </summary>
        public string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "P";
                else return "P@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Precision" or "Precision at k".
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Precision";
                else return "Precision at " + this.Cutoff;
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
        /// Gets and sets the cut-off k for P@k. If null, P is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for P and P@k.
        /// </summary>
        /// <param name="minScore">The minimum relevance score a document must have to be considered relevant.</param>
        /// <param name="cutoff">The cut-off k for P@k.</param>
        public Precision(double minScore = 1.0, int? cutoff = null)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
        }

        /// <summary>
        /// Computes the P score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The P score.</returns>
        public double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return this.Evaluate(groundTruth, systemRun, this.MinScore, systemRun.Count);
        }
        /// <summary>
        /// Computes the P@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The P@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return this.Evaluate(groundTruth, systemRun, this.MinScore, cut);
        }

        /// <summary>
        /// Computes the P@k score of the specified run according to the specified ground truth, with the specified minimum relevance score and for the specified cut-off.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <param name="minScore">The minimum relevance score.</param>
        /// <param name="cutOff">The cut-off.</param>
        /// <returns>The P@k score.</returns>
        protected double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun, double minScore, int cutOff)
        {
            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(r => r.Score >= minScore).Select(r => r.Document.Id));
            HashSet<string> retrieved = new HashSet<string>(systemRun.Take(cutOff).Select(r => r.Document.Id));
            if (cutOff == 0) return 0;
            else return ((double)relevant.Intersect(retrieved).Count()) / cutOff;
        }
    }
}
