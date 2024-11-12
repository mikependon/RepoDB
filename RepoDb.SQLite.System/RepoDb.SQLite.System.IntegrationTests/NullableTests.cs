using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;

namespace RepoDb.SQLite.System.IntegrationTests
{
    [TestClass]
    public class NullableTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }



        [TestMethod]
        [DataRow(ConversionType.Default)]
        [DataRow(ConversionType.Automatic)]
        public void InsertAndFetch(ConversionType conversionType)
        {
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = conversionType
                })
                .UseSQLite();

            using (var connection = new SQLiteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                connection.Insert(new SdsCompleteTable(), trace: new SimpleTrace());


                var original = connection.QueryAll<SdsCompleteTable>().Single();

                GlobalConfiguration
                    .Setup(new()
                    {
                        ConversionType = ConversionType.Default
                    })
                    .UseSQLite();

                var readDefault = connection.QueryAll<SdsCompleteTable>().Single();

                GlobalConfiguration
                    .Setup(new()
                    {
                        ConversionType = ConversionType.Automatic
                    })
                    .UseSQLite();

                var readAuto = connection.QueryAll<SdsCompleteTable>().Single();


                Assert.AreEqual(original.ColumnBigInt, readDefault.ColumnBigInt);
                Assert.AreEqual(original.ColumnInt, readDefault.ColumnInt);

                Assert.AreEqual(original.ColumnBigInt, readAuto.ColumnBigInt);
                Assert.AreEqual(original.ColumnInt, readAuto.ColumnInt);

            }
        }
    }
    public class SimpleTrace : ITrace
    {
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
        }

        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void BeforeExecution(CancellableTraceLog log)
        {
            Console.WriteLine(log);
        }

        public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(log);

            return Task.CompletedTask;
        }
    }
}
