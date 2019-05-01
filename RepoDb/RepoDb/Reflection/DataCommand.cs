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
        private static ClientTypeToSqlDbTypeResolver m_clientTypeToSqlDbTypeResolver = new ClientTypeToSqlDbTypeResolver();

        #region CreateParameters<TEntity>

        /// <summary>
        /// Create a parameters for the <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="properties">The list of <see cref="ClassProperty"/> objects where the parameters will be based.</param>
        public static void CreateParameters(DbCommand command,
            IEnumerable<ClassProperty> properties)
        {
            // Set the parameters
            CreateDbCommandParametersFromProperties(command, properties);
        }

        /// <summary>
        /// Create a parameters for the <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects where the parameters will be based.</param>
        public static void CreateParameters(DbCommand command,
            IEnumerable<Field> fields)
        {
            // Set the parameters
            CreateDbCommandParametersFromFields(command, fields);
        }

        #endregion

        #region SetParameters<TEntity>

        /// <summary>
        /// Set the parameters (and/or values) of the <see cref="DbCommand"/> object based on the passed data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="command">The <see cref="DbCommand"/> object where to set the parameters.</param>
        /// <param name="entity">The data entity object where the properties (and/or values) will be retrieved.</param>
        /// <param name="actualProperties">The list of the <see cref="ClassProperty"/> objects to be retrived from the data entity object.</param>
        public static void SetParameters<TEntity>(DbCommand command,
            TEntity entity,
            IEnumerable<ClassProperty> actualProperties)
            where TEntity : class
        {
            // Get the actual function
            var func = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(command,
                entity,
                actualProperties);

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
            var func = FunctionCache.GetDataCommandParameterSetterFunction(command,
                tableName,
                fields);

            // Execute the function
            func(command, entity);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the list of <see cref="ClassProperty"/> objects based on the actual list of <see cref="DbField"/> objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <returns>The actual list of <see cref="ClassProperty"/> objects of the table.</returns>
        private static IEnumerable<ClassProperty> GetDataEntityActualProperties<TEntity>(IDbConnection connection)
            where TEntity : class
        {
            // TODO: Add a caching here

            // Get the properties first
            var properties = PropertyCache.Get<TEntity>();
            if (properties?.Any() != true)
            {
                return null;
            }

            // Get all the fields from the database
            var dbFields = (IEnumerable<DbField>)null;

            // Check the connection first
            if (connection != null)
            {
                dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>());
            }

            // Return the filtered one
            return dbFields?.Any() == true ?
                properties.Where(property => dbFields.FirstOrDefault(
                        df => df.UnquotedName.ToLower() == property.GetUnquotedMappedName().ToLower()) != null) :
                properties;
        }

        /// <summary>
        /// Get the list of <see cref="Field"/> objects based on the actual list of <see cref="DbField"/> objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects from the request.</param>
        /// <returns>The actual list of <see cref="Field"/> objects of the table.</returns>
        private static IEnumerable<Field> GetTableActualFields(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields)
        {
            // TODO: Add a caching here

            // Get all the fields from the database
            var dbFields = (IEnumerable<DbField>)null;

            // Check the connection first
            if (connection != null)
            {
                dbFields = DbFieldCache.Get(connection, tableName);
            }

            // Return the filtered one
            return dbFields?.Any() == true ?
                fields.Where(property => dbFields.FirstOrDefault(
                        df => df.UnquotedName.ToLower() == property.UnquotedName.ToLower()) != null) :
                fields;
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

            // Clear the parameters
            command.Parameters.Clear();

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

                // Resolve manually
                if (dbType == null)
                {
                    dbType = m_clientTypeToSqlDbTypeResolver.Resolve(property.PropertyInfo.PropertyType);
                }

                // Set the DB Type if present
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }

                // Add the parameter
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Create the <see cref="DbCommand"/> parameters based on the list of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="command">The target <see cref="DbCommand"/> object.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects.</param>
        private static void CreateDbCommandParametersFromFields(DbCommand command,
            IEnumerable<Field> fields)
        {
            // Check first the presence
            if (fields?.Any() != true)
            {
                return;
            }

            // Variables needed
            var bytesType = typeof(byte[]);

            // Clear the parameters
            command.Parameters.Clear();

            // Iterate and recreate
            foreach (var field in fields)
            {
                // Create the parameter
                var parameter = command.CreateParameter();

                // Set the property
                parameter.ParameterName = field.UnquotedName;

                // Set the DB Type
                var dbType = TypeMapper.Get(field.Type?.GetUnderlyingType())?.DbType;

                // Ensure the type mapping
                if (dbType == null)
                {
                    if (field.Type == bytesType)
                    {
                        dbType = DbType.Binary;
                    }
                }

                // Resolve manually
                if (dbType == null)
                {
                    dbType = m_clientTypeToSqlDbTypeResolver.Resolve(field.Type);
                }

                // Set the DB Type if present
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }

                // Add the parameter
                command.Parameters.Add(parameter);
            }
        }

        #endregion
    }
}
