using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that handles the information of the <i>DataEntity</i> child item data.
    /// </summary>
    internal class DataEntityChildItemData
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.DataEntityChildItemData</i> object.
        /// </summary>
        /// <param name="key">The value of the primary key of the <i>DataEntity</i> object.</param>
        /// <param name="dataEntity">The instance of the <i>DataEntity</i> object.</param>
        public DataEntityChildItemData(object key, object dataEntity)
        {
            Key = key;
            DataEntity = dataEntity;
        }

        /// <summary>
        /// Gets the primary key value of the <i>DataEntity</i> object.
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// Gets the <i>DataEntity</i> object.
        /// </summary>
        public object DataEntity { get; }

        /// <summary>
        /// Gets or sets the list object to be used.
        /// </summary>
        public object List { get; set; }

        /// <summary>
        /// Gets or sets the <i>MethodInfo</i> object used that is pointed to the current list.
        /// </summary>
        public MethodInfo AddMethod { get; set; }
    }
}
