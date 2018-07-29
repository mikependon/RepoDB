using System;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to handle the information of the <i>DataEntity</i> child list.
    /// </summary>
    internal class DataEntityChildListData
    {
        /// <summary>
        /// The property of the <i>DataEntity</i> children list.
        /// </summary>
        public PropertyInfo ChildListProperty { get; set; }
        
        /// <summary>
        /// The type of the child <i>DataEntity</i>.
        /// </summary>
        public Type ChildListType { get; set; }
        
        /// <summary>
        /// The type of the parent <i>DataEntity</i>.
        /// </summary>
        public Type ParentDataEntityType { get; set; }
    }
}
