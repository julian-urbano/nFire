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
    /// An evaluator for Discounted Cumulated Gain (DCG) and Discounted Cumulated Gain after k documents retrieved (DCG@k).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class DiscountedCumulatedGain :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "DCG" or "DCG@k".
        /// </summary>
        public virtual string ShortName
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
        public virtual string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Discounted Cumulated Gain";
                else return "Discounted Cumulated Gain at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the cut-off k for DCG@k. If null, DCG is computed, with not cut-off.
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
        /// Gets and sets the function to compute gain discount factors.
        /// </summary>
        public IDiscountFunction DiscountFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for DCG and DCG@k with the specified gain and discount functions.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="discountFunction">The function to discount gains.</param>
        /// <param name="cutoff">The cut-off k for DCG@k.</param>
        public DiscountedCumulatedGain(IGainFunction gainFunction, IDiscountFunction discountFunction, int? cutoff = null)
        {
            this.GainFunction = gainFunction;
            this.DiscountFunction = discountFunction;
            this.Cutoff = cutoff;
        }
        /// <summary>
        /// Creates an evaluator for DCG and DCG@k with a Linear gain function and a Logarithmic (Microsoft) discount function with base 2.
        /// </summary>
        /// <param name="cutoff">The cut-off k for DCG@k.</param>
        public DiscountedCumulatedGain(int? cutoff = null)
            : this(new LinearGain(), new LogarithmicDiscount(2.0), cutoff)
        {
        }

        /// <summary>
        /// Computes the DCG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The DCG@k score.</returns>
        public virtual double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;

            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth)
                gains.Add(doc.Document.Id, this.GainFunction.Gain(doc.Score));

            double dcg = 0;
            for (int i = 0; i < systemRun.Count && i < cut; i++) {
                double g;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out g)) {
                    dcg += g / this.DiscountFunction.Discount(i + 1);
                } else {
                    // Unjudged document
                }
            }

            return dcg;
        }
    }

    /// <summary>
    /// An evaluator for Normalized Discounted Cumulated Gain (nDCG) and Normalized Discounted Cumulated Gain after k documents retrieved (nDCGk).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class NormalizedDiscountedCumulatedGain : DiscountedCumulatedGain
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "nDCG" or "nDCG@k".
        /// </summary>
        public override string ShortName
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
        public override string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Normalized Discounted Cumulated Gain";
                else return "Normalized Discounted Cumulated Gain at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Creates an evaluator for nDCG and nDCG@k with the specified gain and discount functions.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="discountFunction">The function to discount gains.</param>
        /// <param name="cutoff">The cut-off k for nDCG@k.</param>
        public NormalizedDiscountedCumulatedGain(IGainFunction gainFunction, IDiscountFunction discountFunction, int? cutoff = null)
            : base(gainFunction, discountFunction, cutoff)
        {
        }        
        /// <summary>
        /// Creates an evaluator for nDCG and nDCG@k with a Linear gain function and a Logarithmic (Microsoft) discount function with base 2.
        /// </summary>
        /// <param name="cutoff">The cut-off k for nDCG@k.</param>
        public NormalizedDiscountedCumulatedGain(int? cutoff = null)
            : base(cutoff)
        {
        }

        /// <summary>
        /// Computes the nDCG@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The nDCG@k score.</returns>
        public override double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            double dcg = base.Evaluate(groundTruth, systemRun);
            nFire.Base.Run<IListResult> ideal = new Base.Run<IListResult>(groundTruth.Query, groundTruth.System);
            foreach (IListResult res in groundTruth.OrderByDescending(r => base.GainFunction.Gain(r.Score)))
                ideal.Add(res);
            double idcg = base.Evaluate(groundTruth, ideal);

            return dcg / idcg;
        }
    }
}