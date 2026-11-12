#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using RepoDb.Exceptions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public class DbJsonDialectMapperTest
    {
        #region Get

        [TestMethod]
        public void TestDbJsonDialectMapperGetWithNullSetting()
        {
            // Act
            var dialect = DbJsonDialectMapper.Get(null);

            // Assert
            Assert.AreSame(DefaultDbJsonDialect.Instance, dialect);
        }

        [TestMethod]
        public void TestDbJsonDialectMapperGetWithUnmappedSetting()
        {
            // Act
            var dialect = DbJsonDialectMapper.Get(new CustomDbSetting());

            // Assert
            Assert.AreSame(DefaultDbJsonDialect.Instance, dialect);
        }

        [TestMethod]
        public void TestDbJsonDialectMapperGetWithMappedSetting()
        {
            // Setup
            var mapped = new CustomDbJsonDialect();
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(mapped, force: true);

            try
            {
                // Act
                var dialect = DbJsonDialectMapper.Get(new CustomJsonDbSetting());

                // Assert
                Assert.AreSame(mapped, dialect);
            }
            finally
            {
                DbJsonDialectMapper.Remove<CustomJsonDbSetting>();
            }
        }

        [TestMethod]
        public void TestDbJsonDialectMapperGetWithDerivedSetting()
        {
            // Setup
            var mapped = new CustomDbJsonDialect();
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(mapped, force: true);

            try
            {
                // Act
                var dialect = DbJsonDialectMapper.Get(new CustomJsonDerivedDbSetting());

                // Assert
                Assert.AreSame(mapped, dialect);
            }
            finally
            {
                DbJsonDialectMapper.Remove<CustomJsonDbSetting>();
            }
        }

        #endregion

        #region Add

        [TestMethod]
        public void TestDbJsonDialectMapperAddWithExistingMapping()
        {
            // Setup
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(new CustomDbJsonDialect(), force: true);

            try
            {
                // Act and Assert
                Assert.Throws<MappingExistsException>(() => DbJsonDialectMapper.Add<CustomJsonDbSetting>(new CustomDbJsonDialect(), force: false));
            }
            finally
            {
                DbJsonDialectMapper.Remove<CustomJsonDbSetting>();
            }
        }

        [TestMethod]
        public void TestDbJsonDialectMapperAddWithForce()
        {
            // Setup
            var replacement = new CustomDbJsonDialect();
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(new CustomDbJsonDialect(), force: true);

            try
            {
                // Act
                DbJsonDialectMapper.Add<CustomJsonDbSetting>(replacement, force: true);

                // Assert
                Assert.AreSame(replacement, DbJsonDialectMapper.Get(new CustomJsonDbSetting()));
            }
            finally
            {
                DbJsonDialectMapper.Remove<CustomJsonDbSetting>();
            }
        }

        #endregion

        #region Remove

        [TestMethod]
        public void TestDbJsonDialectMapperRemove()
        {
            // Setup
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(new CustomDbJsonDialect(), force: true);

            // Act
            DbJsonDialectMapper.Remove<CustomJsonDbSetting>();

            // Assert
            Assert.AreSame(DefaultDbJsonDialect.Instance, DbJsonDialectMapper.Get(new CustomJsonDbSetting()));
        }

        #endregion

        #region Clear

        [TestMethod]
        public void TestDbJsonDialectMapperClear()
        {
            // Setup
            DbJsonDialectMapper.Add<CustomJsonDbSetting>(new CustomDbJsonDialect(), force: true);

            // Act
            DbJsonDialectMapper.Clear();

            // Assert
            Assert.AreSame(DefaultDbJsonDialect.Instance, DbJsonDialectMapper.Get(new CustomJsonDbSetting()));
        }

        #endregion
    }
}
