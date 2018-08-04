using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using RepoDb.IntegrationTests.Setup;
using Shouldly;
using System;

//https://stackoverflow.com/questions/425389/c-sharp-equivalent-of-sql-server-datatypes

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class BlobDataTypeMappingTest : FixturePrince
    {
        [Test]
        public void BlobStringTypeMap()
        {
            //arrange
            var baseText = @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
            byte[] baseByteData = Encoding.ASCII.GetBytes(baseText);

            var fixtureData = new Models.TypeMapBlob
            {
                binary_column = baseByteData,
                image_column = baseByteData,
                varbinary_column = baseByteData,
                varbinarymax_column = baseByteData
            };

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key

            //assert
            var saveData = sut.Query<Models.TypeMapBlob>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();

            var binary_column = saveData.binary_column.Take(fixtureData.binary_column.Length);
            var resulText = Encoding.ASCII.GetString(binary_column?.ToArray());

            resulText.ShouldBe(baseText);
            binary_column?.SequenceEqual(fixtureData.binary_column).ShouldBe(true);

            saveData.image_column.SequenceEqual(fixtureData.image_column).ShouldBe(true);
            saveData.varbinary_column.SequenceEqual(fixtureData.varbinary_column).ShouldBe(true);
            saveData.varbinarymax_column.SequenceEqual(fixtureData.varbinarymax_column).ShouldBe(true);
        }

        [Test]
        public void BlobImageTypeMap()
        {
            //arrange
            var resourceName = "RepoDb.IntegrationTests.Setup.hello-world.png";
            var baseByteData = ExtractResource(resourceName);

            var fixtureData = new Models.TypeMapBlob
            {
                image_column = baseByteData,
                varbinary_column = baseByteData,
                varbinarymax_column = baseByteData
            };

            //act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key

            //assert
            var saveData = sut.Query<Models.TypeMapBlob>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.image_column.SequenceEqual(fixtureData.image_column).ShouldBe(true);
            saveData.varbinary_column.SequenceEqual(fixtureData.varbinary_column).ShouldBe(true);
            saveData.varbinarymax_column.SequenceEqual(fixtureData.varbinarymax_column).ShouldBe(true);
        }

        public static byte[] ExtractResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(filename))
            {
                if (stream == null) return null;
                byte[] ba = new byte[stream.Length];
                stream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}