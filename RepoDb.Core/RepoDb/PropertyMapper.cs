using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to map a class into its equivalent database column (ie: Table, View).
    /// </summary>
    public static class PropertyMapper
    {
        private static readonly ConcurrentDictionary<int, string> m_maps = new ConcurrentDictionary<int, string>();

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="columnName">The name of the database column.</param>
        public static void Add<T>(Expression<Func<T, object>> expression,
            string columnName)
            where T : class => Add(expression, columnName, false);

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Expression<Func<T, object>> expression,
            string columnName,
            bool force)
            where T : class => Add(GetProperty<T>(expression), columnName, force);

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        public static void Add<T>(string propertyName,
            string columnName)
            where T : class => Add<T>(propertyName, columnName, false);

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="columnName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(string propertyName,
            string columnName,
            bool force)
            where T : class
        {
            // Do not use 'FieldCache' or 'PropertyCache' as this is the underlying mapper
            Add(GetProperty<T>(propertyName), columnName, force);
        }

        /// <summary>
        /// Adds a mapping between a <see cref="Field"/> and the database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        public static void Add<T>(Field field,
            string columnName)
            where T : class => Add<T>(field, columnName, false);

        /// <summary>
        /// Adds a mapping between a <see cref="Field"/> and the database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Field field,
            string columnName,
            bool force)
            where T : class
        {
            // Do not use 'FieldCache' or 'PropertyCache' as this is the underlying mapper
            Add(GetProperty<T>(field.Name), columnName, force);
        }

        /// <summary>
        /// Adds a mapping between a <see cref="ClassProperty"/> and the database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        public static void Add(ClassProperty classProperty,
            string columnName) => Add(classProperty.PropertyInfo, columnName, false);

        /// <summary>
        /// Adds a mapping between a <see cref="ClassProperty"/> and the database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(ClassProperty classProperty,
            string columnName,
            bool force) => Add(classProperty?.PropertyInfo, columnName, force);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> and the database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        public static void Add(PropertyInfo propertyInfo,
            string columnName) => Add(propertyInfo, columnName, false);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> and the database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(PropertyInfo propertyInfo,
            string columnName,
            bool force)
        {
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (string)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, columnName, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"Mapping to '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, columnName);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Get<T>(Expression<Func<T, object>> expression)
            where T : class => Get(GetProperty<T>(expression));

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped name of the property.</returns>
        public static string Get<T>(string propertyName)
            where T : class => Get(GetProperty<T>(propertyName));

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped name of the property.</returns>
        public static string Get<T>(Field field)
            where T : class => Get(GetProperty<T>(field.Name));

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped name of the property.</returns>
        public static string Get(ClassProperty classProperty) =>
            Get(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the mapped name of the property.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped name of the property.</returns>
        public static string Get(PropertyInfo propertyInfo)
        {
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (string)null;

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Adds a mapping between a class property and the database column.
        /// </summary>
        /// <typeparam name="T">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<T>(Expression<Func<T, object>> expression)
            where T : class => Remove(GetProperty<T>(expression));

        /// <summary>
        /// Removes the mapping between the class property and database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        public static void Remove<T>(string propertyName)
            where T : class => Remove(GetProperty<T>(propertyName));

        /// <summary>
        /// Removes the mapping between the class property and database column.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        public static void Remove<T>(Field field)
            where T : class => Remove(GetProperty<T>(field.Name));

        /// <summary>
        /// Removes the mapping between the class property and database column.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        public static void Remove(ClassProperty classProperty) =>
            Remove(classProperty.PropertyInfo);

        /// <summary>
        /// Removes the mapping between the class property and database column.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        public static void Remove(PropertyInfo propertyInfo)
        {
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (string)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property mapped names.
        /// </summary>
        public static void Flush()
        {
            m_maps.Clear();
        }

        /// <summary>
        /// Validates the target property that is being passed.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        private static void Validate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new NullReferenceException("The target property cannot be null.");
            }
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        private static PropertyInfo GetProperty<T>(string propertyName)
            where T : class
        {
            return typeof(T)
                .GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName));
        }

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on expression.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="expression">The expression to be extracted.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        private static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
            where T : class
        {
            if (expression.Body.IsMember())
            {
                var member = expression.Body.ToMember();
                if (member.Member is PropertyInfo)
                {
                    return PropertyCache.Get<T>()
                        .FirstOrDefault(p =>
                            string.Equals(p.PropertyInfo.Name, member.Member.Name, StringComparison.OrdinalIgnoreCase))?
                        .PropertyInfo;
                }
            }
            throw new InvalidExpressionException($"Expression '{expression.ToString()}' is invalid.");
        }

        #endregion
    }
}
