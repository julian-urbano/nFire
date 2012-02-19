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
using System.IO;
using nFire.Base;
using nFire.Core;

namespace nFire.Trec
{
    /// <summary>
    /// Reads ground truth data in TREC format.
    /// </summary>
    public class QrelFormatter : IRunReader<IListResult>, IRunWriter<IListResult>
    {
        /// <summary>
        /// Reads qrels from the specified file.
        /// </summary>
        /// <param name="path">The path to the file to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the qrels read.</returns>
        public IEnumerable<IRun<IListResult>> Read(string path, ITask<IListResult> task)
        {
            using (Stream stream= File.OpenRead(path)) {
                return this.Read(stream, task);
            }
        }
        /// <summary>
        /// Reads qrels from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the qrels read.</returns>
        public IEnumerable<IRun<IListResult>> Read(Stream stream, ITask<IListResult> task)
        {
                List<IRun<IListResult>> allRuns = new List<IRun<IListResult>>();
                Run<IListResult> currentRun = null;
            using (StreamReader reader = new StreamReader(stream)) {

                string line = reader.ReadLine();
                while (line != null) {
                    string[] parts = line.Split(' ', '\t');
                    if (parts.Length != 4) {
                        throw new FormatException("\"" + line + "\" is not a well-formatted Qrel line");
                    }
                    string topicId = parts[0];
                    string documentId = parts[2];
                    double relevance = double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

                    // If the topic changes, add the previous list and create a new one
                    if (currentRun == null || currentRun.Query.Id != topicId) {
                        // Add the previous only if it's not null (first topic)
                        if (currentRun != null) {
                            allRuns.Add(currentRun);
                        }
                        currentRun = new Run<IListResult>(task.Queries[topicId], task.Systems["Qrel"]);
                    }
                    if(relevance>=0)
                    currentRun.Add(new ListResult(task.Documents[documentId], relevance, currentRun.Count + 1));

                    line = reader.ReadLine();
                }

                // In case no list was read
                if (currentRun != null) {
                    allRuns.Add(currentRun);
                }
            }
            return allRuns;
        }

        /// <summary>
        /// Writes the specified qrels to the specified file.
        /// </summary>
        /// <param name="runs">The qrels to write.</param>
        /// <param name="path">The path to the file.</param>
        public void Write(IEnumerable<IRun<IListResult>> runs, string path)
        {
            using (Stream stream = File.Create(path)) {
                this.Write(runs, stream);
            }
        }
        /// <summary>
        /// Writes the specified qrels to the specified file.
        /// </summary>
        /// <param name="runs">The qrels to write.</param>
        /// <param name="stream">The path to the file.</param>
        public void Write(IEnumerable<IRun<IListResult>> runs, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream)) {
                foreach (IRun<IListResult> run in runs) {
                    foreach (IListResult res in run) {
                        string line = string.Format("{0} 0 {1} {2}", run.Query.Id, res.Document.Id, res.Score.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        writer.WriteLine(line);
                    }
                }
                writer.Flush();
            }
        }
    }
}
