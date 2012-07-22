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

namespace nFire.Evaluators.Binary
{
    /// <summary>
    /// An evaluator for Interpolated Precision at Recall levels (iPR@r).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    /// <remarks>Based on the implementation by Chris Buckley in trec_eval 9.1.</remarks>
    public class InterpolatedPrecissionAtRecall :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "iPR@r".
        /// </summary>
        public string ShortName
        {
            get
            {
                return "iPR@"+this.RecallPoint;
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Interpolated Precission at r Recall".
        /// </summary>
        public string FullName
        {
            get
            {
                return "Interpolated Precission at " + this.RecallPoint + " Recall";
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
        /// Gets and sets the recall point to calculate interpolated precision.
        /// </summary>
        public double RecallPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an evaluator for iPR@r.
        /// </summary>
        /// <param name="minScore">The minimum relevance score a document must have to be considered relevant.</param>
        /// <param name="recallPoint">The recall point to calculate interpolated precision.</param>
        public InterpolatedPrecissionAtRecall(double minScore = 1.0, double recallPoint = 1.0)
        {
            this.MinScore = minScore;
            this.RecallPoint = recallPoint;
        }

        /// <summary>
        /// Computes the iPR@k score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The iPR@r score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            double[] RecallPoints = new double[] { this.RecallPoint };
            double[] values = new double[RecallPoints.Length];

            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(r => r.Score >= this.MinScore).Select(r => r.Document.Id));
            int relevantRetrieved = systemRun.Count(r => relevant.Contains(r.Document.Id));

            // Translate recall points as a number of documents
            int[] cutoffs = new int[RecallPoints.Length];
            for (int i = 0; i < RecallPoints.Length; i++)
                cutoffs[i] = (int)(RecallPoints[i] * relevant.Count + 0.9);

            int current_cut = RecallPoints.Length - 1;
            while (current_cut >= 0 && cutoffs[current_cut] > relevantRetrieved)
                current_cut--;

            // Loop over all retrieved docs in reverse order.  Needs to be reverse order since are calcualting interpolated precision.
            // Int_Prec (X) defined to be MAX (Prec (Y)) for all Y >= X.
            double precis = ((double)relevantRetrieved) / systemRun.Count;
            double int_precis = precis;
            int rel_so_far = relevantRetrieved;
            for (int i = systemRun.Count; i > 0 && rel_so_far > 0; i--) {
                precis = ((double)rel_so_far) / i;
                if (int_precis < precis)
                    int_precis = precis;
                if (relevant.Contains(systemRun.ElementAt(i - 1).Document.Id)) {
                    while (current_cut >= 0 && rel_so_far == cutoffs[current_cut]) {
                        values[current_cut] = int_precis;
                        current_cut--;
                    }
                    rel_so_far--;
                }
            }

            while (current_cut >= 0) {
                values[current_cut] = int_precis;
                current_cut--;
            }

            return values[0];
        }
    }
}
