using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateMergeTest
    {
        private class TestCreateMergeWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithoutMappings()
        {
            throw new NotImplementedException();
        }
    }
}
