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
        : this(name, CommandType.Text)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.MapAttribute</i> object.
        /// </summary>
        /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
        /// <param name="commandType">
        /// The command type to be used for this mapping (whether Text, TableDirect or StoredProcedure). This value
        /// is only working if the attribute is implemented at the class level.
        /// </param>
        public MapAttribute(string name, CommandType commandType)
        {
            Name = name;
            CommandType = commandType;
        }

        /// <summary>
        /// Gets the name of the mapping that is equivalent to the database object/field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the command type used by the operation for this mapping.
        /// </summary>
        public CommandType CommandType { get; }
    }
}
