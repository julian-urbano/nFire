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
using System.Linq;
using System.Text;

namespace nFire.Core
{
    /// <summary>
    /// A collection of identifiable items such as documents, queries or systems.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IItemCollection<out T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to get.</param>
        /// <returns>The item with the specified ID.</returns>
        T this[string id] { get; }
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Determines whether the collection contains an item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to locate.</param>
        /// <returns>true if the collection contains an item with the specified ID; false otherwise.</returns>
        bool Contains(string id);
    }
}
