#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Tests;
using RepoDb.Schema.AuroraDbMySqlConnector.UnitTests.CustomObjects;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    [TestClass]
    public class LoadDefinitionTest : LoadDefinitionTestBase
    {
        protected override IDbConnection Connection(bool exists)
        {
            SchemaComposerMapper.Add<CustomDbConnection>(new AuroraDbMySqlConnectorSchemaComposer(), true);
            return new CustomDbConnection { ScalarResult = _ => exists ? 1 : 0 };
        }

        protected override IList<string> Commands(IDbConnection connection) =>
            ((CustomDbConnection)connection).ExecutedCommands;
    }
}
