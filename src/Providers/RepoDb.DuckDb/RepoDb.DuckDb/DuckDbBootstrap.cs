#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.PropertyHandlers.DuckDb;
using RepoDb.StatementBuilders;
using System;
using System.IO;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="DuckDBConnection"/> object.
    /// </summary>
    public static class DuckDbBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///
        /// </summary>
        internal static void InitializeInternal()
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Map the DbSetting
            DbSettingMapper.Add<DuckDBConnection>(new DuckDbDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<DuckDBConnection>(new DuckDbDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<DuckDBConnection>(new DuckDbStatementBuilder(), true);

            // Map the PropertyHandler for BLOB columns at the type level (keyed by the reader's reported
            // CLR type, System.IO.Stream - see DuckDbStreamToByteArrayPropertyHandler). DuckDB.NET returns
            // an unmanaged Stream when reading a BLOB value, which has no built-in coercion to byte[]. This
            // makes any byte[]-typed property against a DuckDB BLOB column work without requiring the
            // caller to register a per-property handler (an explicit per-property registration, like the
            // ones in the integration tests' Database.cs, still takes precedence over this default).
            PropertyHandlerMapper.Add<Stream, DuckDbStreamToByteArrayPropertyHandler>(true);

            // Map the PropertyHandler for TIME columns at the type level (keyed by System.TimeSpan - the
            // CLR type DuckDbTypeNameToClientTypeResolver resolves a TIME column's DbField.Type to). This
            // one matters specifically for dynamic/table-name writes (e.g. InsertAll(tableName, entities)
            // against anonymous/ExpandoObject entities): RepoDb.Core's dynamic parameter-creation path has
            // no ClassProperty to attach a per-property handler to, so it falls back to this type-level
            // registration instead (see RepoDb.Reflection.Compiler.GetDictionaryStringObjectPropertyValueExpression/
            // GetObjectInstancePropertyValueExpression). Without it, a raw TimeSpan value is handed straight
            // to DuckDB.NET, whose converter has no case for (DuckDBType.Time, TimeSpan) and blindly casts
            // the boxed value to its native DuckDBTimeOnly struct, throwing InvalidCastException. DuckDB.NET
            // *does* accept a plain DateTime for DATE columns natively, so no equivalent registration is
            // needed for DuckDbDateOnlyToDateTimePropertyHandler here.
            PropertyHandlerMapper.Add<TimeSpan, DuckDbTimeOnlyToTimeSpanPropertyHandler>(true);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
