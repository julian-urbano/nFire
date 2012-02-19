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
    /// An evaluator for reciprocal rank.
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class ReciprocalRank : IEvaluator<double,IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "RR".
        /// </summary>
        public string ShortName
        {
            get { return "RR"; }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "Reciprocal Rank".
        /// </summary>
        public string FullName
        {
            get { return "Reciprocal Rank"; }
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
        /// Creates an evaluator for reciprocal rank.
        /// </summary>
        /// <param name="minScore">The minimum score a judgment must have to be considered relevant.</param>
        public ReciprocalRank(double minScore = 1)
        {
            this.MinScore = minScore;
        }

        /// <summary>
        /// Computes the RR score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The RR score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            double rr = 0;
            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(j => j.Score >= this.MinScore).Select(j => j.Document.Id));
            for (int i = 0; i < systemRun.Count && rr == 0; i++) {
                if (relevant.Contains(systemRun.ElementAt(i).Document.Id)) {
                    rr = 1.0 / (i + 1);
                }
            }
            return rr;
        }
    }
}
