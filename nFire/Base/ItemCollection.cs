using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nFire.Base
{
    /// <summary>
    /// A generic collection of identifiable items such as documents, queries or systems.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    [global::System.Diagnostics.DebuggerDisplay("Name = {Name}, Count = {Items.Count}")]
    public abstract class ItemCollection<T> : nFire.Core.IItemCollection<T> where T : IComparable<T>
    {
        /// <summary>
        /// An internal dictionary to store items indexed by their ID.
        /// </summary>
        protected Dictionary<string, T> Items
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the item collection.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a new generic item collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the item collection.</param>
        public ItemCollection(string name= "Unnamed")
        {
            this.Items = new Dictionary<string, T>();
            this.Name = name;
        }

        /// <summary>
        /// Gets the item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to get.</param>
        /// <returns>The item with the specified ID.</returns>
        public abstract T this[string id]
        {
            get;
        }
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Determines whether the collection contains an item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the item to locate.</param>
        /// <returns>true if the collection contains an item with the specified ID; false otherwise.</returns>
        public bool Contains(string id)
        {
            return this.Items.ContainsKey(id);
        }
        /// <summary>
        /// Returns an enumerator that iterates thought the item collection.
        /// </summary>
        /// <returns>An enumerator to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.Values.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates thought the item collection.
        /// </summary>
        /// <returns>An enumerator to iterate through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
