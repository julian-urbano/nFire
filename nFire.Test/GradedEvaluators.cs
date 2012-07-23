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
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(),
        new nFire.Evaluators.Graded.ExponentialGain(2.0),
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            double[][] res301 = new double[][]{
        new double[]{ 0, 2, 2, 5, 7, 23, 42, 74, 74, 74 }, 
       new double[] { 0, 4, 4, 10, 14, 46, 84, 156, 156, 156 }, 
        new double[]{0, 6, 6, 15, 21, 69, 126, 291, 291, 291}
      };
            double[][] res302 = new double[][]{
        new double[] { 12, 21, 36, 48, 66, 126, 132, 150, 150, 150 }, 
       new double[] { 32, 56, 96, 128, 176, 336, 352, 400, 400, 400 }, 
        new double[]{108, 189, 324, 432, 594, 1134, 1188, 1350, 1350, 1350}
      };
            double[][] res303 = new double[][]{
       new double[] { 0, 0, 0, 2, 2, 14, 16, 16, 16, 16 }, 
       new double[] { 0, 0, 0, 4, 4, 28, 32, 32, 32, 32 }, 
        new double[]{0, 0, 0, 9, 9, 63, 72, 72, 72, 72}
      };

            nFire.Evaluators.Graded.CumulatedGain cg = new nFire.Evaluators.Graded.CumulatedGain();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                cg.GainFunction = gainFunctions[gainFunction];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    cg.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], cg);
                    Assert.AreEqual(res301[gainFunction][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[gainFunction][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[gainFunction][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void AverageGainTest()
        {
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
                new nFire.Evaluators.Graded.LinearGain(),
                new nFire.Evaluators.Graded.ExponentialGain(2.0),
                new nFire.Evaluators.Graded.ExponentialGain(3.0)};

            nFire.Evaluators.Graded.AverageGain ag = new nFire.Evaluators.Graded.AverageGain();
            nFire.Evaluators.Graded.CumulatedGain cg = new nFire.Evaluators.Graded.CumulatedGain();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                ag.GainFunction = gainFunctions[gainFunction];
                cg.GainFunction = gainFunctions[gainFunction];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    ag.Cutoff = cutoffs[cutoff];
                    cg.Cutoff = cutoffs[cutoff];

                    int cut = cutoffs[cutoff] == null ? 500 : (int)cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], ag);
                    var res2 = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], cg);
                    Assert.AreEqual(res2[this.Task.Queries["301"]] / cut, res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res2[this.Task.Queries["302"]] / cut, res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res2[this.Task.Queries["303"]] / cut, res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void NormalizedAverageGainTest()
        {
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
                new nFire.Evaluators.Graded.LinearGain(),
                new nFire.Evaluators.Graded.ExponentialGain(2.0),
                new nFire.Evaluators.Graded.ExponentialGain(3.0)};

            nFire.Evaluators.Graded.NormalizedAverageGain nag = new nFire.Evaluators.Graded.NormalizedAverageGain(4);
            nFire.Evaluators.Graded.CumulatedGain cg = new nFire.Evaluators.Graded.CumulatedGain();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                nag.GainFunction = gainFunctions[gainFunction];
                cg.GainFunction = gainFunctions[gainFunction];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    nag.Cutoff = cutoffs[cutoff];
                    cg.Cutoff = cutoffs[cutoff];

                    int cut = cutoffs[cutoff] == null ? 500 : (int)cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], nag);
                    var res2 = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], cg);
                    Assert.AreEqual(res2[this.Task.Queries["301"]] / nag.GainFunction.Gain(4.0) / cut, res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res2[this.Task.Queries["302"]] / nag.GainFunction.Gain(4.0) / cut, res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res2[this.Task.Queries["303"]] / nag.GainFunction.Gain(4.0) / cut, res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void DiscountedCumulatedGainTest()
        {
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(), 
        new nFire.Evaluators.Graded.ExponentialGain(2.0), 
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            nFire.Evaluators.Graded.IDiscountFunction[] discountFunctions = new nFire.Evaluators.Graded.IDiscountFunction[]{
        new nFire.Evaluators.Graded.OriginalDiscount(2.0), 
        new nFire.Evaluators.Graded.OriginalDiscount(10.0), 
        new nFire.Evaluators.Graded.LogarithmicDiscount(2.0), 
        new nFire.Evaluators.Graded.LogarithmicDiscount(10.0)};
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][][] res301 = new double[][][]{
        new double[][] {
          new double[]{0, 0.7431, 0.7431, 1.4643, 1.8808, 4.6196, 7.2717, 11.1678, 11.1678, 11.1678}, 
          new double[]{0, 2, 2, 4.3957, 5.7795, 14.8775, 23.6877, 36.6303, 36.6303, 36.6303}, 
          new double[]{0, 0.6895, 0.6895, 1.3973, 1.8094, 4.5355, 7.1838, 11.0775, 11.0775, 11.0775}, 
          new double[]{0, 1.6808, 1.6808, 3.7785, 5.0547, 13.8142, 22.5148, 35.3874, 35.3874, 35.3874}
        }, 
        new double[][]{
          new double[]{ 0, 1.4861, 1.4861, 2.9285, 3.7616, 9.2391, 14.5434, 23.3039, 23.3039, 23.3039}, 
          new double[]{0, 4, 4, 8.7915, 11.5589, 29.7550, 47.3755, 76.4771, 76.4771, 76.4771}, 
          new double[]{ 0, 1.3791, 1.3791, 2.7945, 3.6189, 9.0710, 14.3677, 23.1228, 23.1228, 23.1228}, 
          new double[]{ 0, 3.3615, 3.3615, 7.5571, 10.1094, 27.6284, 45.0297, 73.9752, 73.9752, 73.9752}
        }, 
        new double[][]{
          new double[]{0, 2.2292, 2.2292, 4.3928, 5.6424, 13.8587, 21.8152, 41.8548, 41.8548, 41.8548}, 
          new double[]{0, 6, 6, 13.1872, 17.3384, 44.6324, 71.0632, 137.6335, 137.6335, 137.6335}, 
          new double[]{0, 2.0686, 2.0686, 4.1918, 5.4283, 13.6065, 21.5515, 41.5793, 41.5793, 41.5793},  
          new double[]{0, 5.0423, 5.0423, 11.3356, 15.1641, 41.4426, 67.5445, 133.7657, 133.7657, 133.7657}
        }
      };
            double[][][] res302 = new double[][][]{
        new double[][] {
          new double[]{8.7920, 11.8990, 15.9695, 18.8792, 22.7556, 33.4611, 34.3113, 36.4429, 36.4429, 36.4429}, 
          new double[]{12, 21, 34.5221, 44.1876, 57.0648, 92.6277, 95.4522, 102.5330, 102.5330, 102.5330}, 
          new double[]{7.3454, 10.2635, 14.2168, 17.0706, 20.8997, 31.5460, 32.3950, 34.5255, 34.5255, 34.5255}, 
          new double[]{11.1914, 18.5703, 29.7565, 38.1916, 49.9360, 83.9421, 86.7289, 93.7784, 93.7784, 93.7784}
        }, 
        new double[][]{
          new double[]{23.4454, 31.7306, 42.5854, 50.3444, 60.6815, 89.2295, 91.4969, 97.1810, 97.1810, 97.1810}, 
          new double[]{32, 56, 92.0590, 117.8337, 152.1728, 247.0071, 254.5393, 273.4214, 273.4214, 273.4214}, 
          new double[]{19.5877, 27.3693, 37.9116, 45.5216, 55.7325, 84.1228, 86.3867, 92.0679, 92.0679, 92.0679}, 
          new double[]{29.8437, 49.5207, 79.3506, 101.8443, 133.1626, 223.8455, 231.2769, 250.0758, 250.0758, 250.0758}
        }, 
        new double[][]{
          new double[]{79.1283, 107.0908, 143.7259, 169.9124, 204.8002, 301.1495, 308.8021, 327.9859, 327.9859, 327.9859}, 
          new double[]{108, 189, 310.6990, 397.6888, 513.5833, 833.6489, 859.0701, 922.7974, 922.7974, 922.7974}, 
          new double[]{66.1084, 92.3714, 127.9516, 153.6354, 188.0970, 283.9144, 291.5551, 310.7293, 310.7293, 310.7293}, 
          new double[]{100.7226, 167.1325, 267.8084, 343.7245, 449.4239, 755.4785, 780.5597, 844.0060, 844.0060, 844.0060}
        }
      };
            double[][][] res303 = new double[][][]{
        new double[][] {
          new double[]{0, 0, 0, 0.4708, 0.4708, 2.6248, 2.9214, 2.9214, 2.9214, 2.9214}, 
          new double[]{0, 0, 0, 1.5640, 1.5640, 8.7192, 9.7048, 9.7048, 9.7048, 9.7048}, 
          new double[]{0, 0, 0, 0.4628, 0.4628, 2.6047, 2.9008, 2.9008, 2.9008, 2.9008}, 
          new double[]{0, 0, 0, 1.3820, 1.3820, 8.2208, 9.1895, 9.1895, 9.1895, 9.1895}
        }, 
        new double[][]{
          new double[]{0, 0, 0, 0.9416, 0.9416, 5.2495, 5.8428, 5.8428, 5.8428, 5.8428}, 
          new double[]{0, 0, 0, 3.1280, 3.1280, 17.4385, 19.4095, 19.4095, 19.4095, 19.4095}, 
          new double[]{0, 0, 0, 0.9255, 0.9255, 5.2094, 5.8016, 5.8016, 5.8016, 5.8016}, 
          new double[]{0, 0, 0, 2.7640, 2.7640, 16.4415, 18.3791, 18.3791, 18.3791, 18.3791}
        }, 
        new double[][]{
          new double[]{0, 0, 0, 2.1187, 2.1187, 11.8114, 13.1464, 13.1464, 13.1464, 13.1464}, 
          new double[]{0, 0, 0, 7.0381, 7.0381, 39.2366, 43.6714, 43.6714, 43.6714, 43.6714}, 
          new double[]{0, 0, 0, 2.0824, 2.0824, 11.7212, 13.0535, 13.0535, 13.0535, 13.0535}, 
          new double[]{0, 0, 0, 6.2191, 6.2191, 36.9934, 41.3529, 41.3529, 41.3529, 41.3529}
        }
      };

            nFire.Evaluators.Graded.DiscountedCumulatedGain dcg = new nFire.Evaluators.Graded.DiscountedCumulatedGain();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                dcg.GainFunction = gainFunctions[gainFunction];
                for (int discountFunction = 0; discountFunction < discountFunctions.Length; discountFunction++) {
                    dcg.DiscountFunction = discountFunctions[discountFunction];
                    for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                        dcg.Cutoff = cutoffs[cutoff];

                        var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], dcg);
                        Assert.AreEqual(res301[gainFunction][discountFunction][cutoff], res[this.Task.Queries["301"]], 0.00005);
                        Assert.AreEqual(res302[gainFunction][discountFunction][cutoff], res[this.Task.Queries["302"]], 0.00005);
                        Assert.AreEqual(res303[gainFunction][discountFunction][cutoff], res[this.Task.Queries["303"]], 0.00005);
                    }
                }
            }
        }
        [TestMethod()]
        public void NormalizedDiscountedCumulatedGainTest()
        {
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(), 
        new nFire.Evaluators.Graded.ExponentialGain(2.0), 
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            nFire.Evaluators.Graded.IDiscountFunction[] discountFunctions = new nFire.Evaluators.Graded.IDiscountFunction[]{
        new nFire.Evaluators.Graded.OriginalDiscount(2.0), 
        new nFire.Evaluators.Graded.OriginalDiscount(10.0), 
        new nFire.Evaluators.Graded.LogarithmicDiscount(2.0), 
        new nFire.Evaluators.Graded.LogarithmicDiscount(10.0)};
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][][] res301 = new double[][][]{
        new double[][] {
          new double[]{0.0000,0.0404,0.0365,0.0680,0.0794,0.1301,0.1472,0.1358,0.1358,0.1358}, 
          new double[]{0.0000,0.0625,0.0521,0.1037,0.1167,0.1675,0.1755,0.1501,0.1501,0.1501}, 
          new double[]{0.0000,0.0439,0.0393,0.0746,0.0867,0.1390,0.1544,0.1396,0.1396,0.1396}, 
          new double[]{0.0000,0.0590,0.0498,0.1015,0.1156,0.1692,0.1769,0.1501,0.1501,0.1501}
        }, 
        new double[][]{
          new double[]{0.0000,0.0217,0.0206,0.0392,0.0476,0.0900,0.1115,0.1188,0.1188,0.1188}, 
          new double[]{0.0000,0.0357,0.0321,0.0662,0.0786,0.1319,0.1490,0.1426,0.1426,0.1426}, 
          new double[]{0.0000,0.0238,0.0224,0.0437,0.0531,0.0989,0.1203,0.1249,0.1249,0.1249}, 
          new double[]{0.0000,0.0333,0.0301,0.0638,0.0769,0.1332,0.1508,0.1434,0.1434,0.1434}
        }, 
        new double[][]{
          new double[]{0.0000,0.0067,0.0066,0.0128,0.0162,0.0360,0.0512,0.0798,0.0798,0.0798}, 
          new double[]{0.0000,0.0115,0.0110,0.0236,0.0299,0.0639,0.0849,0.1182,0.1182,0.1182}, 
          new double[]{0.0000,0.0074,0.0072,0.0145,0.0183,0.0411,0.0578,0.0882,0.0882,0.0882}, 
          new double[]{0.0000,0.0106,0.0102,0.0224,0.0289,0.0648,0.0870,0.1214,0.1214,0.1214}
        }
      };
            double[][][] res302 = new double[][][]{
        new double[][] {
          new double[]{0.8229,0.7548,0.8052,0.8055,0.7616,0.6117,0.6273,0.6662,0.6662,0.6662}, 
          new double[]{0.8000,0.7000,0.7932,0.7963,0.7421,0.5813,0.5990,0.6435,0.6435,0.6435}, 
          new double[]{0.8304,0.7530,0.8085,0.8082,0.7604,0.6046,0.6209,0.6617,0.6617,0.6617}, 
          new double[]{0.8010,0.7091,0.7962,0.7980,0.7409,0.5732,0.5922,0.6403,0.6403,0.6403}
        }, 
        new double[][]{
          new double[]{0.8229,0.7548,0.8052,0.8055,0.7616,0.6117,0.6273,0.6662,0.6662,0.6662}, 
          new double[]{0.8000,0.7000,0.7932,0.7963,0.7421,0.5813,0.5990,0.6435,0.6435,0.6435}, 
          new double[]{0.8304,0.7530,0.8085,0.8082,0.7604,0.6046,0.6209,0.6617,0.6617,0.6617}, 
          new double[]{0.8010,0.7091,0.7962,0.7980,0.7409,0.5732,0.5922,0.6403,0.6403,0.6403}
        }, 
        new double[][]{
          new double[]{0.8229,0.7548,0.8052,0.8055,0.7616,0.6117,0.6273,0.6662,0.6662,0.6662}, 
          new double[]{0.8000,0.7000,0.7932,0.7963,0.7421,0.5813,0.5990,0.6435,0.6435,0.6435}, 
          new double[]{0.8304,0.7530,0.8085,0.8082,0.7604,0.6046,0.6209,0.6617,0.6617,0.6617}, 
          new double[]{0.8010,0.7091,0.7962,0.7980,0.7409,0.5732,0.5922,0.6403,0.6403,0.6403}
        }
      };
            double[][][] res303 = new double[][][]{
        new double[][] {
          new double[]{0.0000,0.0000,0.0000,0.0508,0.0508,0.2830,0.3149,0.3149,0.3149,0.3149}, 
          new double[]{0.0000,0.0000,0.0000,0.0978,0.0978,0.5450,0.6065,0.6065,0.6065,0.6065}, 
          new double[]{0.0000,0.0000,0.0000,0.0585,0.0585,0.3294,0.3669,0.3669,0.3669,0.3669}, 
          new double[]{0.0000,0.0000,0.0000,0.0966,0.0966,0.5748,0.6426,0.6426,0.6426,0.6426}
        }, 
        new double[][]{
          new double[]{0.0000,0.0000,0.0000,0.0508,0.0508,0.2830,0.3149,0.3149,0.3149,0.3149}, 
          new double[]{0.0000,0.0000,0.0000,0.0978,0.0978,0.5450,0.6065,0.6065,0.6065,0.6065}, 
          new double[]{0.0000,0.0000,0.0000,0.0585,0.0585,0.3294,0.3669,0.3669,0.3669,0.3669}, 
          new double[]{0.0000,0.0000,0.0000,0.0966,0.0966,0.5748,0.6426,0.6426,0.6426,0.6426}
        }, 
        new double[][]{
          new double[]{0.0000,0.0000,0.0000,0.0508,0.0508,0.2830,0.3149,0.3149,0.3149,0.3149}, 
          new double[]{0.0000,0.0000,0.0000,0.0978,0.0978,0.5450,0.6065,0.6065,0.6065,0.6065}, 
          new double[]{0.0000,0.0000,0.0000,0.0585,0.0585,0.3294,0.3669,0.3669,0.3669,0.3669}, 
          new double[]{0.0000,0.0000,0.0000,0.0966,0.0966,0.5748,0.6426,0.6426,0.6426,0.6426}
        }
      };

            nFire.Evaluators.Graded.NormalizedDiscountedCumulatedGain ndcg = new nFire.Evaluators.Graded.NormalizedDiscountedCumulatedGain();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                ndcg.GainFunction = gainFunctions[gainFunction];
                for (int discountFunction = 0; discountFunction < discountFunctions.Length; discountFunction++) {
                    ndcg.DiscountFunction = discountFunctions[discountFunction];
                    for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                        ndcg.Cutoff = cutoffs[cutoff];

                        var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], ndcg);
                        Assert.AreEqual(res301[gainFunction][discountFunction][cutoff], res[this.Task.Queries["301"]], 0.00005);
                        Assert.AreEqual(res302[gainFunction][discountFunction][cutoff], res[this.Task.Queries["302"]], 0.00005);
                        Assert.AreEqual(res303[gainFunction][discountFunction][cutoff], res[this.Task.Queries["303"]], 0.00005);
                    }
                }
            }
        }
        [TestMethod()]
        public void QMeasureTest()
        {
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(), 
        new nFire.Evaluators.Graded.ExponentialGain(2.0), 
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            double[] betas = new double[] { 0.5, 1, 2.0 };
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            double[][][] res301 = new double[][][]{
        new double[][] {
          new double[]{0.0000,0.0233,0.0156,0.0345,0.0350,0.0457,0.0431,0.0296,0.0296,0.0296}, 
          new double[]{0.0000,0.0188,0.0125,0.0292,0.0303,0.0422,0.0408,0.0285,0.0285,0.0285}, 
          new double[]{0.0000,0.0157,0.0105,0.0254,0.0268,0.0394,0.0389,0.0276,0.0276,0.0276}
        }, 
        new double[][]{
          new double[]{0.0000,0.0107,0.0071,0.0196,0.0214,0.0347,0.0356,0.0260,0.0260,0.0260}, 
          new double[]{0.0000,0.0085,0.0057,0.0162,0.0180,0.0312,0.0331,0.0248,0.0248,0.0248}, 
          new double[]{0.0000,0.0074,0.0049,0.0143,0.0160,0.0290,0.0313,0.0239,0.0239,0.0239}
        }, 
        new double[][]{
          new double[]{0.0000,0.0030,0.0020,0.0065,0.0077,0.0172,0.0212,0.0186,0.0186,0.0186}, 
          new double[]{0.0000,0.0024,0.0016,0.0054,0.0064,0.0148,0.0188,0.0173,0.0173,0.0173}, 
          new double[]{0.0000,0.0021,0.0014,0.0048,0.0057,0.0135,0.0174,0.0165,0.0165,0.0165}
        }
      };
            double[][][] res302 = new double[][][]{
        new double[][] {
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3990,0.4103,0.4302,0.4302,0.4302}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3992,0.4115,0.4370,0.4370,0.4370}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3993,0.4125,0.4444,0.4444,0.4444}
        }, 
        new double[][]{
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3992,0.4120,0.4400,0.4400,0.4400}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3993,0.4129,0.4472,0.4472,0.4472}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3994,0.4134,0.4531,0.4531,0.4531}
        }, 
        new double[][]{
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3994,0.4133,0.4518,0.4518,0.4518}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3994,0.4137,0.4563,0.4563,0.4563}, 
          new double[]{0.7100,0.5911,0.6496,0.6527,0.5899,0.3994,0.4139,0.4590,0.4590,0.4590}
        }
      };
            double[][][] res303 = new double[][][]{
        new double[][] {
          new double[]{0.0000,0.0000,0.0000,0.0093,0.0093,0.1232,0.1406,0.1406,0.1406,0.1406}, 
          new double[]{0.0000,0.0000,0.0000,0.0107,0.0107,0.1607,0.1851,0.1851,0.1851,0.1851}, 
          new double[]{0.0000,0.0000,0.0000,0.0123,0.0123,0.2134,0.2493,0.2493,0.2493,0.2493}
        }, 
        new double[][]{
          new double[]{0.0000,0.0000,0.0000,0.0107,0.0107,0.1607,0.1851,0.1851,0.1851,0.1851}, 
          new double[]{0.0000,0.0000,0.0000,0.0123,0.0123,0.2134,0.2493,0.2493,0.2493,0.2493}, 
          new double[]{0.0000,0.0000,0.0000,0.0136,0.0136,0.2746,0.3272,0.3272,0.3272,0.3272}
        }, 
        new double[][]{
          new double[]{0.0000,0.0000,0.0000,0.0125,0.0125,0.2235,0.2619,0.2619,0.2619,0.2619}, 
          new double[]{0.0000,0.0000,0.0000,0.0137,0.0137,0.2849,0.3408,0.3408,0.3408,0.3408}, 
          new double[]{0.0000,0.0000,0.0000,0.0146,0.0146,0.3403,0.4160,0.4160,0.4160,0.4160}
        }
      };

            nFire.Evaluators.Graded.QMeasure q = new nFire.Evaluators.Graded.QMeasure();
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                q.GainFunction = gainFunctions[gainFunction];
                for (int beta = 0; beta < betas.Length; beta++) {
                    q.Beta = betas[beta];
                    for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                        q.Cutoff = cutoffs[cutoff];

                        var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], q);
                        Assert.AreEqual(res301[gainFunction][beta][cutoff], res[this.Task.Queries["301"]], 0.00005);
                        Assert.AreEqual(res302[gainFunction][beta][cutoff], res[this.Task.Queries["302"]], 0.00005);
                        Assert.AreEqual(res303[gainFunction][beta][cutoff], res[this.Task.Queries["303"]], 0.00005);
                    }
                }
            }
        }
        [TestMethod()]
        public void RankBiasedPrecisionTest()
        {
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(), 
        new nFire.Evaluators.Graded.ExponentialGain(2.0), 
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            double[] persistences = new double[] { 0.5, 0.9, 0.95 };
            double[][] res301 = new double[][]{
          new double[]{0.0059,0.0465,0.0547}, 
          new double[]{0.0029,0.0233,0.0274}, 
          new double[]{0.0009,0.0069,0.0081}
        };
            double[][] res302 = new double[][]{
          new double[]{0.6497,0.5721,0.5187}, 
          new double[]{0.4331,0.3814,0.3458}, 
          new double[]{0.2887,0.2543,0.2305}
        };
            double[][] res303 = new double[][]{
          new double[]{0.0000,0.0106,0.0246}, 
          new double[]{0.0000,0.0053,0.0123}, 
          new double[]{0.0000,0.0024,0.0055}
        };
            nFire.Evaluators.Graded.RankBiasedPrecision rbp = new nFire.Evaluators.Graded.RankBiasedPrecision(4);
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                rbp.GainFunction = gainFunctions[gainFunction];
                for (int persistence = 0; persistence < persistences.Length; persistence++) {
                    rbp.Persistence = persistences[persistence];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], rbp);
                    Assert.AreEqual(res301[gainFunction][persistence], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[gainFunction][persistence], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[gainFunction][persistence], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void ExpectedReciprocalRankTest()
        {
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(),
        new nFire.Evaluators.Graded.ExponentialGain(2.0),
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            double[][] res301 = new double[][]{
        new double[]{0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0024,0.0024,0.0024}, 
       new double[] {0.0000,0.0188,0.0188,0.0275,0.0307,0.0387,0.0399,0.0402,0.0402,0.0402 }, 
        new double[]{0.0000,0.0076,0.0076,0.0114,0.0130,0.0181,0.0197,0.0209,0.0209,0.0209}
      };
            double[][] res302 = new double[][]{
        new double[] { 0.6687,0.6768,0.6774,0.6774,0.6774,0.6774,0.6774,0.6774,0.6774,0.6774 }, 
       new double[] {0.6107,0.6226,0.6241,0.6241,0.6241,0.6241,0.6241,0.6241,0.6241,0.6241  }, 
        new double[]{0.4871,0.5077,0.5124,0.5129,0.5129,0.5129,0.5129,0.5129,0.5129,0.5129}
      };
            double[][] res303 = new double[][]{
       new double[] {0.0000,0.0000,0.0000,0.0132,0.0132,0.0275,0.0278,0.0278,0.0278,0.0278 }, 
       new double[] { 0.0000,0.0000,0.0000,0.0099,0.0099,0.0230,0.0234,0.0234,0.0234,0.0234 }, 
        new double[]{0.0000,0.0000,0.0000,0.0052,0.0052,0.0144,0.0149,0.0149,0.0149,0.0149}
      };

            nFire.Evaluators.Graded.ExpectedReciprocalRank err = new nFire.Evaluators.Graded.ExpectedReciprocalRank(4);
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                err.GainFunction = gainFunctions[gainFunction];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    err.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], err);
                    Assert.AreEqual(res301[gainFunction][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[gainFunction][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[gainFunction][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
        [TestMethod()]
        public void NormalizedExpectedReciprocalRankTest()
        {
            int?[] cutoffs = new int?[] { 5, 10, 15, 20, 30, 100, 200, 500, 1000, null };
            nFire.Evaluators.Graded.IGainFunction[] gainFunctions = new Evaluators.Graded.IGainFunction[]{
        new nFire.Evaluators.Graded.LinearGain(),
        new nFire.Evaluators.Graded.ExponentialGain(2.0),
        new nFire.Evaluators.Graded.ExponentialGain(3.0)};
            double[][] res301 = new double[][]{
        new double[]{0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0028,0.0028,0.0028}, 
       new double[] {0.0000,0.0194,0.0194,0.0284,0.0317,0.0400,0.0412,0.0415,0.0415,0.0415}, 
        new double[]{0.0000,0.0076,0.0076,0.0115,0.0130,0.0182,0.0198,0.0210,0.0210,0.0210}
      };
            double[][] res302 = new double[][]{
        new double[] {0.9713,0.9765,0.9773,0.9773,0.9773,0.9773,0.9773,0.9773,0.9773,0.9773}, 
       new double[] {0.9619,0.9688,0.9706,0.9707,0.9707,0.9707,0.9707,0.9707,0.9707,0.9707}, 
        new double[]{0.9402,0.9480,0.9542,0.9547,0.9549,0.9549,0.9549,0.9549,0.9549,0.9549}
      };
            double[][] res303 = new double[][]{
       new double[] {0.0000,0.0000,0.0000,0.0290,0.0290,0.0606,0.0613,0.0613,0.0613,0.0613 }, 
       new double[] {0.0000,0.0000,0.0000,0.0266,0.0266,0.0621,0.0632,0.0632,0.0632,0.0632}, 
        new double[]{0.0000,0.0000,0.0000,0.0232,0.0232,0.0642,0.0662,0.0662,0.0662,0.0662}
      };

            nFire.Evaluators.Graded.NormalizedExpectedReciprocalRank nerr = new nFire.Evaluators.Graded.NormalizedExpectedReciprocalRank(4);
            for (int gainFunction = 0; gainFunction < gainFunctions.Length; gainFunction++) {
                nerr.GainFunction = gainFunctions[gainFunction];
                for (int cutoff = 0; cutoff < cutoffs.Length; cutoff++) {
                    nerr.Cutoff = cutoffs[cutoff];

                    var res = this.Task.EvaluateAllQueries<double>(this.Task.Systems["STANDARD"], nerr);
                    Assert.AreEqual(res301[gainFunction][cutoff], res[this.Task.Queries["301"]], 0.00005);
                    Assert.AreEqual(res302[gainFunction][cutoff], res[this.Task.Queries["302"]], 0.00005);
                    Assert.AreEqual(res303[gainFunction][cutoff], res[this.Task.Queries["303"]], 0.00005);
                }
            }
        }
    }
}
