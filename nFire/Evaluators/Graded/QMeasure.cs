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

using nFire.Core;

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// An evaluator for Q-measure (Q) and Q-measure after k documents retrieved (Q@k).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class QMeasure :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "Q" or "Q@k".
        /// </summary>
        public virtual string ShortName
        {
            get
            {
                if (this.Cutoff == null) return "Q";
                else return "Q@" + this.Cutoff;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Q-measure" or "Q-measure at k".
        /// </summary>
        public virtual string FullName
        {
            get
            {
                if (this.Cutoff == null) return "Q-measure";
                else return "Q-measure at " + this.Cutoff;
            }
        }

        /// <summary>
        /// Gets and sets the cut-off k for Q@k. If null, Q is computed, with not cut-off.
        /// </summary>
        public int? Cutoff
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the beta parameter for the computation of blended ratios.
        /// </summary>
        public double Beta
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
        /// Creates an evaluator for Q and Q@k with the specified gain function and beta parameter.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="beta">The beta parameter for blended ratios.</param>
        /// <param name="cutoff">The cut-off k for Q@k.</param>
        public QMeasure(IGainFunction gainFunction, double beta = 1.0, int? cutoff = null)
        {
            this.GainFunction = gainFunction;
            this.Beta = beta;
            this.Cutoff = cutoff;
        }      
        /// <summary>
        /// Creates an evaluator for Q and Q@k with the specified beta parameter and a Linear gain function.
        /// </summary>
        /// <param name="beta">The beta parameter for blended ratios.</param>
        /// <param name="cutoff">The cut-off k for Q@k.</param>
        public QMeasure(double beta = 1.0, int? cutoff = null)
            :this(new LinearGain(), beta, cutoff)
        {
        }

        /// <summary>
        /// Computes the Q@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The Q@k score.</returns>
        public virtual double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            int cut = this.Cutoff == null ? systemRun.Count : (int)this.Cutoff;

            Dictionary<string,double> relevants = new Dictionary<string,double>();
             List<double> ideal = new List<double>();
            foreach (IListResult res in groundTruth.OrderByDescending(r => this.GainFunction.Gain(r.Score))){
                double gain = this.GainFunction.Gain(res.Score);
                ideal.Add(gain);
                if(gain>0)
                    relevants[res.Document.Id]=gain;
            }

            if (relevants.Count > 0) {
                double q = 0;
                double c = 0;
                double cg = 0, cgi = 0;
                for (int i = 0; i < cut && i < systemRun.Count; i++) {
                    if(i<relevants.Count)
                        cgi += ideal[i];
                    double g = 0;
                    if(relevants.TryGetValue(systemRun.ElementAt(i).Document.Id, out g)){
                        cg += g;
                        c++;
                        q += (c + this.Beta * cg) / (i + 1 + this.Beta * cgi);
                    }
                }

                return q / Math.Min(cut, relevants.Count); // This is the NTCIREVAL definition. On the contrary, ouir AP@k is divided by R, even if k<R.
            } else {
                // No relevants in ground truth
                return 0;
            }
        }
    }
}
