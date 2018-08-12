using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests.Comparers
{
    [TestClass]
    public class DynamicPropertiesComparerTest
    {
        [TestMethod]
        public void TestSinglePropertyWithTheSameValues()
        {
            // Prepare
            var objA = new { Id = 1 };
            var objB = new { Id = 1 };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSinglePropertyWithDifferentValues()
        {
            // Prepare
            var objA = new { Id = 1 };
            var objB = new { Id = 2 };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMultiplePropertiesWithTheSameValues()
        {
            // Prepare
            var objA = new { Id = 1, Name = "A" };
            var objB = new { Id = 1, Name = "A" };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMultiplePropertiesWithDifferentValues()
        {
            // Prepare
            var objA = new { Id = 1, Name = "A" };
            var objB = new { Id = 2, Name = "B" };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestTwoDifferentSinglePropertiesWithTheSameValues()
        {
            // Prepare
            var objA = new { IdA = 1 };
            var objB = new { IdB = 1 };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestTwoDifferentSinglePropertiesWithDifferentValues()
        {
            // Prepare
            var objA = new { IdA = 1 };
            var objB = new { IdB = 2 };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMultipleDifferentPropertiesPropertiesWithTheSameValues()
        {
            // Prepare
            var objA = new { IdA = 1, NameA = "A" };
            var objB = new { IdB = 1, NameB = "A" };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMultipleDifferentPropertiesPropertiesWithDifferentValues()
        {
            // Prepare
            var objA = new { IdA = 1, NameA = "A" };
            var objB = new { IdB = 2, NameB = "B" };

            // Act
            var actual = DynamicComparer.ArePropertiesEqual(objA, objB);

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
