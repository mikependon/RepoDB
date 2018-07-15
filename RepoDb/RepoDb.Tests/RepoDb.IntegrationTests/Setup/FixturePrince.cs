using System;
using System.Data;
using NUnit.Framework;

namespace RepoDb.IntegrationTests.Setup
{
    [TestFixture]
    public class FixturePrince
    {
        [SetUp]
        public void Setup()
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            //SetupHelper.CleanDatabase();
            SetupHelper.InitDatabase();
        }

        [TearDown]
        public void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }
    }
}
