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

namespace nFire.Evaluators.Graded
{
    /// <summary>
    /// Specifies a function that discounts gain scores, based on their rank, for calculation of gain-based measures.
    /// </summary>
    /// <remarks>The returned discount factor divides gain, it does not multiply (i.e. score = gain / discount, rather than gain * discount).</remarks>
    public interface IDiscountFunction
    {
        /// <summary>
        /// Computes the discount factor corresponding to the specified rank.
        /// </summary>
        /// <param name="rank">The 1-based rank.</param>
        /// <returns>The discount factor.</returns>
        double Discount(int rank);
    }

    /// <summary>
    /// The original (n)DCG discount function as in Järveling &amp; Kekäläinen, ACM TOIS, 2002 (i.e. 1 if i&lt;b or log_b(i) if i&lt;=b).
    /// </summary>
    public class OriginalDiscount : IDiscountFunction
    {
        /// <summary>
        /// Gets and sets the logarithm base to use.
        /// </summary>
        public double LogBase
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new discount function as in the original (n)DCG formulation.
        /// </summary>
        /// <param name="logBase">The logarithm base to use.</param>
        public OriginalDiscount(double logBase = 2.0)
        {
            this.LogBase = logBase;
        }

        /// <summary>
        /// Computes the discount factor corresponding to the specified rank.
        /// </summary>
        /// <param name="rank">The 1-based rank.</param>
        /// <returns>The discount factor.</returns>
        public double Discount(int rank)
        {
            if (rank < this.LogBase) return 1;
            else return Math.Log(rank, this.LogBase);
        }
    }
    /// <summary>
    /// The function introduced by Microsoft (i.e. log_b(i+b-1)).
    /// </summary>
    public class LogarithmicDiscount : IDiscountFunction
    {
        /// <summary>
        /// Gets and sets the logarithm base to use.
        /// </summary>
        public double LogBase
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new discount function as in the Microsoft (n)DCG formulation.
        /// </summary>
        /// <param name="logBase">The logarithm base to use.</param>
        public LogarithmicDiscount(double logBase = 2.0)
        {
            this.LogBase = logBase;
        }

        /// <summary>
        /// Computes the discount factor corresponding to the specified rank.
        /// </summary>
        /// <param name="rank">The 1-based rank.</param>
        /// <returns>The discount factor.</returns>
        public double Discount(int rank)
        {
            return Math.Log(rank + this.LogBase -1, this.LogBase);
        }
    }
}
