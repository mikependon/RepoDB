#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Invokes the <c>BulkInsert</c> (and <c>BulkInsertAsync</c>) extension method that the <c>BulkOperations</c> package of the destination provider defines for its connection,
    /// so a data reader can be fed to it. The extension methods are defined for the connection type of each provider (with different options), so they are found by their shape
    /// (<c>BulkInsert(connection, string tableName, reader, ...)</c>) and the optional arguments are given by their names.
    /// </summary>
    internal static class BulkInsertInvoker
    {
        #region Private Variables

        private const string BulkOperationsSuffix = ".BulkOperations";
        private const string ColumnMappingsBehaviorProperty = "BulkColumnMappingsBehavior";
        private const string KeepIdentityOption = "KeepIdentity";
        private static readonly ConcurrentDictionary<(Type Connection, Type Reader, bool IsAsync), MethodInfo> _methods =
            new ConcurrentDictionary<(Type, Type, bool), MethodInfo>();

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether the <c>BulkOperations</c> package of the provider of the connection is available for the reader.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="reader"></param>
        /// <param name="isAsync"></param>
        /// <returns><c>true</c> if the bulk insert is available; otherwise, <c>false</c>.</returns>
        internal static bool IsAvailable(IDbConnection connection,
            IDataReader reader,
            bool isAsync) =>
            Find(connection, reader, isAsync) != null;

        /// <summary>
        /// Bulk inserts the rows of the reader into the table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="batchSize"></param>
        /// <param name="columnMappingsBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows that were inserted.</returns>
        internal static int BulkInsert(IDbConnection connection,
            string tableName,
            IDataReader reader,
            int batchSize,
            CopyTableSchemaColumnMappingsBehavior columnMappingsBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            var method = Find(connection, reader, false) ?? throw CreateMissingException(connection);
            using (ColumnMappingsScope.Enter(connection, columnMappingsBehavior))
            {
                try
                {
                    return Convert.ToInt32(method.Invoke(null, CreateArguments(method, connection, tableName, reader, batchSize, commandTimeout, traceKey, trace, transaction, CancellationToken.None)), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }
            }
        }

        /// <summary>
        /// Bulk inserts the rows of the reader into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="batchSize"></param>
        /// <param name="columnMappingsBehavior"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows that were inserted.</returns>
        internal static async Task<int> BulkInsertAsync(IDbConnection connection,
            string tableName,
            IDataReader reader,
            int batchSize,
            CopyTableSchemaColumnMappingsBehavior columnMappingsBehavior,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var method = Find(connection, reader, true) ?? throw CreateMissingException(connection);
            using (await ColumnMappingsScope.EnterAsync(connection, columnMappingsBehavior, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var task = (Task)method.Invoke(null, CreateArguments(method, connection, tableName, reader, batchSize, commandTimeout, traceKey, trace, transaction, cancellationToken));
                    await task.ConfigureAwait(false);
                    return Convert.ToInt32(task.GetType().GetProperty(nameof(Task<int>.Result)).GetValue(task), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }
            }
        }

        #endregion

        #region Column Mappings

        /// <summary>
        /// Applies the column mappings behavior to the bulk insert for as long as the scope lives. The bulk operations package of a provider reads the behavior
        /// from the setting of the connection (the <c>BulkColumnMappingsBehavior</c> property of its <c>&lt;Provider&gt;BulkOperationsDbSetting</c>, of the enumeration
        /// <c>&lt;Provider&gt;BulkColumnMappingsBehavior</c> that has the same names and values as <see cref="CopyTableSchemaColumnMappingsBehavior"/>), so the behavior is set on the
        /// setting and then set back. The setting is shared by every connection of the same type, so the bulk inserts of the setting take turns while the behavior is applied.
        /// </summary>
        private sealed class ColumnMappingsScope : IDisposable
        {
            private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, SemaphoreSlim> _gates =
                new System.Runtime.CompilerServices.ConditionalWeakTable<object, SemaphoreSlim>();

            private readonly object _setting;
            private readonly PropertyInfo _property;
            private readonly SemaphoreSlim _gate;
            private object _previous;
            private bool _applied;

            private ColumnMappingsScope(object setting,
                PropertyInfo property,
                SemaphoreSlim gate)
            {
                _setting = setting;
                _property = property;
                _gate = gate;
            }

            internal static ColumnMappingsScope Enter(IDbConnection connection,
                CopyTableSchemaColumnMappingsBehavior behavior)
            {
                var scope = Create(connection, behavior);
                if (scope?._gate == null)
                {
                    return scope;
                }
                scope._gate.Wait();
                return scope.Apply(behavior);
            }

            internal static async Task<ColumnMappingsScope> EnterAsync(IDbConnection connection,
                CopyTableSchemaColumnMappingsBehavior behavior,
                CancellationToken cancellationToken)
            {
                var scope = Create(connection, behavior);
                if (scope?._gate == null)
                {
                    return scope;
                }
                await scope._gate.WaitAsync(cancellationToken).ConfigureAwait(false);
                return scope.Apply(behavior);
            }

            /// <summary>
            /// Finds the property of the setting of the connection that defines the behavior (<c>null</c> scope if the setting has none and the behavior is the default one).
            /// </summary>
            private static ColumnMappingsScope Create(IDbConnection connection,
                CopyTableSchemaColumnMappingsBehavior behavior)
            {
                var setting = DbSettingMapper.Get(connection);
                var property = setting?.GetType().GetProperty(ColumnMappingsBehaviorProperty, BindingFlags.Public | BindingFlags.Instance);
                if (property == null || !property.CanRead || !property.CanWrite || !property.PropertyType.IsEnum)
                {
                    if (behavior == CopyTableSchemaColumnMappingsBehavior.Automatic)
                    {
                        return new ColumnMappingsScope(null, null, null);
                    }
                    throw new InvalidOperationException($"The column mappings behavior '{behavior}' requires the setting of the bulk operations package of the provider " +
                        $"(a setting with the '{ColumnMappingsBehaviorProperty}' property) to be mapped to the connection '{connection.GetType().FullName}'.");
                }
                if (!Enum.IsDefined(property.PropertyType, behavior.ToString()))
                {
                    throw new InvalidOperationException($"The column mappings behavior '{behavior}' is not defined by '{property.PropertyType.FullName}'.");
                }
                return new ColumnMappingsScope(setting, property, _gates.GetValue(setting, _ => new SemaphoreSlim(1, 1)));
            }

            private ColumnMappingsScope Apply(CopyTableSchemaColumnMappingsBehavior behavior)
            {
                try
                {
                    _previous = _property.GetValue(_setting);
                    _property.SetValue(_setting, Enum.Parse(_property.PropertyType, behavior.ToString()));
                    _applied = true;
                }
                catch
                {
                    _gate.Release();
                    throw;
                }
                return this;
            }

            public void Dispose()
            {
                if (_applied)
                {
                    _property.SetValue(_setting, _previous);
                    _applied = false;
                    _gate.Release();
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Finds the extension method that bulk inserts the rows of the reader into the connection (the result is cached).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="reader"></param>
        /// <param name="isAsync"></param>
        /// <returns>The method, or <c>null</c> if there is none.</returns>
        private static MethodInfo Find(IDbConnection connection,
            IDataReader reader,
            bool isAsync)
        {
            var key = (connection.GetType(), reader.GetType(), isAsync);
            if (_methods.TryGetValue(key, out var method))
            {
                return method;
            }
            return _methods.GetOrAdd(key, _ => Search(key.Item1, key.Item2, isAsync));
        }

        /// <summary>
        /// Searches the <c>BulkOperations</c> assemblies for the method.
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="readerType"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        private static MethodInfo Search(Type connectionType,
            Type readerType,
            bool isAsync)
        {
            var name = isAsync ? "BulkInsertAsync" : "BulkInsert";
            return GetBulkOperationsTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(method => method.Name == name &&
                    !method.IsGenericMethodDefinition &&
                    method.IsDefined(typeof(ExtensionAttribute), false))
                .Where(method =>
                {
                    var parameters = method.GetParameters();
                    return parameters.Length >= 3 &&
                        parameters[0].ParameterType.IsAssignableFrom(connectionType) &&
                        parameters[1].ParameterType == typeof(string) &&
                        parameters[2].ParameterType.IsAssignableFrom(readerType) &&
                        parameters.Skip(3).All(parameter => parameter.IsOptional || parameter.ParameterType == typeof(CancellationToken));
                })
                .OrderBy(method => method.GetParameters()[0].ParameterType == connectionType ? 0 : 1)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the types of the loaded <c>BulkOperations</c> assemblies (the ones that the application references are loaded).
        /// </summary>
        /// <returns></returns>
        private static Type[] GetBulkOperationsTypes()
        {
            var loaded = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic).ToList();
            foreach (var name in loaded.SelectMany(GetReferencedNames).Where(name => name.Name != null && name.Name.EndsWith(BulkOperationsSuffix, StringComparison.Ordinal)).ToList())
            {
                if (!loaded.Any(assembly => string.Equals(assembly.GetName().Name, name.Name, StringComparison.Ordinal)))
                {
                    try
                    {
                        loaded.Add(Assembly.Load(name));
                    }
                    catch (Exception e) when (e is System.IO.IOException || e is BadImageFormatException)
                    {
                    }
                }
            }
            foreach (var file in GetBulkOperationsFiles())
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(file);
                if (!loaded.Any(assembly => string.Equals(assembly.GetName().Name, name, StringComparison.Ordinal)))
                {
                    try
                    {
                        loaded.Add(Assembly.LoadFrom(file));
                    }
                    catch (Exception e) when (e is System.IO.IOException || e is BadImageFormatException)
                    {
                    }
                }
            }
            return loaded
                .Where(assembly => assembly.GetName().Name.EndsWith(BulkOperationsSuffix, StringComparison.Ordinal))
                .SelectMany(GetExportedTypes)
                .ToArray();
        }

        /// <summary>
        /// Gets the <c>BulkOperations</c> assemblies that are deployed with the application (a package that the code does not use directly is not referenced by the assembly of the application).
        /// </summary>
        /// <returns></returns>
        private static string[] GetBulkOperationsFiles()
        {
            try
            {
                return System.IO.Directory.GetFiles(AppContext.BaseDirectory, "*" + BulkOperationsSuffix + ".dll");
            }
            catch (Exception e) when (e is System.IO.IOException || e is UnauthorizedAccessException)
            {
                return new string[0];
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static AssemblyName[] GetReferencedNames(Assembly assembly)
        {
            try
            {
                return assembly.GetReferencedAssemblies();
            }
            catch (Exception e) when (e is NotSupportedException || e is System.IO.IOException)
            {
                return new AssemblyName[0];
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static Type[] GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(type => type != null && type.IsPublic).ToArray();
            }
        }

        /// <summary>
        /// Creates the arguments of the method: the optional arguments are given by their names, and the other ones keep their default values.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="batchSize"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static object[] CreateArguments(MethodInfo method,
            IDbConnection connection,
            string tableName,
            IDataReader reader,
            int batchSize,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var parameters = method.GetParameters();
            var arguments = new object[parameters.Length];
            arguments[0] = connection;
            arguments[1] = tableName;
            arguments[2] = reader;
            for (var i = 3; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                switch (parameter.Name)
                {
                    case "batchSize":
                        arguments[i] = batchSize;
                        break;
                    case "bulkCopyTimeout":
                    case "commandTimeout":
                        arguments[i] = commandTimeout;
                        break;
                    case "options":
                    case "bulkCopyOptions":
                        arguments[i] = GetOptions(parameter);
                        break;
                    case "identityBehavior":
                        arguments[i] = GetIdentityBehavior(parameter);
                        break;
                    case "transaction":
                        arguments[i] = parameter.ParameterType.IsInstanceOfType(transaction) ? transaction : GetDefault(parameter);
                        break;
                    case "trace":
                        arguments[i] = parameter.ParameterType.IsInstanceOfType(trace) ? trace : GetDefault(parameter);
                        break;
                    case "traceKey":
                        arguments[i] = traceKey ?? GetDefault(parameter);
                        break;
                    case "cancellationToken":
                        arguments[i] = cancellationToken;
                        break;
                    default:
                        arguments[i] = GetDefault(parameter);
                        break;
                }
            }
            return arguments;
        }

        /// <summary>
        /// Gets the value of the <c>options</c> argument: its default value with the <c>KeepIdentity</c> option when it has one (i.e.: <c>SqlBulkCopyOptions.KeepIdentity</c>),
        /// so the values of the identity columns are copied as they are instead of being generated again by the destination database (the rows of the related tables reference them).
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static object GetOptions(ParameterInfo parameter)
        {
            var type = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            if (!type.IsEnum || !Enum.GetNames(type).Contains(KeepIdentityOption, StringComparer.Ordinal))
            {
                return GetDefault(parameter);
            }
            var value = Convert.ToInt64(GetDefault(parameter) ?? 0, System.Globalization.CultureInfo.InvariantCulture);
            var keepIdentity = Convert.ToInt64(Enum.Parse(type, KeepIdentityOption), System.Globalization.CultureInfo.InvariantCulture);
            return Enum.ToObject(type, value | keepIdentity);
        }

        /// <summary>
        /// Gets the value of the <c>identityBehavior</c> argument: <c>KeepIdentity</c> when the enumeration has it, so the values of the identity columns are copied as they are.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static object GetIdentityBehavior(ParameterInfo parameter)
        {
            var type = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            return type.IsEnum && Enum.GetNames(type).Contains(KeepIdentityOption, StringComparer.Ordinal)
                ? Enum.Parse(type, KeepIdentityOption)
                : GetDefault(parameter);
        }

        /// <summary>
        /// Gets the default value of an optional parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static object GetDefault(ParameterInfo parameter)
        {
            var value = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            if (value == null || value is DBNull || value is Missing)
            {
                return parameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(parameter.ParameterType) == null
                    ? Activator.CreateInstance(parameter.ParameterType)
                    : null;
            }
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static InvalidOperationException CreateMissingException(IDbConnection connection) =>
            new InvalidOperationException($"There is no 'BulkInsert' extension method for the connection '{connection.GetType().FullName}'. Please install and reference the 'BulkOperations' package of the provider.");

        #endregion
    }
}
