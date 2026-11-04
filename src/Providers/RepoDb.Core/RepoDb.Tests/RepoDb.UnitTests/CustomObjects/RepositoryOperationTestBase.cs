#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Registers an isolated fake provider for repository behavior tests.
    /// </summary>
    public abstract class RepositoryOperationTestBase
    {
        private ConversionType conversionType;

        [TestInitialize]
        public void Initialize()
        {
            conversionType = GlobalConfiguration.Options.ConversionType;
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            DbSettingMapper.Add<CoverageDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<CoverageDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<CoverageDbConnection>(new CustomStatementBuilder(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            GlobalConfiguration.Options.ConversionType = conversionType;
            DbSettingMapper.Remove<CoverageDbConnection>();
            DbHelperMapper.Remove<CoverageDbConnection>();
            StatementBuilderMapper.Remove<CoverageDbConnection>();
        }
    }
}
