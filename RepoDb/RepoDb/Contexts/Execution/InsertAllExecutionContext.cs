using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// An execution context class used by insert-all operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class InsertAllExecutionContext<TEntity> : ExecutionContext
        where TEntity : class
    {
        /// <summary>
        /// The identity class property.
        /// </summary>
        public ClassProperty Identity { get; set; }

        /// <summary>
        /// The list of the input <see cref="Field"/> objects to be included in the execution.
        /// </summary>
        public IEnumerable<Field> InputFields { get; set; }

        /// <summary>
        /// The list of the output <see cref="Field"/> objects to be included in the execution.
        /// </summary>
        public IEnumerable<Field> OutputFields { get; set; }

        /// <summary>
        /// The batch size of the execution.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// The actual compiled function.
        /// </summary>
        public Action<DbCommand, IList<TEntity>> Execute { get; set; }
    }
}
