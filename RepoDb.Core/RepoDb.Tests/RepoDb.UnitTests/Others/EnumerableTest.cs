using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class EnumerableTest
    {
        #region SubClasses

        private class EnumerableTestClass
        {
            public int Id { get; set; }
            public string Property1 { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<EnumerableTestClass> GetEntities(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EnumerableTestClass
                {
                    Id = i,
                    Property1 = Guid.NewGuid().ToString()
                };
            }
        }

        private void AssertRange(int from,
            int to,
            IEnumerable<EnumerableTestClass> source,
            IEnumerable<EnumerableTestClass> target)
        {
            for (var i = from; i <= to; i++)
            {
                var sourceEntity = source.ElementAt(i);
                var targetEntity = source.ElementAt(i);
                Assert.AreEqual(sourceEntity.Id, targetEntity.Id);
                Assert.AreEqual(sourceEntity.Property1, targetEntity.Property1);
            }
        }

        #endregion

        #region Split

        /*
         * Count
         */

        [TestMethod]
        public void TestEnumerableSplitCountLessThanSize()
        {
            // Setup
            var entities = GetEntities(100);

            // Act
            var result = entities.Split(50);
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod]
        public void TestEnumerableSplitCountEqualsToSize()
        {
            // Setup
            var entities = GetEntities(100);

            // Act
            var result = entities.Split(100);
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod]
        public void TestEnumerableSplitCountGreatherThanSize()
        {
            // Setup
            var entities = GetEntities(100);

            // Act
            var result = entities.Split(120);
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod]
        public void TestEnumerableSplitCountModularFromSize()
        {
            // Setup
            var entities = GetEntities(100);

            // Act
            var result = entities.Split(33);
            var expected = 4;

            // Assert
            Assert.AreEqual(expected, result.Count());
        }

        /*
         * Value
         */

        [TestMethod]
        public void TestEnumerableSplitValueLessThanSize()
        {
            // Setup
            var entities = GetEntities(100).AsList();

            // Act
            var result = entities.Split(50);

            // Assert
            AssertRange(0, 49, entities, result.ElementAt(0));
            AssertRange(50, 99, entities, result.ElementAt(1));
        }

        [TestMethod]
        public void TestEnumerableSplitValueEqualsToSize()
        {
            // Setup
            var entities = GetEntities(100).AsList();

            // Act
            var result = entities.Split(100);

            // Assert
            AssertRange(0, 99, entities, result.ElementAt(0));
        }

        [TestMethod]
        public void TestEnumerableSplitValueGreatherThanSize()
        {
            // Setup
            var entities = GetEntities(100).AsList();

            // Act
            var result = entities.Split(120);

            // Assert
            AssertRange(0, 99, entities, result.ElementAt(0));
        }

        [TestMethod]
        public void TestEnumerableSplitValueModularFromSize()
        {
            // Setup
            var entities = GetEntities(100).AsList();

            // Act
            var result = entities.Split(33);

            // Assert
            AssertRange(0, 32, entities, result.ElementAt(0));
            AssertRange(33, 65, entities, result.ElementAt(1));
            AssertRange(66, 98, entities, result.ElementAt(2));
            AssertRange(99, 99, entities, result.ElementAt(3));
        }

        #endregion
    }
}
