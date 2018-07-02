using System;
using RepoDb.Enumerations;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a class property or any <i>DataEntity</i> object properties to be marked as ignoreable
    /// during the actual repository operation.
    /// </summary>
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.IgnoreAttribute</i> object.
        /// </summary>
        /// <param name="command">The target operation command where to ignore the property.</param>
        public IgnoreAttribute(Command command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets the target operation command where the property is being ignored by the operation.
        /// </summary>
        public Command Command { get; }
    }
}