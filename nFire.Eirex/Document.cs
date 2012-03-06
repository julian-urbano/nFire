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
using System.IO;
using System.Linq;

namespace nFire.Eirex
{
    /// <summary>
    /// An EIREX document.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Id = {Id}")]
    public class Document : nFire.Base.Document
    {
        /// <summary>
        /// Gets or sets the path to the collection, either a directory or a TAR archive.
        /// </summary>
        protected string Path
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the byte offset within the TAR archive where the document content starts.
        /// </summary>
        /// <remarks>Only used with TAR-based document collections.</remarks>
        protected long Offset
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the byte size of the document content.
        /// </summary>
        /// <remarks>Only used with TAR-based document collections.</remarks>
        protected int Size
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new EIREX document with the specified ID and from a directory-based collection.
        /// </summary>
        /// <param name="id">The ID of the new document.</param>
        /// <param name="path">The path to the file containing the document.</param>
        internal Document(string id, string path)
            : base(id)
        {
            this.Path = path;
            this.Offset = -1;
            this.Size = -1;
        }
        /// <summary>
        /// Creates a new EIREX document with the specified ID and from a TAR-based collection.
        /// </summary>
        /// <param name="id">The ID of the new document.</param>
        /// <param name="path">The path to the TAR archive containing the collection.</param>
        /// <param name="offset">The byte offset within the TAR archive where the document starts.</param>
        /// <param name="size">The byte size of the document content.</param>
        internal Document(string id, string path, long offset, int size)
            : base(id)
        {
            this.Path = path;
            this.Offset = offset;
            this.Size = size;
        }

        /// <summary>
        /// Gets the contents of the document.
        /// </summary>
        public string Contents
        {
            get
            {
                if (this.Offset<0 || this.Size<0) {
                    // Collection as a directory
                    using (StreamReader inStream = new StreamReader(File.OpenRead(this.Path))) {
                        return inStream.ReadToEnd();
                    }
                } else {
                    // Collection as a tar archive
                    // Use an intermediate MemoryStream to handle encoding
                    using (MemoryStream memStream = new MemoryStream(this.Size)) {
                        using (Stream inStream = File.OpenRead(this.Path)) {
                            byte[] buf = new byte[this.Size];
                            inStream.Seek(this.Offset, SeekOrigin.Begin);
                            inStream.Read(buf, 0, this.Size);
                            memStream.Write(buf, 0, this.Size);
                            memStream.Seek(0, SeekOrigin.Begin);
                        }
                        using (StreamReader inStream = new StreamReader(memStream)) {
                            return inStream.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// A collection of EIREX documents.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Name = {Name}, Count = {Items.Count}")]
    public class DocumentCollection : nFire.Base.ItemCollection<Document>
    {
        /// <summary>
        /// Extension of the ICSharpCode.SharpZipLib.Tar.TarInputStream to provide the real byte position of a TarEntry.
        /// The original implementation uses a buffer, so the Position provided is the position by the end of the buffer, not the TarEntry position.
        /// </summary>
        protected class TarStream : ICSharpCode.SharpZipLib.Tar.TarInputStream
        {
            /// <summary>
            /// Creates a new TAR input stream from the specified stream.
            /// </summary>
            /// <param name="stream">The stream of the TAR archive.</param>
            internal TarStream(Stream stream)
                : base(stream)
            {
            }
            /// <summary>
            /// Gets the byte position of the TAR stream.
            /// </summary>
            internal long RealPosition
            {
                get
                {
                    return base.tarBuffer.RecordSize * base.tarBuffer.CurrentRecord + ICSharpCode.SharpZipLib.Tar.TarBuffer.BlockSize * base.tarBuffer.CurrentBlock;
                }
            }
        }

        /// <summary>
        /// Creates a new collection of EIREX documents from the specified path.
        /// </summary>
        /// <param name="pathToDocuments">The path to the collection: either the directory under which all documents are, or a TAR file with all documents archived.</param>
        public DocumentCollection(string pathToDocuments)
        {
            if (File.Exists(pathToDocuments)) {
                // Collection as a TAR archive
                var input = new DocumentCollection.TarStream(File.OpenRead(pathToDocuments));

                ICSharpCode.SharpZipLib.Tar.TarEntry entry = null;
                while ((entry = input.GetNextEntry()) != null) {
                    if (!entry.IsDirectory) {
                        string docid = Path.GetFileNameWithoutExtension(entry.Name);
                        long start = input.RealPosition;
                        long size = entry.Size;

                        base.Items.Add(docid, new Document(docid, pathToDocuments, start, (int)size));
                    }
                }
            } else if (Directory.Exists(pathToDocuments)) {
                // Collection as a directory
                foreach(string dir in Directory.GetDirectories(pathToDocuments)){
                    foreach(string file in Directory.GetFiles(dir)){
                        string docid = Path.GetFileNameWithoutExtension(file);
                        base.Items.Add(docid, new Document(docid, file));
                    }
                }
            } else
                throw new ArgumentException("\"" + pathToDocuments + "\" is not a valid path to an EIREX document collection.");

            // Figure out the name
            Document doc = base.Items.First().Value;
            string year = doc.Id.Split('-')[0];
            base.Name = "EIREX " + year;
        }

        /// <summary>
        /// Gets the document with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the document to get.</param>
        /// <returns>The document with the specified ID.</returns>
        public override Document this[string id]
        {
            get
            {
                return base.Items[id];
            }
        }
    }
}