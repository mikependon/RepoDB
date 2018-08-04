using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;

namespace RepoDb.IntegrationTests
{
    [TestFixture()]
    public class InlineCrudTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }
    }
}