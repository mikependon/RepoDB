using System.Collections.Generic;
using System.Data.Common;

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
        /// <param name="entity">The data entity object where the properties (and/or values) will be retrieved.</param>
        /// <param name="fields">The list of the <see cref="ClassProperty"/> objects to be used.</param>
        public static void SetParameters<TEntity>(DbCommand command,
            TEntity entity,
            IEnumerable<Field> fields)
            where TEntity : class
        {
            // Get the actual function
            var func = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(fields);

            // Execute the function
            func(command, entity);
        }

        #endregion

        #region SetParameters(TableName)

        /// <summary>
        /// Set the parameters (and/or values) of the <see cref="DbCommand"/> object based on the passed dynamic object.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entity">The dynamic object where the properties (and/or values) will be retrieved.</param>
        /// <param name="fields">The list of the <see cref="Field"/> objects to be retrived from the dynamic object.</param>
        public static void SetParameters(DbCommand command,
            string tableName,
            object entity,
            IEnumerable<Field> fields)
        {
            // Get the actual function
            var func = FunctionCache.GetDataCommandParameterSetterFunction(tableName,
                fields);

            // Execute the function
            func(command, entity);
        }

        #endregion
    }
}
