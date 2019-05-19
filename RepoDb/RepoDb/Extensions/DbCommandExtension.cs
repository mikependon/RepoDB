using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbCommand"/> object.
    /// </summary>
    public static class DbCommandExtension
    {
        #region Privates

        private static Type m_bytesType = typeof(byte[]);
        private static Type m_dictionaryType = typeof(Dictionary<,>);

        #endregion

        #region CreateParameters

        /// <summary>
        /// Creates the command object list of parameters based on type.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="properties">The target list of properties.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        internal static void CreateParametersFromClassProperties(this IDbCommand command,
            IEnumerable<ClassProperty> properties,
            IEnumerable<string> propertiesToSkip)
        {
            // Filter the target properties
            if (propertiesToSkip?.Any() == true)
            {
                properties = properties?.Where(p =>
                    propertiesToSkip.Contains(PropertyMappedNameCache.Get(p.PropertyInfo, false), StringComparer.CurrentCultureIgnoreCase) == false);
            }

            // Check if there are properties
            if (properties?.Any() == true)
            {
                // Iterate the properties
                foreach (var property in properties)
                {
                    // Get the database type
                    var dbType = property.GetDbType() ??
                        TypeMapper.Get(property.PropertyInfo.PropertyType.GetUnderlyingType());

                    // Ensure the type mapping
                    if (dbType == null)
                    {
                        if (property.PropertyInfo.PropertyType == m_bytesType)
                        {
                            dbType = DbType.Binary;
                        }
                    }

                    // Create the parameter
                    var parameter = CreateParameter(command, PropertyMappedNameCache.Get(property.PropertyInfo, false), null, dbType);

                    // Add the parameter
                    command.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// Creates a parameter for a command object.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The database type of the parameter.</param>
        /// <returns>An instance of the newly created parameter object.</returns>
        public static IDbDataParameter CreateParameter(this IDbCommand command,
            string name,
            object value,
            DbType? dbType = null)
        {
            // Create the parameter
            var parameter = command.CreateParameter();

            // Set the values
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;

            // The DB Type is auto set when setting the values (so check properly Time/DateTime problem)
            if (dbType != null && parameter.DbType != dbType.Value)
            {
                parameter.DbType = dbType.Value;
            }

            // Return the parameter
            return parameter;
        }

        /// <summary>
        /// Creates a parameter for a command object.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="commandArrayParameters">The list of <see cref="CommandArrayParameter"/> to be used for replacement.</param>
        internal static void CreateParametersFromArray(this IDbCommand command,
            IEnumerable<CommandArrayParameter> commandArrayParameters)
        {
            if (commandArrayParameters == null)
            {
                return;
            }
            for (var i = 0; i < commandArrayParameters.Count(); i++)
            {
                var commandArrayParameter = commandArrayParameters.ElementAt(i);
                for (var c = 0; c < commandArrayParameter.Values.Count(); c++)
                {
                    var name = string.Concat(commandArrayParameter.ParameterName, c).AsParameter();
                    var value = commandArrayParameter.Values.ElementAt(c);
                    command.Parameters.Add(command.CreateParameter(name, value));
                }
            }
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        public static void CreateParameters(this IDbCommand command,
            object param)
        {
            CreateParameters(command, param, null);
        }

        /// <summary>
        /// Creates a parameter from object by mapping the property from the target entity type.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The object to be used when creating the parameters.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        public static void CreateParameters(this IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip)
        {
            // Check for presence
            if (param == null)
            {
                return;
            }

            // Supporting the IDictionary<string, object>
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                CreateParameters(command, (IDictionary<string, object>)param, propertiesToSkip);
            }

            // Supporting the QueryField
            else if (param is QueryField)
            {
                CreateParameters(command, (QueryField)param, propertiesToSkip);
            }

            // Supporting the IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                CreateParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip);
            }

            // Supporting the QueryGroup
            else if (param is QueryGroup)
            {
                CreateParameters(command, (QueryGroup)param, propertiesToSkip);
            }

            // Otherwise, iterate the properties
            else
            {
                var type = param.GetType();

                // Check the validity of the type
                if (type.IsGenericType && type.GetGenericTypeDefinition() == m_dictionaryType)
                {
                    throw new InvalidOperationException("Invalid parameters passed. The supported type of dictionary object must be typeof(IDictionary<string, object>).");
                }

                // Variables for properties
                var properties = (IEnumerable<ClassProperty>)null;

                // Add this check for performance
                if (propertiesToSkip == null)
                {
                    properties = PropertyCache.Get(type);
                }
                else
                {
                    properties = PropertyCache.Get(type)
                        .Where(p => propertiesToSkip?.Contains(p.PropertyInfo.Name,
                            StringComparer.CurrentCultureIgnoreCase) == false);
                }

                // Iterate the properties
                foreach (var property in properties)
                {
                    // Get the property vaues
                    var name = property.GetUnquotedMappedName();
                    var value = property.PropertyInfo.GetValue(param);
                    var dbType = property.GetDbType() ??
                        TypeMapper.Get(property.PropertyInfo.PropertyType.GetUnderlyingType() ??
                            value?.GetType().GetUnderlyingType());

                    // Ensure the type mapping
                    if (dbType == null)
                    {
                        if (value == null && property.PropertyInfo.PropertyType == m_bytesType)
                        {
                            dbType = DbType.Binary;
                        }
                    }

                    // Add the new parameter
                    command.Parameters.Add(command.CreateParameter(name, value, dbType));
                }
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="IDictionary{TKey, TValue}"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip)
        {
            // Variables needed
            var dbType = (DbType?)null;
            var kvps = dictionary.Where(kvp => propertiesToSkip?.Contains(kvp.Key, StringComparer.CurrentCultureIgnoreCase) != true);

            // Iterate the key value pairs
            foreach (var kvp in kvps)
            {
                var value = kvp.Value;

                // Cast the proper object and identify the properties
                if (kvp.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)kvp.Value;
                    var property = commandParameter.MappedToType.GetProperty(kvp.Key);

                    // Get the value
                    value = commandParameter.Value;

                    // Get the DB Type
                    dbType = property?.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(property?.PropertyType?.GetUnderlyingType() ??
                            kvp.Value?.GetType()?.GetUnderlyingType());
                }
                else
                {
                    dbType = TypeMapper.Get(kvp.Value?.GetType()?.GetUnderlyingType());
                }

                // Add the parameter
                command.Parameters.Add(command.CreateParameter(kvp.Key, value, dbType));
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryGroup">The value of the <see cref="QueryGroup"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip)
        {
            // Call the overloaded methods for the query fields
            CreateParameters(command, queryGroup?.GetFields(true), propertiesToSkip);
        }

        /// <summary>
        /// Create the command parameters from the list of <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> propertiesToSkip)
        {
            // Filter the query fields
            var filteredQueryFields = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.UnquotedName, StringComparer.CurrentCultureIgnoreCase) != true);

            // Iterate the filtered query fields
            foreach (var queryField in filteredQueryFields)
            {
                CreateParameters(command, queryField, null);
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        private static void CreateParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip)
        {
            // Exclude those to be skipped
            if (propertiesToSkip?.Contains(queryField.Field.UnquotedName, StringComparer.CurrentCultureIgnoreCase) == true)
            {
                return;
            }

            // Validate, make sure to only have the proper operation
            if (queryField.Operation != Operation.Equal)
            {
                throw new InvalidOperationException($"Operation must only be '{nameof(Operation.Equal)}' when calling the 'Execute' methods.");
            }

            // Get the values
            var value = queryField.Parameter.Value;
            var dbType = TypeMapper.Get(value?.GetType()?.GetUnderlyingType());

            // Create the parameter
            command.Parameters.Add(command.CreateParameter(queryField.Parameter.Name, value, dbType));
        }

        #endregion

        #region SetParameters

        /// <summary>
        /// Set the <see cref="IDbCommand"/> exist <see cref="IDbDataParameter"/> object value and type.
        /// </summary>
        /// <param name="command">The command object instance to be used.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The database type of the parameter.</param>
        /// <returns>The instance of updated parameter.</returns>
        public static IDbDataParameter SetParameter(this IDbCommand command,
            string name,
            object value,
            DbType? dbType = null)
        {
            // Check the presence
            if (command.Parameters.Contains(name) == false)
            {
                throw new ParameterNotFoundException($"Parameter '{name}' is not found.");
            }

            // Get the parameter
            var parameter = (DbParameter)command.Parameters[name];

            // Set the properties
            parameter.Value = value ?? DBNull.Value;
            if (dbType != null)
            {
                parameter.DbType = dbType.Value;
            }

            // Return the parameter
            return parameter;
        }
        /// <summary>
        /// Set the <see cref="IDbCommand"/> object existing <see cref="IDbDataParameter"/> values.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="param">The instance of where the parameter values will be set.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        /// <param name="resetOthers">True to reset the other parameter object. This will ignore the skipped properties.</param>
        internal static void SetParameters(this IDbCommand command,
            object param,
            IEnumerable<string> propertiesToSkip = null,
            bool resetOthers = true)
        {
            // Do nothing if there is no parameter
            if (command.Parameters.Count == 0)
            {
                return;
            }

            // Check for presence
            if (param == null)
            {
                foreach (var parameter in command.Parameters.OfType<DbParameter>())
                {
                    parameter.Value = DBNull.Value;
                    parameter.ResetDbType();
                }
            }

            // Supporting the IDictionary<string, object>
            else if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                SetParameters(command, (IDictionary<string, object>)param, propertiesToSkip, resetOthers);
            }

            // Supporting the QueryField
            else if (param is QueryField)
            {
                SetParameters(command, (QueryField)param, propertiesToSkip, resetOthers);
            }

            // Supporting the IEnumerable<QueryField>
            else if (param is IEnumerable<QueryField>)
            {
                SetParameters(command, (IEnumerable<QueryField>)param, propertiesToSkip, resetOthers);
            }

            // Supporting the QueryGroup
            else if (param is QueryGroup)
            {
                SetParameters(command, (QueryGroup)param, propertiesToSkip, resetOthers);
            }

            // Otherwise, iterate the properties
            else
            {
                var type = param.GetType();

                // Check the validity of the type
                if (type.IsGenericType && type.GetGenericTypeDefinition() == m_dictionaryType)
                {
                    throw new InvalidOperationException("Invalid parameters passed. The supported type of dictionary object must be typeof(IDictionary<string, object>).");
                }

                // variables for properties
                var properties = (IEnumerable<ClassProperty>)null;

                // Add this check for performance
                if (propertiesToSkip == null)
                {
                    properties = PropertyCache.Get(type);
                }
                else
                {
                    properties = PropertyCache.Get(type)
                        .Where(p => propertiesToSkip?.Contains(p.PropertyInfo.Name,
                            StringComparer.CurrentCultureIgnoreCase) == false);
                }

                // Ensure there are properties
                if (resetOthers == true && properties.Any() != true)
                {
                    SetParameters(command, null);
                }
                else
                {
                    var parameters = command.Parameters.OfType<DbParameter>();
                    var missingParameters = (IList<DbParameter>)null;

                    // Iterate the parameter instead
                    foreach (var parameter in parameters)
                    {
                        var property = properties.FirstOrDefault(p => p.GetUnquotedMappedName().ToLower() == parameter.ParameterName.ToLower());

                        // Skip if null
                        if (property == null)
                        {
                            // Add to missing properties if allowed to
                            if (resetOthers == true)
                            {
                                if (missingParameters == null)
                                {
                                    missingParameters = new List<DbParameter>();
                                }
                                missingParameters.Add(parameter);
                            }

                            // Continue to next
                            continue;
                        }

                        // Get the property values
                        var value = property.PropertyInfo.GetValue(param);
                        var dbType = property.GetDbType() ??
                            TypeMapper.Get(property.PropertyInfo.PropertyType.GetUnderlyingType());

                        // Ensure the type mapping
                        if (dbType == null)
                        {
                            if (value == null && property.PropertyInfo.PropertyType == m_bytesType)
                            {
                                dbType = DbType.Binary;
                            }
                        }

                        // Set the parameter
                        parameter.Value = value;
                        if (dbType != null)
                        {
                            parameter.DbType = dbType.Value;
                        }
                    }

                    // If there is any (then set to null)
                    if (resetOthers == true && missingParameters?.Any() == true)
                    {
                        foreach (var parameter in missingParameters)
                        {
                            parameter.Value = DBNull.Value;
                            parameter.ResetDbType();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="IDictionary{TKey, TValue}"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        /// <param name="resetOthers">True to reset the other parameter object. This will ignore the skipped properties.</param>
        private static void SetParameters(this IDbCommand command,
            IDictionary<string, object> dictionary,
            IEnumerable<string> propertiesToSkip,
            bool resetOthers = true)
        {
            // Variables needed
            var dbType = (DbType?)null;
            var others = (IList<DbParameter>)null;
            var fitered = dictionary
                .Where(kvp => propertiesToSkip?.Contains(kvp.Key, StringComparer.CurrentCultureIgnoreCase) != true);

            // Iterate the parameter instead
            foreach (var parameter in command.Parameters.OfType<DbParameter>())
            {
                var kvp = fitered.FirstOrDefault(item => item.Key.ToLower() == parameter.ParameterName.ToLower());

                // Skip and add to missing if null
                if (ReferenceEquals(null, kvp))
                {
                    // Add to missing properties if allowed to
                    if (resetOthers == true)
                    {
                        if (others == null)
                        {
                            others = new List<DbParameter>();
                        }
                        others.Add(parameter);
                    }

                    // Continue to next
                    continue;
                }

                // Get the value
                var value = kvp.Value;

                // Cast the proper object and identify the properties
                if (kvp.Value is CommandParameter)
                {
                    var commandParameter = (CommandParameter)kvp.Value;
                    var property = commandParameter.MappedToType.GetProperty(kvp.Key);

                    // Get the value
                    value = commandParameter.Value;

                    // Get the DB Type
                    dbType = property?.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                        TypeMapper.Get(property?.PropertyType.GetUnderlyingType());
                }
                else
                {
                    // Get the DB Type
                    dbType = TypeMapper.Get(kvp.Value?.GetType()?.GetUnderlyingType());
                }

                // Set the parameter
                parameter.Value = value;
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }
                else
                {
                    parameter.ResetDbType();
                }
            }

            // If there is any (then set to null)
            if (resetOthers == true && others?.Any() == true)
            {
                foreach (var parameter in others)
                {
                    parameter.Value = DBNull.Value;
                    parameter.ResetDbType();
                }
            }
        }

        /// <summary>
        /// Create the command parameters from the <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryGroup">The value of the <see cref="QueryGroup"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        /// <param name="resetOthers">True to reset the other parameter object. This will ignore the skipped properties.</param>
        private static void SetParameters(this IDbCommand command,
            QueryGroup queryGroup,
            IEnumerable<string> propertiesToSkip,
            bool resetOthers = true)
        {
            // Call the overloaded methods for the query fields
            SetParameters(command, queryGroup?.GetFields(true), propertiesToSkip, resetOthers);
        }

        /// <summary>
        /// Create the command parameters from the list of <see cref="QueryField"/> objects.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        /// <param name="resetOthers">True to reset the other parameter object. This will ignore the skipped properties.</param>
        private static void SetParameters(this IDbCommand command,
            IEnumerable<QueryField> queryFields,
            IEnumerable<string> propertiesToSkip,
            bool resetOthers = true)
        {
            // Variables needed
            var dbType = (DbType?)null;
            var others = (IList<DbParameter>)null;
            var filtered = queryFields
                .Where(qf => propertiesToSkip?.Contains(qf.Field.UnquotedName, StringComparer.CurrentCultureIgnoreCase) != true);

            // Iterate the parameter instead
            foreach (var parameter in command.Parameters.OfType<DbParameter>())
            {
                var queryField = filtered.FirstOrDefault(qf => qf.Field.UnquotedName.ToLower() == parameter.ParameterName.ToLower());

                // Skip and add to missing if null
                if (queryField == null)
                {
                    // Add to missing properties if allowed to
                    if (resetOthers == true)
                    {
                        if (others == null)
                        {
                            others = new List<DbParameter>();
                        }
                        others.Add(parameter);
                    }

                    // Continue to next
                    continue;
                }

                // Get the value
                var value = queryField.Parameter.Value;

                // Get the DB Type
                dbType = TypeMapper.Get(value?.GetType()?.GetUnderlyingType());

                // Set the parameter
                parameter.Value = value;
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }
                else
                {
                    parameter.ResetDbType();
                }
            }

            // If there is any (then set to null)
            if (resetOthers == true && others?.Any() == true)
            {
                foreach (var parameter in others)
                {
                    parameter.Value = DBNull.Value;
                    parameter.ResetDbType();
                }
            }
        }

        /// <summary>
        /// Creates a command parameter from the <see cref="QueryField"/> object.
        /// </summary>
        /// <param name="command">The command object to be used.</param>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="propertiesToSkip">The list of the properties to be skpped.</param>
        /// <param name="resetOthers">True to reset the other parameter object. This will ignore the skipped properties.</param>
        private static void SetParameters(this IDbCommand command,
            QueryField queryField,
            IEnumerable<string> propertiesToSkip,
            bool resetOthers = true)
        {
            // Exclude those to be skipped
            if (propertiesToSkip?.Contains(queryField.Field.UnquotedName, StringComparer.CurrentCultureIgnoreCase) == true)
            {
                return;
            }

            // Validate, make sure to only have the proper operation
            if (queryField.Operation != Operation.Equal)
            {
                throw new InvalidOperationException($"Operation must only be '{nameof(Operation.Equal)}' when calling the 'Execute' methods.");
            }

            // Get the values
            var parameters = command.Parameters.OfType<DbParameter>();
            var parameter = parameters.FirstOrDefault(p => p.ParameterName.ToLower() == queryField.Field.UnquotedName.ToLower());

            // Get the target parameter
            if (parameter != null)
            {
                var value = queryField.Parameter.Value;
                var dbType = TypeMapper.Get(value?.GetType()?.GetUnderlyingType());

                // Set the value
                parameter.Value = value;

                // Set the DB Type
                if (dbType != null)
                {
                    parameter.DbType = dbType.Value;
                }
            }

            // If there is any (then set to null)
            if (resetOthers == true)
            {
                var others = parameters.Where(p => p != parameter);
                if (others?.Any() == true)
                {
                    foreach (var p in others)
                    {
                        p.Value = DBNull.Value;
                        p.ResetDbType();
                    }
                }
            }
        }

        #endregion
    }
}
