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
        /// <param name="fields">The list of <see cref="Field"/> objects where the parameters will be based.</param>
        public static void CreateParameters(DbCommand command,
            IEnumerable<Field> fields)
        {
            // Set the parameters
            FunctionFactory.CreateDbCommandParametersFromFields(command, fields);
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
        /// <param name="inputFields">The list of the input <see cref="Field"/> objects to be used.</param>
        /// <param name="outputFields">The list of the output <see cref="Field"/> objects to be used.</param>
        public static void SetParameters<TEntity>(DbCommand command,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> inputFields,
            IEnumerable<Field> outputFields)
            where TEntity : class
        {
            // Get the actual function
            var func = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(tableName,
                inputFields,
                outputFields,
                entities.Count());

            // Execute the function
            func(command, entities.ToList());
        }

        #endregion
    }
}
