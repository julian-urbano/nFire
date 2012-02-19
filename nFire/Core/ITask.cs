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

namespace nFire.Core
{
    /// <summary>
    /// A testbed to analyze several systems for several queries.
    /// </summary>
    public interface ITask<T> where T : IResult
    {
        /// <summary>
        /// Gets the name of the testbed.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the document collection.
        /// </summary>
        nFire.Core.IItemCollection<IDocument> Documents { get; }
        /// <summary>
        /// Gets the query collection.
        /// </summary>
        nFire.Core.IItemCollection<IQuery> Queries { get; }
        /// <summary>
        /// Gets the system collection.
        /// </summary>
        nFire.Core.IItemCollection<ISystem> Systems { get; }

        /// <summary>
        /// Uses the specified reader to read runs from the specified file and adds them as ground truth .
        /// </summary>
        /// <param name="reader">The reader to read runs from the file.</param>
        /// <param name="path">The path to the file containing the runs.</param>
        void AddGroundTruths(IRunReader<T> reader, string path);
        /// <summary>
        /// Adds the specified runs as ground truth.
        /// </summary>
        /// <param name="runs">The runs to add as ground truth.</param>
        void AddGroundTruths(IEnumerable<IRun<T>> runs);
        /// <summary>
        /// Adds the specified run as ground truth.
        /// </summary>
        /// <param name="run">The run to add as ground truth.</param>
        void AddGroundTruth(IRun<T> run);

        /// <summary>
        /// Uses the specified reader to read runs from the specified file and adds them as system runs.
        /// </summary>
        /// <param name="reader">The reader to read runs from the file.</param>
        /// <param name="path">The path to the file containing the runs.</param>
        void AddSystemRuns(IRunReader<T> reader, string path);
        /// <summary>
        /// Adds the specified runs as system runs.
        /// </summary>
        /// <param name="runs">The runs to add as system runs.</param>
        void AddSystemRuns(IEnumerable<IRun<T>> runs);
        /// <summary>
        /// Adds the specified run as system run.
        /// </summary>
        /// <param name="run">The run to add as system run.</param>
        void AddSystemRun(IRun<T> run);

        /// <summary>
        /// Evaluates the run of the specified system for the specified query according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="system">The system to evaluate.</param>
        /// <param name="query">The query to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The score returned by the evaluator.</returns>
        TScore Evaluate<TScore>(ISystem system, IQuery query, IEvaluator<TScore, T> eval);
        /// <summary>
        /// Evaluates all runs of the specified query according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="query">The query to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        IDictionary<ISystem, TScore> EvaluateAllSystems<TScore>(IQuery query, IEvaluator<TScore, T> eval);
        /// <summary>
        /// Evaluates all runs of the specified system according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="system">The system to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        IDictionary<IQuery, TScore> EvaluateAllQueries<TScore>(ISystem system, IEvaluator<TScore, T> eval);
        /// <summary>
        /// Evaluates all runs according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        IDictionary<ISystem, IDictionary<IQuery, TScore>> EvaluateAllSystems<TScore>(IEvaluator<TScore, T> eval);
        /// <summary>
        /// Evaluates all runs according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        IDictionary<IQuery, IDictionary<ISystem, TScore>> EvaluateAllQueries<TScore>(IEvaluator<TScore, T> eval);
    }
}
