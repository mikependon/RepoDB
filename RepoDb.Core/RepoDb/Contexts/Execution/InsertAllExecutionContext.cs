using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// An execution context class used by insert-all operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class InsertAllExecutionContext<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// The execution command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// The list of the input <see cref="DbField"/> objects to be included in the execution.
        /// </summary>
        public IReadOnlyList<DbField> InputFields { get; set; }

        /// <summary>
        /// The batch size of the execution.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// The compiled function that is used to set the <see cref="DbCommand"/> parameters.
        /// </summary>
        public Action<DbCommand, TEntity> SingleDataEntityParametersSetterFunc { get; set; }

        /// <summary>
        /// The compiled function that is used to set the <see cref="DbCommand"/> parameters.
        /// </summary>
        public Action<DbCommand, IList<TEntity>> MultipleDataEntitiesParametersSetterFunc { get; set; }

        /// <summary>
        /// The compiled expression that is used to set the property value.
        /// </summary>
        public Action<TEntity, object> IdentityPropertySetterFunc { get; set; }

        /// <summary>
        /// The list of compiled expression that is used to set the identity value.
        /// </summary>
        public IEnumerable<Action<TEntity, DbCommand>> IdentityPropertySettersFunc { get; set; }
    }
}
