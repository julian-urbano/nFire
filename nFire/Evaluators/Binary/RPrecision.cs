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
    /// An evaluator for R-Precision (RP).
    /// It is assumed that the results in the run are ordered by rank.
    /// </summary>
    public class RPrecision :
        IEvaluator<double, IListResult>
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator: "RP".
        /// </summary>
        public string ShortName
        {
            get { return "RP"; }
        }
        /// <summary>
        /// Gets the full name of the evaluator: "R-Precision".
        /// </summary>
        public string FullName
        {
            get { return "R-Precision"; }
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
        /// Creates an evaluator for RP.
        /// </summary>
        /// <param name="minScore">The minimum relevance score a document must have to be considered relevant.</param>
        public RPrecision(double minScore = 1)
        {
            this.MinScore = minScore;
        }

        /// <summary>
        /// Computes the RP score of the specified run according to the specified ground truth.
        /// It is assumed that the results in the run are ordered by rank.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The RP score.</returns>
        public double Evaluate(IRun<IListResult> groundTruth, IRun<IListResult> systemRun)
        {
            HashSet<string> relevant = new HashSet<string>(groundTruth.Where(r => r.Score >= this.MinScore).Select(r => r.Document.Id));
            HashSet<string> retrieved = new HashSet<string>(systemRun.Take(relevant.Count).Select(r => r.Document.Id));
            if (relevant.Count == 0) return 0;
            else return ((double)relevant.Intersect(retrieved).Count()) / relevant.Count;
        }
    }
}
