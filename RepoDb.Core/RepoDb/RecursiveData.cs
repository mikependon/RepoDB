using System;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to hold the handles of the <i>DataEntity</i> recursive data.
    /// </summary>
    internal class RecursiveData
    {
        /// <summary>
        /// The property of the field.
        /// </summary>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// The type of the child <i>DataEntity</i>.
        /// </summary>
        public Type ChildDataType { get; set; }
        /// <summary>
        /// The type of the parent <i>DataEntity</i>.
        /// </summary>
        public Type ParentDataType { get; set; }
    }
}
