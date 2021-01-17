using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a .NET CLR type or class property into its equivalent <see cref="DbType"/> object.
    /// </summary>
    public static class TypeMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> maps = new ConcurrentDictionary<int, DbType?>();

        #endregion

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add<TType>(DbType? dbType) =>
            Add(typeof(TType), dbType);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType>(DbType? dbType,
            bool force) =>
            Add(typeof(TType), dbType, force);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add(Type type,
            DbType? dbType) =>
            Add(type, dbType, false);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            DbType? dbType,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = GenerateHashCode(type);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Update the existing one
                    maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"Mapping to '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Type Level: Gets the mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The dynamic .NET CLR type used for mapping.</typeparam>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get<TType>() =>
            Get(typeof(TType));

        /// <summary>
        /// Type Level: Gets the mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type used for mapping.</param>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = GenerateHashCode(type);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Type Level: Removes the existing mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type where the mapping is to be removed.</typeparam>
        public static void Remove<TType>() =>
            Remove(typeof(TType));

        /// <summary>
        /// Type Level: Removes the existing mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type where the mapping is to be removed.</param>
        public static void Remove(Type type)
        {
            var key = type.GetHashCode();
            var value = (DbType?)null;

            // Try get the value
            maps.TryRemove(key, out value);
        }

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(expression, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            DbType? dbType,
            bool force)
            where TEntity : class =>
            Add<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression), dbType, force);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(string propertyName,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(propertyName, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string propertyName,
            DbType? dbType,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TEntity>(property, dbType, force);
        }

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(Field field,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(field, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            DbType? dbType,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TEntity>(property, dbType, force);
        }

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        internal static void Add<TEntity>(PropertyInfo propertyInfo,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(propertyInfo, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add<TEntity>(PropertyInfo propertyInfo,
            DbType? dbType,
            bool force)
            where TEntity : class =>
            Add(typeof(TEntity), propertyInfo, dbType, force);

        /// <summary>
        /// Property Level: Adds a mapping between a data entity type property and a <see cref="DbType"/> object (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add(Type entityType,
            PropertyInfo propertyInfo,
            DbType? dbType,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");
            ValidateTargetColumnName(dbType);

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Update the existing one
                    maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"A database type mapping to '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Property Level: Gets the mapped <see cref="DbType"/> object of the data entity type property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the mapped <see cref="DbType"/> object of the data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the mapped <see cref="DbType"/> object of the data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Field field)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the mapped <see cref="DbType"/> object of the data entity type property via <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        internal static DbType? Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Property Level: Removes the mapped <see cref="DbType"/> object of the data entity type property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Remove(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Removes the mapped <see cref="DbType"/> object of the data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        public static void Remove<TEntity>(string propertyName)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Removes the mapped <see cref="DbType"/> object of the data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        public static void Remove<TEntity>(Field field)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Removes the mapped <see cref="DbType"/> object of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        internal static void Remove(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);
            var value = (DbType?)null;

            // Try get the value
            maps.TryRemove(key, out value);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached database types.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo) =>
            TypeExtension.GenerateHashCode(entityType, propertyInfo);

        /// <summary>
        /// Validates the value of the target column name.
        /// </summary>
        /// <param name="dbType">The column name to be validated.</param>
        private static void ValidateTargetColumnName(DbType? dbType)
        {
            if (dbType == null)
            {
                throw new NullReferenceException("The target column name cannot be null or empty.");
            }
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion
    }
}
