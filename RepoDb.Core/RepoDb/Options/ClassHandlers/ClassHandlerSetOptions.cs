using System.Data.Common;

namespace RepoDb.Options
{
    /// <summary>
    /// An option class that is containing the optional values when pushing the class properties values towards the database.
    /// </summary>
    public sealed class ClassHandlerSetOptions : ClassHandlerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        private ClassHandlerSetOptions() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        internal ClassHandlerSetOptions(DbCommand command)
        {
            DbCommand = command;
        }

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="DbCommand"/> object in used during the push operation.
        /// </summary>
        public DbCommand DbCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static ClassHandlerSetOptions Create(DbCommand command) =>
            new ClassHandlerSetOptions(command);

        #endregion
    }
}
