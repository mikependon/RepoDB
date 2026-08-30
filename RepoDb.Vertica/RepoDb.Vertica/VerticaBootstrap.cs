#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;
using System.Globalization;
using System.Threading;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize necessary objects that is connected to <see cref="VerticaConnection"/> object.
    /// </summary>
    public static class VerticaBootstrap
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
            DbSettingMapper.Add<VerticaConnection>(new VerticaDbSetting(), true);

            // Map the DbHelper
            DbHelperMapper.Add<VerticaConnection>(new VerticaDbHelper(), true);

            // Map the Statement Builder
            StatementBuilderMapper.Add<VerticaConnection>(new VerticaStatementBuilder(), true);

            // Vertica.Data (confirmed against v24.1.0 and v24.3.0) formats/re-parses date-like parameter
            // values using the ambient thread culture instead of CultureInfo.InvariantCulture. On any
            // machine whose culture uses a non-colon time separator (e.g. en-DK renders 13:45:30 as
            // 13.45.30), this corrupts the value the driver actually sends - a native DateTime bound to a
            // TIMESTAMP/TIME column fails INSERT with "Row 1 was rejected by the server", and even a plain
            // VarChar parameter carrying an already-correct "HH:mm:ss" string comes back re-formatted with
            // dots and fails server-side parsing on UPDATE/SELECT. There is no per-call interception point
            // available to a provider (RepoDb.Core calls VerticaCommand.ExecuteScalar()/ExecuteNonQuery()/
            // ExecuteReader() directly), so the only reliable fix is to force Invariant culture for the
            // calling thread now, and for every new thread this process creates from here on.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
