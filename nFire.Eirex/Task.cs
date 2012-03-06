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

using nFire.Core;

namespace nFire.Eirex
{
    /// <summary>
    /// An EIREX task.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Name = {Name}, Documents = {Documents.Count}, Topics = {Queries.Count}")]
    public class Task : nFire.Base.Task<IListResult>
    {
        /// <summary>
        /// An EIREX formatter to read trels and system runs.
        /// </summary>
        protected Formatter Formatter
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new EIREX task with the specified name and paths to document collection and topic definitions
        /// </summary>
        /// <param name="name">The name of the new task.</param>
        /// <param name="pathToDocuments">The path to the collection: either the directory under which all documents are, or a TAR file with all documents archived.</param>
        /// <param name="pathToTopics">The path to the topic definition file.</param>
        public Task(string name, string pathToDocuments, string pathToTopics)
        {
            base.Name = name;

            this.Documents = new DocumentCollection(pathToDocuments);
            this.Queries = new TopicCollection(pathToTopics);
            this.Systems = new nFire.Base.SystemCollection();

            this.Formatter = new Formatter();
        }

        /// <summary>
        /// Reads runs in EIREX format from the specified file and adds them as ground truth.
        /// </summary>
        /// <param name="path">The path to the file containing the runs.</param>
        public void AddGroundTruths(string path)
        {
            this.AddGroundTruths(this.Formatter, path);
        }
        /// <summary>
        /// Reads runs in EIREX format from the specified file and adds them as system runs.
        /// </summary>
        /// <param name="path">The path to the file containing the runs.</param>
        public void AddSystemRuns(string path)
        {
            this.AddSystemRuns(this.Formatter, path);
        }
    }
}
