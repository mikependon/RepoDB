using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using RepoDb.IntegrationTests.Extensions;
using RepoDb.IntegrationTests.Setup;
using Shouldly;

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class StandardDataTypeMappingTest : FixturePrince
    {
        [Test]
        public void TestStringDataTypesInsert()
        {
            //arrange
            var fixtureData = new Models.TypeMap
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

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key

            //assert
            var saveData = sut.Query<Models.TypeMap>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.char_column.ShouldContain(fixtureData.char_column);;
            saveData.nchar_column.ShouldContain(fixtureData.nchar_column);
            saveData.ntext_column.ShouldBe(fixtureData.ntext_column);
            saveData.nvarchar_column.ShouldBe(fixtureData.nvarchar_column);
            saveData.nvarcharmax_column.ShouldBe(fixtureData.nvarcharmax_column);
            saveData.text_column.ShouldBe(fixtureData.text_column);
            saveData.varchar_column.ShouldBe(fixtureData.varchar_column);
            saveData.varcharmax_column.ShouldBe(fixtureData.varcharmax_column);
            saveData.uniqueidentifier.ShouldBe(fixtureData.uniqueidentifier);

        }

        [Test]
        public void TestNumericDataTypesInsert()
        {
            //arrange
            var fixtureData = new Models.TypeMap
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

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //assert
            var saveData = sut.Query<Models.TypeMap>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.bigint_column.ShouldBe(fixtureData.bigint_column);
            saveData.bit_column.ShouldBe(fixtureData.bit_column);
            saveData.decimal_column.ShouldBe(fixtureData.decimal_column);
            saveData.float_column.ShouldBe(fixtureData.float_column);
            saveData.int_column.ShouldBe(fixtureData.int_column);
            saveData.money_column.ShouldBe(fixtureData.money_column);
            saveData.numeric_column.ShouldBe(fixtureData.numeric_column);
            saveData.real_column.ShouldBe(fixtureData.real_column);
            saveData.smallint_column.ShouldBe(fixtureData.smallint_column);
            saveData.smallmoney_column.ShouldBe(fixtureData.smallmoney_column);
            saveData.tinyint_column.ShouldBe(fixtureData.tinyint_column);
        }

        [Test]
        public void TestDateTimeDataTypes()
        {
            //arrange
            var baseTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(20).AddSeconds(30).AddMilliseconds(123456789);

            var dateTimeOffset = new DateTimeOffset(2007, 11, 22, 16, 0, 0, new TimeSpan(-5, 0, 0));
            var time = baseTime.TimeOfDay;

            var fixtureData = new Models.TypeMap
            {
                date_column = baseTime.Date,
                datetime_column = baseTime,
                datetime2_column = baseTime,
                datetimeoffset_column = dateTimeOffset,
                smalldatetime_column = baseTime,
                time_column = time
            };

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key
            
            //assert
            var saveData = sut.Query<Models.TypeMap>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.date_column.ShouldBe(fixtureData.date_column);
            saveData.datetime_column.ShouldBeEx(fixtureData.datetime_column);
            saveData.datetime2_column.ShouldBeEx(fixtureData.datetime2_column);
            saveData.datetimeoffset_column.ShouldBe(fixtureData.datetimeoffset_column);
            saveData.smalldatetime_column.ShouldBeEx(fixtureData.smalldatetime_column);
            saveData.time_column.Value.TotalMilliseconds.ShouldBe(fixtureData.time_column.Value.TotalMilliseconds);
        }

        [Test]
        public void TestXmlDataTypeInsert()
        {
            //arrange
            var sampleXmlData = @"
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
                xml_column = sampleXmlData
            };

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key

            //assert
            var saveData = sut.Query<Models.TypeMapXml>(top: 1).FirstOrDefault();

            var testXmlData = Regex.Replace(fixtureData.xml_column, @"\s+", string.Empty);
            var savedXmlData = Regex.Replace(saveData.xml_column, @"\s+", string.Empty);

            saveData.ShouldNotBeNull();
            savedXmlData.ShouldBe(testXmlData);
        }
    }
}
