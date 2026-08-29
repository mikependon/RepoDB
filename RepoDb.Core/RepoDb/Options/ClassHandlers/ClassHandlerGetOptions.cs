#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data.Common;

namespace RepoDb.Options
{
    /// <summary>
    /// An option class that is containing the optional values when pushing the class properties values towards the database.
    /// </summary>
    public sealed class ClassHandlerGetOptions : ClassHandlerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        private ClassHandlerGetOptions() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal ClassHandlerGetOptions(DbDataReader reader)
        {
            DataReader = reader;
        }

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="DbDataReader"/> object in used during the hydration process.
        /// </summary>
        public DbDataReader DataReader { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static ClassHandlerGetOptions Create(DbDataReader reader) =>
            new ClassHandlerGetOptions(reader);

        #endregion
    }
}
