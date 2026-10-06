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
        /// <param name="commandTimeout"></param>
        /// <param name="traceKey"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows that were inserted.</returns>
        internal static int BulkInsert(IDbConnection connection,
            string tableName,
            IDataReader reader,
            int batchSize,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            var method = Find(connection, reader, false) ?? throw CreateMissingException(connection);
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

        /// <summary>
        /// Bulk inserts the rows of the reader into the table in an asynchronous way.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="batchSize"></param>
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
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var method = Find(connection, reader, true) ?? throw CreateMissingException(connection);
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
