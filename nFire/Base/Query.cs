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
using nFire.Core;

namespace nFire.Base
{
    /// <summary>
    /// A base query.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Id = {Id}")]
    public class Query : IQuery
    {
        /// <summary>
        /// Gets the query ID.
        /// </summary>
        public string Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a base query with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the new query.</param>
        public Query(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Compares this query to the specified query, according to their IDs.
        /// </summary>
        /// <param name="other">A query to compare.</param>
        /// <returns>A signed number indicating the relative other of this query with respect to the specified query.</returns>
        public int CompareTo(IQuery other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }

    /// <summary>
    /// A collection of base queries.
    /// </summary>
    public class QueryCollection : nFire.Base.ItemCollection<Query>
    {
        /// <summary>
        /// Gets the query with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the query to get.</param>
        /// <returns>The query with the specified ID.</returns>
        public override Query this[string id]
        {
            get
            {
                Query query;
                if (!base.Items.TryGetValue(id, out query)) {
                    query = new Query(id);
                    this.Items.Add(id, query);
                }
                return query;
            }
        }
    }
}
