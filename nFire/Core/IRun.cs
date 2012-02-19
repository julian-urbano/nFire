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

using System.Collections.Generic;
using System.IO;

namespace nFire.Core
{
	/// <summary>
	/// A run made by a system for a particular query.
	/// </summary>
    /// <typeparam name="T">The type of results in the run.</typeparam>
	public interface IRun<out T> : IEnumerable<T> where T: IResult
	{
		/// <summary>
		/// Gets the query that the run is related to.
		/// </summary>
        IQuery Query { get; }
		/// <summary>
		/// Gets the system that made the run.
		/// </summary>
        ISystem System { get; }
        /// <summary>
        /// Returns the number of judgments in the run.
        /// </summary>
        int Count { get; }
	}

    /// <summary>
    /// Reads runs with a particular format.
    /// </summary>
    /// <typeparam name="T">The type of results in the run.</typeparam>
    public interface IRunReader<T> where T : IResult
    {
        /// <summary>
        /// Reads runs from the specified file.
        /// </summary>
        /// <param name="path">The path to the file to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the runs read.</returns>
        IEnumerable<IRun<T>> Read(string path, ITask<T> task);
        /// <summary>
        /// Reads runs from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="task">The task containing the documents, queries and systems.</param>
        /// <returns>An enumerable with the runs read.</returns>
        IEnumerable<IRun<T>> Read(Stream stream, ITask<T> task);
    }

    /// <summary>
    /// Writes runs with a particular format.
    /// </summary>
    /// <typeparam name="T">The type of results in the run.</typeparam>
    public interface IRunWriter<in T> where T : IResult
    {
        /// <summary>
        /// Writes the specified runs to the specified file.
        /// </summary>
        /// <param name="runs">The runs to write.</param>
        /// <param name="path">The path to the file.</param>
        void Write(IEnumerable<IRun<T>> runs, string path);
        /// <summary>
        /// Writes the specified runs to the specified file.
        /// </summary>
        /// <param name="runs">The runs to write.</param>
        /// <param name="stream">The path to the file.</param>
        void Write(IEnumerable<IRun<T>> runs, Stream stream);
    }
}
