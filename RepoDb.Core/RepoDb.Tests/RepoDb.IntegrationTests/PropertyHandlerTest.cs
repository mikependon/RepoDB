using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RepoDb.IntegrationTests
{
    public class Address
    {
        public string Value { get; set; }
    }

    public class AddressHandler : IPropertyHandler<string, Address>
    {
        public Address Get(string input)
        {
            return new Address { Value = $"{input} :: {Guid.NewGuid()}" };
        }

        public string Set(Address input)
        {
            return $"{input.Value} :: Processed by the handler.";
        }
    }

    [Map("[dbo].[NonIdentityTable]")]
    public class EntityModel
    {
        public Guid Id { get; set; }

        [Map("ColumnNVarChar")]
        [PropertyHandler(typeof(AddressHandler))]
        public Address Address { get; set; }
    }

    [TestClass]
    public class PropertyHandlerTest
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
        public void TestPropertyHandler()
        {
            // Setup
            var models = new List<EntityModel>();
            for (var i = 0; i < 10; i++)
            {
                models.Add(new EntityModel
                {
                    Id = Guid.NewGuid(),
                    Address = new Address
                    {
                        Value = $"Address{i}-{Guid.NewGuid()}"
                    }
                });
            }

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModel>();
            }
        }
    }
}
