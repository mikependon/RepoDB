using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// An execution context class used by insert-all operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class InsertAllCommandExecutionContext<TEntity> : CommandExecutionContext
        where TEntity : class
    {
        /// <summary>
        /// The identity class property.
        /// </summary>
        public ClassProperty Identity { get; set; }

        /// <summary>
        /// The list of <see cref="Field"/> objects to be included in the execution.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// The actual compiled function.
        /// </summary>
        public Action<DbCommand, TEntity> Executor { get; set; }
    }
}
