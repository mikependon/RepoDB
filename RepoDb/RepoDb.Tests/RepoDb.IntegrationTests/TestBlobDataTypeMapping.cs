using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using RepoDb.IntegrationTests.Setup;
using Shouldly;

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class TestBlobDataTypeMapping : FixturePrince
    {
        [Test]
        public void BinaryTypeMap()
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

            var resulText = Encoding.ASCII.GetString(saveData.binary_column);
            resulText.ShouldBe(baseText);

            saveData.binary_column.ShouldBe(fixtureData.binary_column);
            saveData.image_column.ShouldBe(fixtureData.image_column);
            saveData.varbinary_column.ShouldBe(fixtureData.varbinary_column);
            saveData.varbinarymax_column.ShouldBe(fixtureData.varbinarymax_column);
        }

        [Test]
        public void ImageTypeMap()
        {
            //arrange
            var resourceName = "RepoDb.IntegrationTests.Setup.hello-world.png";
            var baseByteData = ExtractResource(resourceName);
            
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
            saveData.binary_column.ShouldBe(fixtureData.binary_column);
            saveData.image_column.ShouldBe(fixtureData.image_column);
            saveData.varbinary_column.ShouldBe(fixtureData.varbinary_column);
            saveData.varbinarymax_column.ShouldBe(fixtureData.varbinarymax_column);
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