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
	/// A base document.
	/// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Id = {Id}")]
	public class Document : IDocument
    {
        /// <summary>
        /// Gets the document ID.
        /// </summary>
        public string Id
        {
            get;
            protected set;
        }

		/// <summary>
		/// Creates a base document with the specified ID.
		/// </summary>
		/// <param name="id">The ID of the new document.</param>
        public Document(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Compares this document to the specified document, according to their IDs.
        /// </summary>
        /// <param name="other">A document to compare.</param>
        /// <returns>A signed number indicating the relative other of this document with respect to the specified document.</returns>
        public int CompareTo(IDocument other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }

    /// <summary>
    /// A collection of base documents.
    /// </summary>
    //[global::System.Diagnostics.DebuggerDisplay("Count = {base.Items.Count}")]
    public class DocumentCollection : nFire.Base.ItemCollection<Document>
    {
        /// <summary>
        /// Gets the document with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the document to get.</param>
        /// <returns>The document with the specified ID.</returns>
        public override Document this[string id]
        {
            get
            {
                Document document;
                if (!base.Items.TryGetValue(id, out document)) {
                    document = new Document(id);
                    this.Items.Add(id, document);
                }
                return document;
            }
        }
    }
}
