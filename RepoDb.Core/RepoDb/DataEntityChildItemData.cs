namespace RepoDb
{
    /// <summary>
    /// A class that handles the information of the data entity child item data.
    /// </summary>
    internal class DataEntityChildItemData
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.DataEntityChildItemData</i> object.
        /// </summary>
        /// <param name="dataEntity">The instance of the data entity object.</param>
        public DataEntityChildItemData(object dataEntity)
        {
            DataEntity = dataEntity;
        }

        /// <summary>
        /// Gets or sets the primary key value of the data entity object.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// Gets the data entity object.
        /// </summary>
        public object DataEntity { get; }
    }
}
