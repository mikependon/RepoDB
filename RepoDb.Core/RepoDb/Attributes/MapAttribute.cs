using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of the class/property into its equivalent object/field name in the database.
    /// </summary>
    public class MapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapAttribute"/> class.
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
