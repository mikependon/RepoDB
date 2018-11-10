using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using RepoDb.IntegrationTests.Setup;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class StandardDataTypeMappingTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SetupHelper.CleanDatabase();
        }

        [TestMethod]
        public void TestStringDataTypesInsert()
        {
            // Setup
            var data = new Models.TypeMap
            {
                char_column = "char text",
                nchar_column = "nchar text",
                ntext_column = "ntext text",
                nvarchar_column = "nvarchar  text",
                nvarcharmax_column = "nvarcharmax text",
                text_column = "text text",
                varchar_column = "varchar text",
                varcharmax_column = "varcharmax text",
                uniqueidentifier = Guid.NewGuid()
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var id = sut.Insert(data);
            var result = sut.Query<Models.TypeMap>(top: 1).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.char_column.ShouldContain(data.char_column); ;
            result.nchar_column.ShouldContain(data.nchar_column);
            result.ntext_column.ShouldBe(data.ntext_column);
            result.nvarchar_column.ShouldBe(data.nvarchar_column);
            result.nvarcharmax_column.ShouldBe(data.nvarcharmax_column);
            result.text_column.ShouldBe(data.text_column);
            result.varchar_column.ShouldBe(data.varchar_column);
            result.varcharmax_column.ShouldBe(data.varcharmax_column);
            result.uniqueidentifier.ShouldBe(data.uniqueidentifier);
        }

        [TestMethod]
        public void TestNumericDataTypesInsert()
        {
            // Setup
            var data = new Models.TypeMapNumeric
            {
                bigint_column = 123456789,
                bit_column = true,
                decimal_column = 12345.6789m,
                float_column = 12345.6789,
                int_column = 123456789,
                money_column = 12345.6789m,
                numeric_column = 12345.6789m,
                real_column = 12345,
                smallint_column = 123,
                smallmoney_column = 12345.6789m,
                tinyint_column = 12
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var id = sut.Insert(data);
            var result = sut.Query<Models.TypeMapNumeric>(top: 1).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.bigint_column.ShouldBe(data.bigint_column);
            result.bit_column.ShouldBe(data.bit_column);
            result.decimal_column.ShouldBe(data.decimal_column);
            result.float_column.ShouldBe(data.float_column);
            result.int_column.ShouldBe(data.int_column);
            result.money_column.ShouldBe(data.money_column);
            result.numeric_column.ShouldBe(data.numeric_column);
            result.real_column.ShouldBe(data.real_column);
            result.smallint_column.ShouldBe(data.smallint_column);
            result.smallmoney_column.ShouldBe(data.smallmoney_column);
            result.tinyint_column.ShouldBe(data.tinyint_column);
        }

        [TestMethod]
        public void TestDateTimeDataTypes()
        {
            // Setup
            var baseTime = new DateTime(2018, 2, 15, 3, 12, 15, 123);
            var dateTimeOffset = new DateTimeOffset(2007, 11, 22, 16, 0, 0, new TimeSpan(-5, 0, 0));
            var data = new Models.TypeMapDate
            {
                date_column = baseTime.Date,
                datetime_column = baseTime,
                datetime2_column = baseTime,
                smalldatetime_column = baseTime,
                datetimeoffset_column = dateTimeOffset,
                time_column = baseTime.TimeOfDay
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var id = sut.Insert(data);
            var result = sut.Query<Models.TypeMapDate>(top: 1).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.date_column.ShouldBe(data.date_column);
            result.datetime_column.ShouldBe(data.datetime_column);
            result.datetime2_column.ShouldBe(data.datetime2_column);
            result.datetimeoffset_column.ShouldBe(data.datetimeoffset_column);
            result.smalldatetime_column.ShouldBe(new DateTime(2018, 2, 15, 3, 12, 0, 0));
            result.time_column.Value.ShouldBe(data.time_column.Value);
        }

        [TestMethod]
        public void TestXmlDataTypeInsert()
        {
            // Setup
            var data = @"
                    <dataset>
	                    <register>
		                    <cpf>82334792845</cpf>
		                    <cnpj>60286360000123</cnpj>
		                    <ssn>549945866</ssn>
		                    <number>90</number>
		                    <name>Vasiliki Croft</name>
	                    </register>
	                    <register>
		                    <cpf>75253258639</cpf>
		                    <cnpj>84055144000135</cnpj>
		                    <ssn>245502705</ssn>
		                    <number>24</number>
		                    <name>Grady Harris</name>
	                    </register>
	                    <register>
		                    <cpf>32151268853</cpf>
		                    <cnpj>75107372000110</cnpj>
		                    <ssn>327200768</ssn>
		                    <number>83</number>
		                    <name>Claud Bryan</name>
	                    </register>
                    </dataset>
                    ";
            var fixtureData = new Models.TypeMapXml
            {
                xml_column = data
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);
            var result = sut.Query<Models.TypeMapXml>(top: 1).FirstOrDefault();
            var dataXml = Regex.Replace(fixtureData.xml_column, @"\s+", string.Empty);
            var resultXml = Regex.Replace(result.xml_column, @"\s+", string.Empty);

            // Assert
            result.ShouldNotBeNull();
            resultXml.ShouldBe(dataXml);
        }
    }
}
