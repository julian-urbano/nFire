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

namespace nFire.Core
{
    /// <summary>
    /// An evaluator.
    /// </summary>
    /// <typeparam name="TScore">The type of results the evaluator works with.</typeparam>
    /// <typeparam name="TResult">The type of the scores returned by the ealuator.</typeparam>
    public interface IEvaluator<TScore,in TResult> where TResult:IResult
    {
        /// <summary>
        /// Gets the abbreviated name of the evaluator.
        /// </summary>
        string ShortName { get; }
        /// <summary>
        /// Gets the full name of the evaluator.
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// Evaluates a system according to a ground truth.
        /// </summary>
        /// <param name="groundTruth">The ground truth.</param>
        /// <param name="systemRun">The system run.</param>
        /// <returns>The evaluation score.</returns>
        TScore Evaluate(IRun<TResult> groundTruth, IRun<TResult> systemRun);
    }
}
