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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nFire.Test
{
    [TestClass()]
    public class Trec
    {
        [TestMethod()]
        public void QrelReadTest()
        {
            ITask<IListResult> task = new nFire.Base.Task<IListResult>();
            QrelFormatter target = new QrelFormatter();
            using (MemoryStream stream = new MemoryStream(Resources.qrels)) {
                List<IRun<IListResult>> qrels = new List<IRun<IListResult>>(target.Read(stream, task));
                Assert.AreEqual(qrels.Count, 3);

                IRun<IListResult> qrel = qrels[0];
                Assert.AreEqual("301", qrel.Query.Id);
                Assert.AreEqual("Qrel", qrel.System.Id);
                Assert.AreEqual(1708, qrel.Count);
                Assert.AreEqual("CR93E-10279", qrel.First().Document.Id);
                Assert.AreEqual("LA123090-0148", qrel.Last().Document.Id);
                Assert.AreEqual(4, qrel.ElementAt(38).Score);
                Assert.AreEqual(1, qrel.ElementAt(958).Score);

                qrel = qrels[1];
                Assert.AreEqual("302", qrel.Query.Id);
                Assert.AreEqual("Qrel", qrel.System.Id);
                Assert.AreEqual(1061, qrel.Count);
                Assert.AreEqual("CR93E-10071", qrel.First().Document.Id);
                Assert.AreEqual("LA123090-0026", qrel.Last().Document.Id);
                Assert.AreEqual(3, qrel.ElementAt(191).Score);
                Assert.AreEqual(0, qrel.ElementAt(555).Score);

                qrel = qrels[2];
                Assert.AreEqual("303", qrel.Query.Id);
                Assert.AreEqual("Qrel", qrel.System.Id);
                Assert.AreEqual(608, qrel.Count);
                Assert.AreEqual("CR93E-11182", qrel.First().Document.Id);
                Assert.AreEqual("LA122990-0030", qrel.Last().Document.Id);
                Assert.AreEqual(0, qrel.ElementAt(96).Score);
                Assert.AreEqual(0, qrel.ElementAt(538).Score);
            }
        }
        [TestMethod()]
        public void RunReadTest()
        {
            ITask<IListResult> task = new nFire.Base.Task<IListResult>();
            RunFormatter target = new RunFormatter();
            using (MemoryStream stream = new MemoryStream(Resources.results)) {
                List<IRun<IListResult>> qrels = new List<IRun<IListResult>>(target.Read(stream, task));
                Assert.AreEqual(qrels.Count, 3);

                IRun<IListResult> qrel = qrels[0];
                Assert.AreEqual("301", qrel.Query.Id);
                Assert.AreEqual("STANDARD", qrel.System.Id);
                Assert.AreEqual(500, qrel.Count);
                Assert.AreEqual("FBIS4-50478", qrel.First().Document.Id);
                Assert.AreEqual("FBIS3-20713", qrel.Last().Document.Id);
                Assert.AreEqual(2.419756, qrel.ElementAt(37).Score);
                Assert.AreEqual(1.887025, qrel.ElementAt(257).Score);

                qrel = qrels[1];
                Assert.AreEqual("302", qrel.Query.Id);
                Assert.AreEqual("STANDARD", qrel.System.Id);
                Assert.AreEqual(500, qrel.Count);
                Assert.AreEqual("FR940126-2-00106", qrel.First().Document.Id);
                Assert.AreEqual("FBIS3-41700", qrel.Last().Document.Id);
                Assert.AreEqual(1.38714, qrel.ElementAt(190).Score);
                Assert.AreEqual(1.124015, qrel.ElementAt(454).Score);

                qrel = qrels[2];
                Assert.AreEqual("303", qrel.Query.Id);
                Assert.AreEqual("STANDARD", qrel.System.Id);
                Assert.AreEqual(500, qrel.Count);
                Assert.AreEqual("LA033090-0082", qrel.First().Document.Id);
                Assert.AreEqual("LA021990-0048", qrel.Last().Document.Id);
                Assert.AreEqual(1.901362, qrel.ElementAt(95).Score);
                Assert.AreEqual(0.923633, qrel.ElementAt(337).Score);
            }
        }
    }
}