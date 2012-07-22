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
    /// An evaluator for Rank-Biased Precision (RBP).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class RankBiasedPrecision : //TODO: RBP@k?
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "RBP".
        /// </summary>
        public virtual string ShortName
        {
            get
            {
                return "RBP";
            }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Rank-Biased Precision".
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return "Rank-Biased Precision";
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
        /// Gets and sets the user persistence parameter.
        /// </summary>
        public double Persistence
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
        /// Creates an evaluator for RBP with the specified gain function and maximum relevance score.
        /// </summary>
        /// <param name="gainFunction">The relevance-gain mapping function.</param>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="persistence">The user persistence parameter.</param>
        public RankBiasedPrecision(IGainFunction gainFunction, double maxScore, double persistence = 0.95)
        {
            this.GainFunction = gainFunction;
            this.MaxScore = maxScore;
            this.Persistence = persistence;
        }
        /// <summary>
        /// Creates an evaluator for RBP with the specified maximum relevance score and a Linear gain function.
        /// </summary>
        /// <param name="maxScore">The maximum relevance score a document may have.</param>
        /// <param name="persistence">The user persistence parameter.</param>
        public RankBiasedPrecision(double maxScore, double persistence = 0.95)
            :this(new LinearGain(), maxScore, persistence)
        {
        }

        /// <summary>
        /// Computes the RBP score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The RBP score.</returns>
        public virtual double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            Dictionary<string, double> gains = new Dictionary<string, double>();
            foreach (var doc in groundTruth)
                gains[doc.Document.Id] = this.GainFunction.Gain(doc.Score);

            double rbp = 0;
            for (int i = 0; i < systemRun.Count; i++) {
                double g;
                if (gains.TryGetValue(systemRun.ElementAt(i).Document.Id, out g)) {
                    rbp += g * Math.Pow(this.Persistence, i);
                } else {
                    // Unjudged document
                }
            }

            return (1 - this.Persistence) * rbp / this.GainFunction.Gain(this.MaxScore);
        }
    }
}
