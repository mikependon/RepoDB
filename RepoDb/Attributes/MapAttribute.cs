using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute class used to define a mapping of the current class/property equivalent to an object/field name in the database.
    /// </summary>
    public class MapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.MapAttribute</i> object.
        /// </summary>
        /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
        public MapAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the mapping that is equivalent to the database object/field.
        /// </summary>
        public string Name { get; }
    }
}
