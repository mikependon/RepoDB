#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class JsonRowReaderTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text,
            bool bom = false)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_json_reader_" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllBytes(path, new UTF8Encoding(bom).GetPreamble().Concat(Encoding.UTF8.GetBytes(text)).ToArray());
            _files.Add(path);
            return path;
        }

        private static string Array(int rows) =>
            "[" + string.Join(",", Enumerable.Range(1, rows).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")) + "]";

        private static List<int> Ids(string path,
            int bufferSize = 64 * 1024)
        {
            var ids = new List<int>();
            using (var reader = new JsonRowReader(path, bufferSize))
            {
                while (true)
                {
                    using (var row = reader.Next())
                    {
                        if (row == null)
                        {
                            return ids;
                        }
                        ids.Add(row.RootElement.GetProperty("Id").GetInt32());
                    }
                }
            }
        }

        private static async Task<List<int>> IdsAsync(string path,
            int bufferSize = 64 * 1024)
        {
            var ids = new List<int>();
            using (var reader = new JsonRowReader(path, bufferSize))
            {
                while (true)
                {
                    using (var row = await reader.NextAsync())
                    {
                        if (row == null)
                        {
                            return ids;
                        }
                        ids.Add(row.RootElement.GetProperty("Id").GetInt32());
                    }
                }
            }
        }

        private static int Skipped(string path,
            int bufferSize = 64 * 1024)
        {
            var count = 0;
            using (var reader = new JsonRowReader(path, bufferSize))
            {
                while (reader.Skip())
                {
                    count++;
                }
            }
            return count;
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var file in _files.Where(File.Exists))
            {
                File.Delete(file);
            }
            _files.Clear();
        }

        #endregion

        #region Shapes

        [TestMethod]
        public void TestNextReadsTheObjectsOfAnArray()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2},{\"Id\":3}]");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, actual);
        }

        [TestMethod]
        public async Task TestNextAsyncReadsTheObjectsOfAnArray()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2},{\"Id\":3}]");

            // Act
            var actual = await IdsAsync(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, actual);
        }

        [TestMethod]
        public void TestNextReadsTheFirstArrayPropertyOfAnObject()
        {
            // Setup
            var path = Write("{\"meta\":{\"source\":\"x\",\"tags\":[1,2]},\"count\":2,\"rows\":[{\"Id\":1},{\"Id\":2}],\"rest\":[{\"Id\":9}]}");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public async Task TestNextAsyncReadsTheFirstArrayPropertyOfAnObject()
        {
            // Setup
            var path = Write("{\"meta\":{\"source\":\"x\"},\"rows\":[{\"Id\":1},{\"Id\":2}]}");

            // Act
            var actual = await IdsAsync(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public void TestNextReadsASequenceOfObjectsOnePerLine()
        {
            // Setup
            var path = Write("{\"Id\":1,\"Name\":\"A\"}\n{\"Id\":2,\"Name\":\"B\"}\r\n{\"Id\":3,\"Name\":\"C\"}\n");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, actual);
        }

        [TestMethod]
        public async Task TestNextAsyncReadsASequenceOfObjectsOnePerLine()
        {
            // Setup
            var path = Write("{\"Id\":1}\n{\"Id\":2}\n{\"Id\":3}\n");

            // Act
            var actual = await IdsAsync(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, actual);
        }

        [TestMethod]
        public void TestNextReadsASequenceOfObjectsThatAreNotSeparatedByALine()
        {
            // Setup
            var path = Write("{\"Id\":1}{\"Id\":2} {\"Id\":3}");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, actual);
        }

        [TestMethod]
        public void TestNextReadsASequenceOfObjectsThatHaveNestedObjects()
        {
            // Setup
            var path = Write("{\"Id\":1,\"Detail\":{\"X\":1}}\n{\"Id\":2,\"Detail\":{\"X\":2}}\n");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public void TestNextReadsASingleObjectAsASequenceOfOneRow()
        {
            // Setup
            var path = Write("{\"Id\":7,\"Name\":\"Only\"}");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 7 }, actual);
        }

        [TestMethod]
        public void TestNextReadsAMinifiedFileLikeAnIndentedOne()
        {
            // Setup
            var minified = Write(Array(5));
            var indented = Write("[\n  " + string.Join(",\n  ", Enumerable.Range(1, 5).Select(i => "{\n    \"Id\": " + i + ",\n    \"Name\": \"Name " + i + "\"\n  }")) + "\n]");

            // Act
            var first = Ids(minified);
            var second = Ids(indented);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, first);
            CollectionAssert.AreEqual(first, second);
        }

        [TestMethod]
        public void TestNextReadsAnEmptyArrayAsNoRow()
        {
            // Setup
            var path = Write("[]");

            // Act
            var actual = Ids(path);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestNextReadsAnObjectWithAnEmptyArrayAsNoRow()
        {
            // Setup
            var path = Write("{\"rows\":[]}");

            // Act
            var actual = Ids(path);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestNextSkipsTheByteOrderMark()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2}]", true);

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2 }, actual);
        }

        [TestMethod]
        public void TestNextAllowsTheCommentsAndTheTrailingCommas()
        {
            // Setup
            var path = Write("[ // the rows\n  {\"Id\":1,}, /* second */ {\"Id\":2,},\n]");

            // Act
            var actual = Ids(path);

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 2 }, actual);
        }

        #endregion

        #region Blocks Of The File

        [DataTestMethod]
        [DataRow(16)]
        [DataRow(17)]
        [DataRow(64)]
        [DataRow(1000)]
        public void TestNextReadsAnArrayThatIsMuchLargerThanTheBuffer(int bufferSize)
        {
            // Setup
            var path = Write(Array(200));

            // Act
            var actual = Ids(path, bufferSize);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 200).ToArray(), actual);
        }

        [DataTestMethod]
        [DataRow(16)]
        [DataRow(33)]
        [DataRow(1000)]
        public async Task TestNextAsyncReadsAnArrayThatIsMuchLargerThanTheBuffer(int bufferSize)
        {
            // Setup
            var path = Write(Array(200));

            // Act
            var actual = await IdsAsync(path, bufferSize);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 200).ToArray(), actual);
        }

        [DataTestMethod]
        [DataRow(16)]
        [DataRow(40)]
        public void TestNextReadsASequenceThatIsMuchLargerThanTheBuffer(int bufferSize)
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 150).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")));

            // Act
            var actual = Ids(path, bufferSize);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 150).ToArray(), actual);
        }

        [TestMethod]
        public void TestNextReadsTheRowsOfAnObjectThatHasALargePartBeforeTheArray()
        {
            // Setup
            var path = Write("{\"meta\":{\"text\":\"" + new string('x', 500) + "\"},\"rows\":" + Array(20) + "}");

            // Act
            var actual = Ids(path, 16);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 20).ToArray(), actual);
        }

        [TestMethod]
        public void TestNextReadsARowThatIsLargerThanTheBuffer()
        {
            // Setup
            var big = new string('y', 5000);
            var path = Write("[{\"Id\":1,\"Text\":\"" + big + "\"},{\"Id\":2,\"Text\":\"short\"}]");

            // Act
            var texts = new List<string>();
            using (var reader = new JsonRowReader(path, 16))
            {
                while (true)
                {
                    using (var row = reader.Next())
                    {
                        if (row == null)
                        {
                            break;
                        }
                        texts.Add(row.RootElement.GetProperty("Text").GetString());
                    }
                }
            }

            // Assert
            Assert.AreEqual(2, texts.Count);
            Assert.AreEqual(big, texts[0]);
            Assert.AreEqual("short", texts[1]);
        }

        [TestMethod]
        public void TestNextReadsAFileWithAByteOrderMarkAndASmallBuffer()
        {
            // Setup
            var path = Write(Array(30), true);

            // Act
            var actual = Ids(path, 16);

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(1, 30).ToArray(), actual);
        }

        #endregion

        #region Skip

        [TestMethod]
        public void TestSkipMovesOverTheRowsOfAnArray()
        {
            // Setup
            var path = Write(Array(25));

            // Act
            var actual = Skipped(path);

            // Assert
            Assert.AreEqual(25, actual);
        }

        [TestMethod]
        public void TestSkipMovesOverTheRowsOfAnObjectThatHoldsThem()
        {
            // Setup
            var path = Write("{\"meta\":{},\"rows\":" + Array(7) + "}");

            // Act
            var actual = Skipped(path);

            // Assert
            Assert.AreEqual(7, actual);
        }

        [TestMethod]
        public void TestSkipMovesOverTheRowsOfASequence()
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 12).Select(i => "{\"Id\":" + i + "}")));

            // Act
            var actual = Skipped(path);

            // Assert
            Assert.AreEqual(12, actual);
        }

        [TestMethod]
        public void TestSkipMovesOverTheRowsOfAFileMuchLargerThanTheBuffer()
        {
            // Setup
            var path = Write(Array(300));

            // Act
            var actual = Skipped(path, 16);

            // Assert
            Assert.AreEqual(300, actual);
        }

        [TestMethod]
        public async Task TestSkipAsyncMovesOverTheRows()
        {
            // Setup
            var path = Write(Array(40));
            var count = 0;

            // Act
            using (var reader = new JsonRowReader(path, 32))
            {
                while (await reader.SkipAsync())
                {
                    count++;
                }
            }

            // Assert
            Assert.AreEqual(40, count);
        }

        [TestMethod]
        public void TestSkipOfAnEmptyArrayFindsNoRow()
        {
            // Setup
            var path = Write("[]");

            // Act
            var actual = Skipped(path);

            // Assert
            Assert.AreEqual(0, actual);
        }

        #endregion

        #region Row Number

        [TestMethod]
        public void TestRowNumberCountsTheRowsThatWereRead()
        {
            // Setup
            var path = Write(Array(3));
            var numbers = new List<long>();

            // Act
            using (var reader = new JsonRowReader(path))
            {
                numbers.Add(reader.RowNumber);
                while (true)
                {
                    using (var row = reader.Next())
                    {
                        if (row == null)
                        {
                            break;
                        }
                        numbers.Add(reader.RowNumber);
                    }
                }
                numbers.Add(reader.RowNumber);
            }

            // Assert
            CollectionAssert.AreEqual(new long[] { 0, 1, 2, 3, 3 }, numbers);
        }

        [TestMethod]
        public void TestNextAfterTheLastRowKeepsReturningNull()
        {
            // Setup
            var path = Write(Array(1));

            // Act
            using (var reader = new JsonRowReader(path))
            {
                using (var first = reader.Next())
                {
                    Assert.IsNotNull(first);
                }

                // Assert
                Assert.IsNull(reader.Next());
                Assert.IsNull(reader.Next());
                Assert.IsFalse(reader.Skip());
            }
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnNextIfAnItemOfTheArrayIsNotAnObject()
        {
            // Setup
            var path = Write("[{\"Id\":1},5,{\"Id\":3}]");

            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => Ids(path));
            StringAssert.Contains(exception.Message, "Item 2", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfTheArrayIsNotClosed()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2}");

            // Act/Assert
            Assert.Throws<FormatException>(() => Ids(path));
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfARowIsNotClosed()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2,\"Name\":");

            // Act/Assert
            Assert.Throws<FormatException>(() => Ids(path));
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfTheFileIsNotValidJson()
        {
            // Setup
            var path = Write("[{\"Id\":1},{Id:2}]");

            // Act/Assert
            Assert.Throws<FormatException>(() => Ids(path));
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfTheFileIsEmpty()
        {
            // Setup
            var path = Write(string.Empty);

            // Act/Assert
            Assert.Throws<FormatException>(() => Ids(path));
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfTheRootIsAValueThatIsNotAnArrayOrAnObject()
        {
            // Setup
            var path = Write("42");

            // Act/Assert
            Assert.Throws<FormatException>(() => Ids(path));
        }

        [TestMethod]
        public void ThrowExceptionOnNextIfTheSequenceHasAValueThatIsNotAnObject()
        {
            // Setup
            var path = Write("{\"Id\":1}\n5\n{\"Id\":3}");

            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => Ids(path));
            StringAssert.Contains(exception.Message, "Item 2", StringComparison.Ordinal);
        }

        [TestMethod]
        public async Task ThrowExceptionOnNextAsyncIfAnItemOfTheArrayIsNotAnObject()
        {
            // Setup
            var path = Write("[{\"Id\":1},\"text\"]");

            // Act/Assert
            await Assert.ThrowsAsync<FormatException>(() => IdsAsync(path));
        }

        [TestMethod]
        public void ThrowExceptionOnSkipIfTheArrayIsNotClosed()
        {
            // Setup
            var path = Write("[{\"Id\":1},{\"Id\":2}");

            // Act/Assert
            Assert.Throws<FormatException>(() => Skipped(path));
        }

        [TestMethod]
        public void ThrowExceptionOnTheConstructorIfTheBufferIsTooSmall()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonRowReader(path, 8));
        }

        [TestMethod]
        public void ThrowExceptionOnTheConstructorIfTheFileDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<FileNotFoundException>(() => new JsonRowReader(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json")));
        }

        #endregion

        #region Dispose

        [TestMethod]
        public void TestDisposeReleasesTheFile()
        {
            // Setup
            var path = Write(Array(2));
            var reader = new JsonRowReader(path);
            using (reader.Next())
            {
            }

            // Act
            reader.Dispose();

            // Assert
            File.Delete(path);
            Assert.IsFalse(File.Exists(path));
        }

        #endregion
    }
}
