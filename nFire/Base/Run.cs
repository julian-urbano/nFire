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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using nFire.Core;

namespace nFire.Base
{
    /// <summary>
    /// A base list of judgments made by a retrieval system for a particular query.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("System = {System.Id}, Query = {Query.Id}, Count = {Count}")]
    public class Run<T> : IRun<T> where T : IResult
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
        /// Gets or sets the list of results in the run.
        /// </summary>
        protected List<T> Results
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new base run for the specified system and the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="system">The system.</param>
        public Run(IQuery query, ISystem system)
        {
            this.Results = new List<T>();
            this.Query = query;
            this.System = system;
        }

        /// <summary>
        /// Returns the number of results in the run.
        /// </summary>
        public int Count {
            get { return this.Results.Count; }
        }
        /// <summary>
        /// Adds the specified result to the run.
        /// </summary>
        /// <param name="t">The result to add.</param>
        public void Add(T t) {
            this.Results.Add(t);
        }

        /// <summary>
        /// Returns an enumerator that iterates thought the results in the run.
        /// </summary>
        /// <returns>An enumerator to iterate through the run</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Results.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates thought the results in the run.
        /// </summary>
        /// <returns>An enumerator to iterate through the run</returns>
        /// <returns>An enumerator to iterate through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Results.GetEnumerator();
        }
    }
}