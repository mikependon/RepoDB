using System;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to handle the information of the data entity child list.
    /// </summary>
    internal class DataEntityChildListData
    {
        /// <summary>
        /// The property of the data entity children list.
        /// </summary>
        public PropertyInfo ChildListProperty { get; set; }
        
        /// <summary>
        /// The type of the child data entity.
        /// </summary>
        public Type ChildListType { get; set; }
        
        /// <summary>
        /// The type of the parent data entity.
        /// </summary>
        public Type ParentDataEntityType { get; set; }
    }
}
