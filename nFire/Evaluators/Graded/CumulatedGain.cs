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
    /// An evaluator for Cumulated Gain (CG) and Cumulated Gain after k documents retrieved (CG@k).
    /// For CG@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class CumulatedGain :
        IEvaluator<double, ISetResult>,
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "CG" or "CG@k".
        /// </summary>
        public virtual string ShortName
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
        public virtual string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Cumulated Gain";
                else return "Cumulated Gain at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the cut-off k for CG@k. If null, CG is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
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
        /// Creates an evaluator for CG and CG@k with the specified gain function.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="cutoff">The cut-off k for DCG@k.</param>
        public CumulatedGain(IGainFunction gainFunction, int? cutoff = null)
        {
            this.GainFunction = gainFunction;
            this.Cutoff = cutoff;
        }
        /// <summary>
        /// Creates an evaluator for CG and CG@k with a Linear gain function.
        /// </summary>
        /// <param name="cutoff">The cut-off k for CG@k.</param>
        public CumulatedGain(int? cutoff = null)
            :this(new LinearGain(), cutoff)
        {
        }

        /// <summary>
        /// Computes the CG score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The CG score.</returns>
        public virtual double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return this.Evaluate(groundTruth, systemRun, systemRun.Count);
        }
        /// <summary>
        /// Computes the CG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The CG@k score.</returns>
        public virtual double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return this.Evaluate(groundTruth, systemRun, cut);
        }

        /// <summary>
        /// Computes the CG@k score of the specified run according to the specified ground truth, with the specified minimum relevance score and for the specified cut-off.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <param name="cutOff">The cut-off.</param>
        /// <returns>The CG@k score.</returns>
        protected double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun, int cutOff)
        {
            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth)
                gains[doc.Document.Id]= this.GainFunction.Gain(doc.Score);

            double cg = 0;
            for (int i = 0; i < systemRun.Count && i < cutOff; i++) {
                double g;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out g)) {
                    cg += g;
                } else {
                    // Unjudged document
                }
            }

            return cg;
        }
    }

    /// <summary>
    /// An evaluator for Average Gain (AG) and Average Gain after k documents retrieved (AG@k).
    /// For AG@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class AverageGain : CumulatedGain
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "AG" or "AG@k".
        /// </summary>
        public override string ShortName
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
        public override string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Average Gain";
                else return "Average Gain at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Creates an evaluator for AG and AG@k with the specified gain function.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="cutoff">The cut-off k for AG@k.</param>
        public AverageGain(IGainFunction gainFunction, int? cutoff = null)
            : base(gainFunction, cutoff)
        {
        }
        /// <summary>
        /// Creates an evaluator for AG and AG@k with a Linear gain function.
        /// </summary>
        /// <param name="cutoff">The cut-off k for AG@k.</param>
        public AverageGain(int? cutoff = null)
            : base(cutoff)
        {
        }

        /// <summary>
        /// Computes the AG score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The AG score.</returns>
        public override double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return base.Evaluate(groundTruth, systemRun, systemRun.Count) / systemRun.Count;
        }
        /// <summary>
        /// Computes the AG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The AG@k score.</returns>
        public override double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return base.Evaluate(groundTruth, systemRun, cut) / cut;
        }
    }

    /// <summary>
    /// An evaluator for Normalized Average Gain (nAG) and Normalized Average Gain after k documents retrieved (nAG@k).
    /// For nAG@k it is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class NormalizedAverageGain : CumulatedGain
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "nAG" or "nAG@k".
        /// </summary>
        public override string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "nAG";
                else return "nAG@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Normalized Average Gain" or "Normalized Average Gain at k".
        /// </summary>
        public override string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Normalized Average Gain";
                else return "Normalized Average Gain at " + this.Cutoff;
            }
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
        /// Creates an evaluator for nAG and nAG@k with the specified gain function and maximum relevance score.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for nAG@k.</param>
        public NormalizedAverageGain(IGainFunction gainFunction, double maxScore, int? cutoff = null)
            : base(gainFunction, cutoff)
        {
            this.MaxScore = maxScore;
        }
        /// <summary>
        /// Creates an evaluator for nAG and nAG@k with the specifiedmaximum relevance score and a Linear gain function.
        /// </summary>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="cutoff">The cut-off k for nAG@k.</param>
        public NormalizedAverageGain(double maxScore, int? cutoff = null)
            : base(cutoff)
        {
            this.MaxScore = maxScore;
        }

        /// <summary>
        /// Computes the nAG score of the specified run according to the specified ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The nAG score.</returns>
        public override double Evaluate(IRun<ISetResult> groundTruth, IRun<ISetResult> systemRun)
        {
            return base.Evaluate(groundTruth, systemRun, systemRun.Count) / systemRun.Count / this.GainFunction.Gain(this.MaxScore);
        }
        /// <summary>
        /// Computes the nAG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The nAG@k score.</returns>
        public override double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;
            return base.Evaluate(groundTruth, systemRun, cut) / cut / this.GainFunction.Gain(this.MaxScore);
        }
    }
}
