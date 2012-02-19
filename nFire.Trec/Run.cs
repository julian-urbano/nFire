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

using System.Collections;
using System.Collections.Generic;
using nFire.Base;
using nFire.Core;

namespace nFire.Trec
{
    /// <summary>
    /// Represents a list of judgments made by a TREC system for a particular query.
    /// Documents are internally sorted by rank regardless of the order in which they are added.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("System = {System.Id}, Query = {Query.Id}, Count = {Count}")]
    public class Run : IRun<ListResult>
    {
        /// <summary>
        /// Gets and sets the query that the run is related to.
        /// </summary>
        public Core.IQuery Query
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets and sets the system that made the run.
        /// </summary>
        public Core.ISystem System
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the sorted list of results in the run.
        /// </summary>
        protected SortedList<int, ListResult> Results
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a TREC run for a particular system and a particular query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="system">The retrieval system.</param>
        public Run(IQuery query, ISystem system)
        {
            this.Results = new SortedList<int, ListResult>();
            this.Query = query;
            this.System = system;
        }

        /// <summary>
        /// Returns the number of results in the run.
        /// </summary>
        public int Count
        {
            get { return this.Results.Count; }
        }
        /// <summary>
        /// Adds the specified result to the run.
        /// </summary>
        /// <param name="t">The result to add.</param>
        public void Add(ListResult t)
        {
            this.Results.Add(t.Rank, t);
        }

        /// <summary>
        /// Returns an enumerator that iterates thought the results in the run.
        /// </summary>
        /// <returns>An enumerator to iterate through the run</returns>
        public IEnumerator<ListResult> GetEnumerator()
        {
            return this.Results.Values.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates thought the results in the run.
        /// </summary>
        /// <returns>An enumerator to iterate through the run</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Results.GetEnumerator();
        }
    }
}
