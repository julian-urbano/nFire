﻿// Copyright (C) 2012  Julián Urbano <urbano.julian@gmail.com>
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

using nFire.Trec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using nFire.Core;
using nFire.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nFire.Test
{
    [TestClass()]
    public class GradedEvaluators
    {
        private Task<IListResult> Task;

        [TestInitialize()]
        public void Initialize()
        {
            this.Task = new Task<IListResult>();
            QrelFormatter qrelForm = new QrelFormatter();
            RunFormatter runForm = new RunFormatter();
            MemoryStream stream = new MemoryStream(Resources.qrels);
            var qrels = qrelForm.Read(stream, this.Task);
            stream = new MemoryStream(Resources.results);
            var runs = runForm.Read(stream, this.Task);
            Task.AddGroundTruths(qrels);
            Task.AddSystemRuns(runs);
        }

        [TestMethod()]
        public void CumulatedGainTest()
        {
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[] res301 = new double[] { 0.0000, 2.0000, 2.0000, 5.0000, 7.00000, 23.0000, 42.0000, 74.0000, 74.0000, 74.0000 };
            double[] res302 = new double[] { 12.0000, 21.0000, 36.0000, 48.0000, 66.0000, 126.0000, 132.0000, 150.0000, 150.0000, 150.0000 };
            double[] res303 = new double[] { 0.0000, 0.0000, 0.0000, 2.0000, 2.0000, 14.0000, 16.0000, 16.0000, 16.0000, 16.0000 };

            nFire.Evaluators.Graded.CumulatedGain cg = new nFire.Evaluators.Graded.CumulatedGain();
            for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                cg.Cutoff = cutoffs[cutoff];

                var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], cg);
                Assert.AreEqual(res301[cutoff], res[this.Task.Queries["301"]], 0.00005);
                Assert.AreEqual(res302[cutoff], res[this.Task.Queries["302"]], 0.00005);
                Assert.AreEqual(res303[cutoff], res[this.Task.Queries["303"]], 0.00005);
            }
        }
        [TestMethod()]
        public void DiscountedCumulatedGainTest()
        {
            int[] logBases = new int[] { 2, 10 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.7431, 0.7431, 1.4643, 1.8808, 4.6196, 7.2717, 11.1678, 11.1678, 11.1678},
                new double[]{0.0000, 2.0000, 2.0000, 4.3957, 5.7795, 14.8775, 23.6877, 36.6303, 36.6303, 36.6303}
            };
            double[][] res302 = new double[][] {
                new double[]{8.7920, 11.8990, 15.9695, 18.8792, 22.7556, 33.4611, 34.3113, 36.4429, 36.4429, 36.4429},
                new double[]{12.0000, 21.0000, 34.5221, 44.1876, 57.0648, 92.6277, 95.4522, 102.5330, 102.5330, 102.5330}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.4708, 0.4708, 2.6248, 2.9214, 2.9214, 2.9214, 2.9214},
                new double[]{0.0000, 0.0000, 0.0000, 1.5640, 1.5640, 8.7192, 9.7048, 9.7048, 9.7048, 9.7048}
            };

            nFire.Evaluators.Graded.DiscountedCumulatedGain dcg = new nFire.Evaluators.Graded.DiscountedCumulatedGain();
            for (int logBase = 0; logBase < logBases.Length; logBase++) {
                dcg.LogBase = logBases[logBase];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    dcg.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], dcg);
                    Assert.AreEqual(res301[logBase][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[logBase][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[logBase][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void NormalizedDiscountedCumulatedGainTest()
        {
            int[] logBases = new int[] { 2, 10 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.0404, 0.0365, 0.0680, 0.0794, 0.1301, 0.1472, 0.1358, 0.1358, 0.1358},
                new double[]{0.0000, 0.0625, 0.0521, 0.1037, 0.1167, 0.1675, 0.1755, 0.1501, 0.1501, 0.1501}
            };
            double[][] res302 = new double[][] {
                new double[]{0.8229, 0.7548, 0.8052, 0.8055, 0.7616, 0.6117, 0.6273, 0.6662, 0.6662, 0.6662},
                new double[]{0.8000, 0.7000, 0.7932, 0.7963, 0.7421, 0.5813, 0.5990, 0.6435, 0.6435, 0.6435}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.0508, 0.0508, 0.2830, 0.3149, 0.3149, 0.3149, 0.3149},
                new double[]{0.0000, 0.0000, 0.0000, 0.0978, 0.0978, 0.5450, 0.6065, 0.6065, 0.6065, 0.6065}
            };

            nFire.Evaluators.Graded.NormalizedDiscountedCumulatedGain ndcg = new nFire.Evaluators.Graded.NormalizedDiscountedCumulatedGain();
            for (int logBase = 0; logBase < logBases.Length; logBase++) {
                ndcg.LogBase = logBases[logBase];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    ndcg.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], ndcg);
                    Assert.AreEqual(res301[logBase][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[logBase][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[logBase][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
    }
}