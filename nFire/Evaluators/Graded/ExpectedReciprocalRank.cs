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

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// An evaluator for Expected Reciprocal Rank (ERR) and Expected Reciprocal Rank after k documents retrieved (ERR@k).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class ExpectedReciprocalRank :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "ERR" or "ERR@k".
        /// </summary>
        public virtual string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "ERR";
                else return "ERR@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Expected Reciprocal Rank" or "Expected Reciprocal Rank at k".
        /// </summary>
        public virtual string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Expected Reciprocal Rank";
                else return "Expected Reciprocal Rank at " + this.Cutoff;
            }
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
        /// Gets and sets the maximum relevance score a document may have.
        /// </summary>
        public double MaxScore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the gain function to map relevance scores onto information gain values.
        /// </summary>
        public IGainFunction GainFunction
        {
            get;
            set;
        }
        
        /// <summary>
        /// Creates an evaluator for ERR and ERR@k with the specified gain function and maximum relevance score.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for ERR@k.</param>
        public ExpectedReciprocalRank(IGainFunction gainFunction, double maxScore, int? cutoff = null)
        {
            this.GainFunction = gainFunction;
            this.MaxScore = maxScore;
            this.Cutoff = cutoff;
        }
        /// <summary>
        /// Creates an evaluator for ERR and ERR@k with the specified maximum relevance score and an Exponential gain function with base 2.0.
        /// </summary>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for ERR@k.</param>
        public ExpectedReciprocalRank(double maxScore, int? cutoff = null)
            : this(new ExponentialGain(2.0), maxScore, cutoff)
        {
        }

        /// <summary>
        /// Computes the ERR@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The ERR@k score.</returns>
        public virtual double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;

            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth) {
                double gain = this.GainFunction.Gain(doc.Score);
                if (gain > 0)
                    gains[doc.Document.Id] = gain;
            }

            double err = 0;
            double product = 1.0;
            double maxGain = this.GainFunction.Gain(this.MaxScore);
            for (int i = 0; i<systemRun.Count && i < cut; i++) {
                double gain;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out gain)) { // Only documents with gain>0, because gain(0)=0. Otherwise, R_i could be negative 
                    double R_i = (gain - 1.0) / maxGain;
                    err += 1.0 / (i+1) * R_i *product;
                    product *= 1.0 - R_i;
                } else {
                    // Unjudged document
                }
            }

            return err;
        }
    }

    /// <summary>
    /// An evaluator for Normalized Expected Reciprocal Rank (nERR) and Normalized Expected Reciprocal Rank after k documents retrieved (nERR@k).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class NormalizedExpectedReciprocalRank : ExpectedReciprocalRank
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "nERR" or "nERR@k".
        /// </summary>
        public override string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "nERR";
                else return "nERR@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Normalized Expected Reciprocal Rank" or "Normalized Expected Reciprocal Rank at k".
        /// </summary>
        public override string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Normalized Expected Reciprocal Rank";
                else return "Normalized Expected Reciprocal Rank at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Creates an evaluator for nERR and nERR@k with the specified gain function and maximum relevance score.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for nERR@k.</param>
        public NormalizedExpectedReciprocalRank(IGainFunction gainFunction, double maxScore, int? cutoff = null)
            :base(gainFunction, maxScore,cutoff)
        {
        }
        /// <summary>
        /// Creates an evaluator for nERR and nERR@k with the specified maximum relevance score and an Exponential gain function with base 2.0.
        /// </summary>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for nERR@k.</param>
        public NormalizedExpectedReciprocalRank(double maxScore, int? cutoff = null)
            : base(maxScore, cutoff)
        {
        }

        /// <summary>
        /// Computes the nERR@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The nERR@k score.</returns>
        public override double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            nFire.Base.Run<IListResult> ideal = new Base.Run<IListResult>(groundTruth.Query, groundTruth.System);
            foreach (IListResult res in groundTruth.OrderByDescending(r =>base.GainFunction.Gain(r.Score)))
                ideal.Add(res);

            return base.Evaluate(groundTruth, systemRun) / base.Evaluate(groundTruth, ideal);
        }
    }
}
