using RepoDb.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <see cref="DbCommand"/> object.
    /// </summary>
    public static class DataCommand
    {
        /// <summary>
        /// Set the parameters (and/or values) of the command object based on the passed data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="entity">The data entity object where the properties (and/or values) will be retrieved.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <param name="recreate">True to clear and re-create the parameters.</param>
        public static void SetParameters<TEntity>(DbCommand command,
            TEntity entity,
            IDbConnection connection,
            bool recreate = false)
            where TEntity : class
        {
            // Get the actual fields
            var actualProperties = GetDataEntityActualProperties(connection, ClassMappedNameCache.Get<TEntity>(),
                PropertyCache.Get<TEntity>());

            // Check if we need to recreate
            if (recreate == true)
            {
                // Clear the parameters
                command.Parameters.Clear();

                // Set the parameters
                CreateDbCommandParametersFromProperties(command, actualProperties);
            }

            // Return if there are no parameters
            if (command.Parameters.Count == 0)
            {
                return;
            }

            // Get the actual function
            var func = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(command,
                entity,
                actualProperties,
                connection);

            // Execute the function
            func(command, entity);
        }

        /// <summary>
        /// Get the list of <see cref="ClassProperty"/> objects based on the actual list of <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target name of the table.</param>
        /// <param name="properties">The list of <see cref="ClassProperty"/> objects from the data entity object.</param>
        /// <returns>The actual list of <see cref="ClassProperty"/> objects of the table.</returns>
        private static IEnumerable<ClassProperty> GetDataEntityActualProperties(IDbConnection connection,
            string tableName,
            IEnumerable<ClassProperty> properties)
        {
            if (properties?.Any() != true)
            {
                return null;
            }

            // Get all the fields from the database
            var dbFields = (IEnumerable<DbField>)null;

            // Check the connection first
            if (connection != null)
            {
                dbFields = DbFieldCache.Get(connection, tableName);
            }

            // Return the filtered one
            return dbFields?.Any() == true ?
                properties.Where(property => dbFields.FirstOrDefault(
                        df => df.UnquotedName.ToLower() == property.GetUnquotedMappedName().ToLower()) != null) :
                properties;
        }

        /// <summary>
        /// Create the <see cref="DbCommand"/> parameters based on the list of <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="command">The target <see cref="DbCommand"/> object.</param>
        /// <param name="properties">The list of <see cref="ClassProperty"/> objects.</param>
        private static void CreateDbCommandParametersFromProperties(DbCommand command,
            IEnumerable<ClassProperty> properties)
        {
            // Check first the presence
            if (properties?.Any() != true)
            {
                return;
            }

            // Variables needed
            var bytesType = typeof(byte[]);

            // Iterate and recreate
            foreach (var property in properties)
            {
                // Create the parameter
                var parameter = command.CreateParameter();

                // Set the property
                parameter.ParameterName = property.GetUnquotedMappedName();

                // Set the DB Type
                var dbType = property.GetDbType() ??
                    TypeMapper.Get(property.PropertyInfo.PropertyType.GetUnderlyingType())?.DbType;

                // Ensure the type mapping
                if (dbType == null)
                {
                    if (property.PropertyInfo.PropertyType == bytesType)
                    {
                        dbType = DbType.Binary;
                    }
                }
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }

                // Add the parameter
                command.Parameters.Add(parameter);
            }
        }
    }
}
