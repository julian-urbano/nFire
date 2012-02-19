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
using System.Linq;
using System.Collections.Generic;

using nFire.Core;

namespace nFire.Evaluators.Binary
{
    /// <summary>
    /// An evaluator for F-measure and F-measure after k documents retrieved (F-measure@k).
    /// For F-measure@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
	public class FMeasure :
        IEvaluator<double, ISetResult>,
        IEvaluator<double,IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "F" or "F@k".
        /// </summary>
        public string ShortName
        {
            get {
                if (this.Cutoff == null) return "F";
                else return "F@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "F-measure" or "F-measure at k".
        /// </summary>
        public string FullName
        {
            get {
                if (this.Cutoff == null) return "F-measure";
                else return "F-measure at " + this.Cutoff;
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
        /// Gets and sets the cut-off k for precision@k. If null, F-measure is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the beta coefficient: the importance of recall with respect to precision.
        /// </summary>
        public double Beta
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the auxiliary pecision evaluator to compute the f score.
        /// </summary>
        protected Precision P
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the auxiliary recall evaluator to compute the f score.
        /// </summary>
        protected Recall R
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for F-measure and F-measure@k.
        /// </summary>
        /// <param name="minScore">The minimum score a judgment must have to be considered relevant.</param>
        /// <param name="cutoff">The cut-off k for F-measure@k.</param>
        /// <param name="beta">The beta coefficient.</param>
        public FMeasure(double minScore = 1.0, int? cutoff =null, double beta=1.0)
        {
            this.MinScore = minScore;
            this.Cutoff = cutoff;
            this.Beta = beta;
            this.P = new Precision();
            this.R = new Recall();
        }

        /// <summary>
        /// Computes the F score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The F score.</returns>
        public double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            this.P.MinScore = this.MinScore;
            this.P.Cutoff = null;
            this.R.MinScore = this.MinScore;
            this.R.Cutoff = null;

            double p = this.P.Evaluate(groundTruth, systemRun);
            double r = this.R.Evaluate(groundTruth, systemRun);
            if (this.Beta * this.Beta * p + r == 0) return 0;
            else return (1.0 + this.Beta * this.Beta) * p * r / (this.Beta * this.Beta * p + r);
        }
        /// <summary>
        /// Computes the F@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The F@k score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            this.P.MinScore = this.MinScore;
            this.P.Cutoff = this.Cutoff;
            this.R.MinScore = this.MinScore;
            this.R.Cutoff = this.Cutoff;

            double p = this.P.Evaluate(groundTruth, systemRun);
            double r = this.R.Evaluate(groundTruth, systemRun);
            if (this.Beta * this.Beta * p + r == 0) return 0;
            else return (1.0 + this.Beta * this.Beta) * p * r / (this.Beta * this.Beta * p + r);
        }
    }
}
