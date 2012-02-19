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
    public class BinaryEvaluators
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
        public void PrecisionTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.2000, 0.1333, 0.2500, 0.2333, 0.2300, 0.2100, 0.1420, 0.0710, 0.1420},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0020, 0.0010, 0.0020},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0020, 0.0010, 0.0020},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0020, 0.0010, 0.0020}
            };
            double[][] res302 = new double[][] {
                new double[]{0.8000, 0.7000, 0.8000, 0.8000, 0.7333, 0.4200, 0.2200, 0.1000, 0.0500, 0.1000},
                new double[]{0.8000, 0.7000, 0.8000, 0.8000, 0.7333, 0.4200, 0.2200, 0.1000, 0.0500, 0.1000},
                new double[]{0.8000, 0.7000, 0.8000, 0.8000, 0.7333, 0.4200, 0.2200, 0.1000, 0.0500, 0.1000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.0500, 0.0333, 0.0700, 0.0400, 0.0160, 0.0080, 0.0160},
                new double[]{0.0000, 0.0000, 0.0000, 0.0500, 0.0333, 0.0700, 0.0400, 0.0160, 0.0080, 0.0160},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };

            nFire.Evaluators.Binary.Precision prec = new nFire.Evaluators.Binary.Precision();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                prec.MinScore = minRels[minRel];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    prec.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], prec);
                    Assert.AreEqual(res301[minRel][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[minRel][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[minRel][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void RecallTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.0042, 0.0042, 0.0105, 0.0148, 0.0485, 0.0886, 0.1498, 0.1498, 0.1498},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0833, 0.0833, 0.0833},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.1667, 0.1667, 0.1667},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.1667, 0.1667, 0.1667}
            };
            double[][] res302 = new double[][] {
                new double[]{0.0519, 0.0909, 0.1558, 0.2078, 0.2857, 0.5455, 0.5714, 0.6494, 0.6494, 0.6494},
                new double[]{0.0519, 0.0909, 0.1558, 0.2078, 0.2857, 0.5455, 0.5714, 0.6494, 0.6494, 0.6494},
                new double[]{0.0519, 0.0909, 0.1558, 0.2078, 0.2857, 0.5455, 0.5714, 0.6494, 0.6494, 0.6494},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.1250, 0.1250, 0.8750, 1.0000, 1.0000, 1.0000, 1.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.1250, 0.1250, 0.8750, 1.0000, 1.0000, 1.0000, 1.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };

            nFire.Evaluators.Binary.Recall rec = new nFire.Evaluators.Binary.Recall();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                rec.MinScore = minRels[minRel];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    rec.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], rec);
                    Assert.AreEqual(res301[minRel][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[minRel][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[minRel][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void FMeasureTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.0083, 0.0082, 0.0202, 0.0278, 0.0801, 0.1246, 0.1458, 0.0963, 0.1458},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0039, 0.0020, 0.0039},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0040, 0.0020, 0.0040},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0040, 0.0020, 0.0040}
            };
            double[][] res302 = new double[][] {
                new double[]{0.0976, 0.1609, 0.2609, 0.3299, 0.4112, 0.4746, 0.3177, 0.1733, 0.0929, 0.1733},
                new double[]{0.0976, 0.1609, 0.2609, 0.3299, 0.4112, 0.4746, 0.3177, 0.1733, 0.0929, 0.1733},
                new double[]{0.0976, 0.1609, 0.2609, 0.3299, 0.4112, 0.4746, 0.3177, 0.1733, 0.0929, 0.1733},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.0714, 0.0526, 0.1296, 0.0769, 0.0315, 0.0159, 0.0315},
                new double[]{0.0000, 0.0000, 0.0000, 0.0714, 0.0526, 0.1296, 0.0769, 0.0315, 0.0159, 0.0315},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            nFire.Evaluators.Binary.FMeasure f = new nFire.Evaluators.Binary.FMeasure();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                f.MinScore = minRels[minRel];
                sb.AppendLine();
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    f.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], f);
                    Assert.AreEqual(res301[minRel][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[minRel][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[minRel][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void ReciprocalRankTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            double[] res301 = new double[] { 0.1667, 0.0033, 0.0033, 0.0033 };
            double[] res302 = new double[] { 1.0000, 1.0000, 1.0000, 0.0000 };
            double[] res303 = new double[] { 0.0526, 0.0526, 0.0000, 0.0000 };

            nFire.Evaluators.Binary.ReciprocalRank rr = new nFire.Evaluators.Binary.ReciprocalRank();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                rr.MinScore = minRels[minRel];

                var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], rr);
                Assert.AreEqual(res301[minRel], res[this.Task.Queries["301"]], 0.00005);
                Assert.AreEqual(res302[minRel], res[this.Task.Queries["302"]], 0.00005);
                Assert.AreEqual(res303[minRel], res[this.Task.Queries["303"]], 0.00005);
            }
        }
        [TestMethod()]
        public void RPrecisionTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            double[] res301 = new double[] { 0.1456, 0.0000, 0.0000, 0.0000 };
            double[] res302 = new double[] { 0.5065, 0.5065, 0.5065, 0.0000 };
            double[] res303 = new double[] { 0.0000, 0.0000, 0.0000, 0.0000 };

            nFire.Evaluators.Binary.RPrecision rprec = new nFire.Evaluators.Binary.RPrecision();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                rprec.MinScore = minRels[minRel];

                var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], rprec);
                Assert.AreEqual(res301[minRel], res[this.Task.Queries["301"]], 0.00005);
                Assert.AreEqual(res302[minRel], res[this.Task.Queries["302"]], 0.00005);
                Assert.AreEqual(res303[minRel], res[this.Task.Queries["303"]], 0.00005);
            }
        }
        [TestMethod()]
        public void InterpolatedPrecisionAtRecallTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            double[] recallPoints = new double[] { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
            double[][] res301 = new double[][] {
                new double[]{0.2857,0.2096,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000},
                new double[]{0.0033,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000},
                new double[]{0.0033,0.0033,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000},
                new double[]{0.0033,0.0033,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000}
            };
            double[][] res302 = new double[][] {
                new double[]{1.0000,0.8421,0.8421,0.7419,0.6863,0.5417,0.1420,0.0000,0.0000,0.0000,0.0000},
                new double[]{1.0000,0.8421,0.8421,0.7419,0.6863,0.5417,0.1420,0.0000,0.0000,0.0000,0.0000},
                new double[]{1.0000,0.8421,0.8421,0.7419,0.6863,0.5417,0.1420,0.0000,0.0000,0.0000,0.0000},
                new double[]{0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000}
            };
            double[][] res303 = new double[][] {
                new double[]{0.1136,0.1136,0.1136,0.1136,0.1136,0.1136,0.1136,0.1045,0.1045,0.0748,0.0748},
                new double[]{0.1136,0.1136,0.1136,0.1136,0.1136,0.1136,0.1136,0.1045,0.1045,0.0748,0.0748},
                new double[]{0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000},
                new double[]{0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000}
            };

            nFire.Evaluators.Binary.InterpolatedPrecissionAtRecall ipr = new nFire.Evaluators.Binary.InterpolatedPrecissionAtRecall();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                ipr.MinScore = minRels[minRel];
                for (int recallPoint = 0; recallPoint < recallPoints.Length; recallPoint++) {
                    ipr.RecallPoint = recallPoints[recallPoint];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], ipr);
                    Assert.AreEqual(res301[minRel][recallPoint], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[minRel][recallPoint], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[minRel][recallPoint], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void AveragePrecisionTest()
        {
            int[] minRels = new int[] { 1, 2, 3, 4 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][] res301 = new double[][] {
                new double[]{0.0000, 0.0010, 0.0010, 0.0023, 0.0033, 0.0118, 0.0208, 0.0324, 0.0324, 0.0324},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0003, 0.0003, 0.0003},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0005, 0.0005, 0.0005},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0005, 0.0005, 0.0005}
            };
            double[][] res302 = new double[][] {
                new double[]{0.0461, 0.0768, 0.1265, 0.1695, 0.2298, 0.3983, 0.4068, 0.4175, 0.4175, 0.4175},
                new double[]{0.0461, 0.0768, 0.1265, 0.1695, 0.2298, 0.3983, 0.4068, 0.4175, 0.4175, 0.4175},
                new double[]{0.0461, 0.0768, 0.1265, 0.1695, 0.2298, 0.3983, 0.4068, 0.4175, 0.4175, 0.4175},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };
            double[][] res303 = new double[][] {
                new double[]{0.0000, 0.0000, 0.0000, 0.0066, 0.0066, 0.0729, 0.0823, 0.0823, 0.0823, 0.0823},
                new double[]{0.0000, 0.0000, 0.0000, 0.0066, 0.0066, 0.0729, 0.0823, 0.0823, 0.0823, 0.0823},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000},
                new double[]{0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000}
            };

            nFire.Evaluators.Binary.AveragePrecision ap = new nFire.Evaluators.Binary.AveragePrecision();
            for (int minRel = 0; minRel < minRels.Length; minRel++) {
                ap.MinScore = minRels[minRel];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    ap.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], ap);
                    Assert.AreEqual(res301[minRel][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[minRel][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[minRel][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
    }
}
