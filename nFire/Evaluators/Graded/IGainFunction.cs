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

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// Specifies a function that maps relevance scores onto information gains for calculation of gain-based measures.
    /// </summary>
    public interface IGainFunction
    {
        /// <summary>
        /// Computes the gain value corresponding to the specified relevance score.
        /// </summary>
        /// <param name="score">The relevance score.</param>
        /// <returns>The corresponding information gain.</returns>
        double Gain(double score);
    }

    /// <summary>
    /// Provides a 1 to 1 mapping between relevance scores and information gain values (i.e. gain = relevance).
    /// </summary>
    public class LinearGain : IGainFunction
    {
        /// <summary>
        /// Computes the gain value corresponding to the specified relevance score.
        /// </summary>
        /// <param name="score">The relevance score.</param>
        /// <returns>The corresponding information gain.</returns>
        public double Gain(double score)
        {
            return score;
        }
    }
    /// <summary>
    /// Provides an exponential mapping between relevance scores and information gain values (i.e. gain = 0 if relevance = 0, or gain = base^relevance otherwise).
    /// </summary>
    public class ExponentialGain : IGainFunction
    {
        /// <summary>
        /// Gets and sets the exponentiation base to use.
        /// </summary>
        public double Base
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an exponential mapping function with the specified base.
        /// </summary>
        /// <param name="theBase">The exponentiation base.</param>
        public ExponentialGain(double theBase = 2.0)
        {
            this.Base = theBase;
        }

        /// <summary>
        /// Computes the gain value corresponding to the specified relevance score.
        /// </summary>
        /// <param name="score">The relevance score.</param>
        /// <returns>The corresponding information gain.</returns>
        public double Gain(double score)
        {
            if (score == 0.0) return 0;
            else return Math.Pow(this.Base, score);
        }
    }
}
