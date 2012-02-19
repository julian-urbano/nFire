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
    /// A base system.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Id = {Id}")]
    public class System : ISystem
    {
        /// <summary>
        /// Gets the system ID.
        /// </summary>
        public string Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a base system with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the new system.</param>
        public System(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Compares this system to the specified system, according to their IDs.
        /// </summary>
        /// <param name="other">A system to compare.</param>
        /// <returns>A signed number indicating the relative other of this system with respect to the specified system.</returns>
        public int CompareTo(ISystem other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }

    /// <summary>
    /// A collection of base systems.
    /// </summary>
    public class SystemCollection : nFire.Base.ItemCollection<System>
    {       
        /// <summary>
        /// Gets the system with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the system to get.</param>
        /// <returns>The system with the specified ID.</returns>
        public override System this[string id]
        {
            get
            {
                System system;
                if (!base.Items.TryGetValue(id, out system)) {
                    system = new System(id);
                    this.Items.Add(id, system);
                }
                return system;
            }
        }
    }
}
