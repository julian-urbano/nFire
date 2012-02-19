using System;
using System.Collections.Generic;
using System.Linq;

using nFire.Core;

namespace nFire.Evaluators
{
    /// <summary>
    /// Evaluates the Average Dynamic Recall (ADR) of a retrieval list given another one as ground truth, with or without cutoffs.
    /// TODO Reciprocal Rank = 1 / rank(first relevant document)
    /// </summary>
    /// <author>Julián Urbano</author>
    /// <email>jurbano@inf.uc3m.es</email>
    /// <date>29 Nov 2009</date>
    /// <version>1.0</version>
    public class AverageDynamicRecall : IMultiPointEvaluator, ISinglePointEvaluator
    {
        
        /// <summary>
        /// Gets the minimum score that a judgment must have for its document to be relevant.
        /// </summary>
        public double MinScore
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a new Average Dynamic Recall evaluator.
        /// </summary>
        /// <param name="minScore">Minimum relevance score for a document to be considered relevant. Defaults to 1.</param>
        public AverageDynamicRecall(double minScore = 1)
        {
            this.MinScore = minScore;
        }
		public double Evaluate(IJudgmentList groundTruth, IJudgmentList run)
		{
			return this.Evaluate(groundTruth, run, groundTruth.Where(j => j.Score>=this.MinScore).Count());
		}
		public double[] Evaluate(IJudgmentList groundTruth, IJudgmentList run, int[] cutOffs)
		{
			double[] res = new double[cutOffs.Length];

			for (int i = 0; i < cutOffs.Length; i++) {
				res[i] = this.Evaluate(groundTruth, run, cutOffs[i]);
			}

			return res;
		}
		public double Evaluate(IJudgmentList groundTruth, IJudgmentList run, int cutOff)
        {
            IJudgment[] sortedRelevant = groundTruth.Where(j => j.Score >= this.MinScore).OrderBy(j => j.Rank).ToArray();
			int size = Math.Min(sortedRelevant.Length,cutOff);
            string[] retrieved = run.Take(size).Select(j => j.Document.Id).ToArray();

            if (sortedRelevant.Count() > 0 && retrieved.Count() > 0) {
                double adr = 0;
				for (int i = 0; i < size; i++) {
					var relevant = sortedRelevant.Where(j => j.Score == sortedRelevant[i].Score || j.Rank < sortedRelevant[i].Rank).Select(j => j.Document.Id);
                    //var relevant = sortedRelevant.Where(j => j.Score >= sortedRelevant[i].Score).Select(j => j.Document.Id);
					double sub = (double)(retrieved.Take(i + 1).Intersect(relevant).Count()) / (i + 1); //relevant.Take(i + 1).Distinct().Count();
                    adr += sub;
                }
               return  adr / cutOff; // TODO en el paper no evalúan la lista entera si el ground truth es más pequeño -> se salta los falsos positivos del final!ºº
            } else {
                return 0;
            }
        }

        /// <summary>
        /// Gets a well-formatted name of this evaluator.
        /// </summary>
        public string Name
        {
            get
            {
                    return "ADR";
            }
        }
	}
}
