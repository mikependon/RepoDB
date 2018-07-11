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
        /// <param name="dataEntity">The instance of the <i>DataEntity</i> object.</param>
        public DataEntityChildItemData(object dataEntity)
        {
            DataEntity = dataEntity;
        }

        /// <summary>
        /// Gets or sets the primary key value of the <i>DataEntity</i> object.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// Gets the <i>DataEntity</i> object.
        /// </summary>
        public object DataEntity { get; }
    }
}
