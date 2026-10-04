#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that is used to register the SQL Server schema objects.
    /// </summary>
    public static class SqlServerSchemaBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the bootstrap has been initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the <see cref="SqlServerSchemaComposer"/> to the <see cref="SchemaComposerMapper"/> for the <see cref="SqlConnection"/> type.
        /// A <see cref="SqlServerSchemaReader"/> owns the connection that it reads, so it is created with its connection instead of being registered here.
        /// </summary>
        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            SchemaComposerMapper.Add<SqlConnection>(new SqlServerSchemaComposer(), force: true);
            IsInitialized = true;
        }

        #endregion
    }
}
