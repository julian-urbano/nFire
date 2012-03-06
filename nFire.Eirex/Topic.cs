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
using System.Linq;

namespace nFire.Eirex
{
    /// <summary>
    /// An EIREX topic.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Id = {Id}, Title = {Title}")]
    public class Topic : nFire.Base.Query
    {
        /// <summary>
        /// Gets or sets the topic title.
        /// </summary>
        public string Title
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets or sets the relevance definition of the topic.
        /// </summary>
        public Dictionary<int,string> Relevance
        {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a new EIREX topic with the specified D, title and relevance definition.
        /// </summary>
        /// <param name="id">The ID of the new topic.</param>
        /// <param name="title">The title of the new topic.</param>
        /// <param name="relevance">The relevance definition of the new topic.</param>
        protected Topic(string id, string title, Dictionary<int,string> relevance)
            : base(id)
        {
            this.Title = title;
            this.Relevance = relevance;
        }

        /// <summary>
        /// Deserializes a new topic from the specified XmlReader.
        /// </summary>
        /// <param name="reader">The XmlReader to read from.</param>
        /// <returns>The new desserialized topic.</returns>
        internal static Topic Deserialize(System.Xml.XmlReader reader)
        {
            string id = reader.GetAttribute("id");
            reader.ReadToDescendant("title");
            string title = reader.ReadString();
            Dictionary<int, string> relevance = new Dictionary<int, string>();
            reader.ReadToFollowing("relevance");
            reader.ReadToDescendant("level");
            do{
                int value = int.Parse(reader.GetAttribute("value"));
                string description = reader.ReadString();
                relevance.Add(value, description);
            }while(reader.ReadToNextSibling("level"));
            reader.ReadOuterXml();
            reader.ReadOuterXml();
            return new Topic(id, title, relevance);
        }
    }

    /// <summary>
    /// A collection of EIREX topics.
    /// </summary>
    [global::System.Diagnostics.DebuggerDisplay("Name = {Name}, Count = {Items.Count}")]
    public class TopicCollection : nFire.Base.ItemCollection<Topic>
    {
        /// <summary>
        /// Creates a new collection of EIREX topics as per the specified topic file.
        /// </summary>
        /// <param name="pathToTopics">The path to the topic definition file.</param>
        public TopicCollection(string pathToTopics): base("Unnamed")
        {
            System.IO.Stream stream = System.IO.File.OpenRead(pathToTopics);
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stream);
            reader.ReadToDescendant("topic");
            do {
                Topic t = Topic.Deserialize(reader);
                base.Items.Add(t.Id, t);
            } while (reader.ReadToNextSibling("topic"));
            stream.Close();

            // Figure out the name
            Topic top = base.Items.First().Value;
            string year = top.Id.Split('-')[0];
            base.Name = "EIREX " + year;
        }

        /// <summary>
        /// Gets the topic with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the topic to get.</param>
        /// <returns>The topic with the specified ID.</returns>
        public override Topic this[string id]
        {
            get
            {
                return base.Items[id];
            }
        }
    }
}
