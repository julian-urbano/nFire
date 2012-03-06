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

namespace nFire.Eirex
{
    /// <summary>
    /// Reads and writes runs in EIREX format.
    /// It is assumed that the results are ordered by rank.
    /// </summary>
    public class Formatter: IRunReader<IListResult>, IRunWriter<IListResult>
    {
        /// <summary>
        /// Reads run from the specified file.
        /// </summary>
        /// <param name="path">The path to the file to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the runs read.</returns>
        public IEnumerable<IRun<IListResult>> Read(string path, ITask<IListResult> task)
        {
            using (Stream stream = File.OpenRead(path)) {
                return this.Read(stream, task);
            }
        }
        /// <summary>
        /// Reads runs from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the runs read.</returns>
        public IEnumerable<IRun<IListResult>> Read(Stream stream, ITask<IListResult> task)
        {
            List<IRun<IListResult>> allRuns = new List<IRun<IListResult>>();
            Run<IListResult> currentRun = null;

            using (StreamReader reader = new StreamReader(stream)) {
                string line = reader.ReadLine();
                while (line != null) {
                    string[] parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 4) {
                        throw new FormatException("\"" + line + "\" is not a well-formatted run line");
                    }
                    string systemid = parts[0];
                    string queryid = parts[1];
                    string documentid = parts[2];
                    double score = double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

                    // If the topic changes, add the previous list and create a new one
                    if (currentRun == null || currentRun.Query.Id != queryid) {
                        // Add the previous only if it's not null (first topic)
                        if (currentRun != null) {
                            allRuns.Add(currentRun);
                        }
                        currentRun = new Run<IListResult>(task.Queries[queryid], task.Systems[systemid]);
                    }
                    currentRun.Add(new ListResult(task.Documents[documentid], score, currentRun.Count + 1));

                    line = reader.ReadLine();
                }
                // In case no second list was read
                if (currentRun != null) {
                    allRuns.Add(currentRun);
                }
            }
            return allRuns;
        }

        /// <summary>
        /// Writes the specified runs to the specified file.
        /// </summary>
        /// <param name="runs">The runs to write.</param>
        /// <param name="path">The path to the file.</param>
        public void Write(IEnumerable<IRun<IListResult>> runs, string path)
        {
            using (Stream stream = File.Create(path)) {
                this.Write(runs, stream);
            }
        }
        /// <summary>
        /// Writes the specified runs to the specified file.
        /// </summary>
        /// <param name="runs">The runs to write.</param>
        /// <param name="stream">The path to the file.</param>
        public void Write(IEnumerable<IRun<IListResult>> runs, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream)) {
                foreach (IRun<IListResult> run in runs) {
                    foreach (IListResult res in run) {
                        string line = string.Format("{0}\t{1}\t{2}\t{3}", run.System.Id, run.Query.Id, res.Document.Id, res.Score.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        writer.WriteLine(line);
                    }
                }
                writer.Flush();
            }
        }
    }
}
