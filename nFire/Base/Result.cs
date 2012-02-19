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

namespace nFire.Base
{
    /// <summary>
    /// A base judgment of a document for a query in set-based retrieval. 
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Document = {Document.Id}, Score = {Score}")]
    public class SetResult : ISetResult
    {
        /// <summary>
        /// Creates a new result for a document.
        /// </summary>
        /// <param name="document">The document retrieved.</param>
        /// <param name="score">The score given to the document.</param>
        public SetResult(IDocument document, double score)
        {
            this.Document = document;
            this.Score = score;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public IDocument Document
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the score given to the document.
        /// </summary>
        public double Score
        {
            get;
            protected set;
        }
    }
    /// <summary>
    /// A base judgment of a document for a query in list-based retrieval. 
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Document = {Document.Id}, Score = {Score}, Rank = {Rank}")]
    public class ListResult : SetResult, IListResult
    {
        /// <summary>
        /// Creates a new result for a document.
        /// </summary>
        /// <param name="document">The document retrieved.</param>
        /// <param name="score">The score given to the document.</param>
        /// <param name="rank">The rank at which the document was retrieved.</param>
        public ListResult(IDocument document, double score, int rank)
            :base(document,score)
        {
            this.Rank = rank;
        }

        /// <summary>
        /// Gets the rank at which the document was retrieved.
        /// </summary>
        public int Rank
        {
            get;
            protected set;
        }
    }
}