using RepoDb.Extensions;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <see cref="DbCommand"/> object.
    /// </summary>
    public static class DataCommand
    {
        #region CreateParameters<TEntity>

        /// <summary>
        /// Create a parameters for the <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="inputFields">The list of <see cref="DbField"/> objects where the parameters will be based.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the entities to be passed.</param>
        public static void CreateParameters(DbCommand command,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields,
            int batchSize)
        {
            // Set the parameters
            FunctionFactory.CreateDbCommandParametersFromFields(command, inputFields, outputFields, batchSize);
        }

        #endregion

        #region SetParameters<TEntity>

        /// <summary>
        /// Set the parameters (and/or values) of the <see cref="DbCommand"/> object based on the passed data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of the data entity objects.</param>
        /// <param name="inputFields">The list of the input <see cref="DbField"/> objects to be used.</param>
        /// <param name="outputFields">The list of the output <see cref="DbField"/> objects to be used.</param>
        public static void SetParameters<TEntity>(DbCommand command,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> inputFields,
            IEnumerable<DbField> outputFields)
            where TEntity : class
        {
            // Get the actual function
            var func = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(tableName,
                inputFields,
                outputFields,
                entities.Count());

            // Execute the function
            func(command, entities.AsList());
        }

        #endregion
    }
}
