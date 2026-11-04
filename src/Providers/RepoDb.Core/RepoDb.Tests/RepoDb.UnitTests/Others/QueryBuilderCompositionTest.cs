#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class QueryBuilderCompositionTest
    {
        [TestMethod]
        public void TestBuilderDeleteSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Delete().GetString();

            // Assert
            Assert.AreEqual("statement DELETE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderEndSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.End().GetString();

            // Assert
            Assert.AreEqual("statement ;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderInsertSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Insert().GetString();

            // Assert
            Assert.AreEqual("statement INSERT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderIntoSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Into().GetString();

            // Assert
            Assert.AreEqual("statement INTO", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderValuesSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Values().GetString();

            // Assert
            Assert.AreEqual("statement VALUES", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderFromSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.From().GetString();

            // Assert
            Assert.AreEqual("statement FROM", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderSelectSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Select().GetString();

            // Assert
            Assert.AreEqual("statement SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderTopSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Top().GetString();

            // Assert
            Assert.AreEqual("statement TOP", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderLimitSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Limit().GetString();

            // Assert
            Assert.AreEqual("statement LIMIT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOffsetSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Offset().GetString();

            // Assert
            Assert.AreEqual("statement OFFSET", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOrderBySeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.OrderBy().GetString();

            // Assert
            Assert.AreEqual("statement ORDER BY", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderWhereSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Where().GetString();

            // Assert
            Assert.AreEqual("statement WHERE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderUpdateSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Update().GetString();

            // Assert
            Assert.AreEqual("statement UPDATE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderUsingSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Using().GetString();

            // Assert
            Assert.AreEqual("statement USING", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderRowNumberSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.RowNumber().GetString();

            // Assert
            Assert.AreEqual("statement ROW_NUMBER()", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOverSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Over().GetString();

            // Assert
            Assert.AreEqual("statement OVER", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAndSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.And().GetString();

            // Assert
            Assert.AreEqual("statement AND", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOrSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Or().GetString();

            // Assert
            Assert.AreEqual("statement OR", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOpenParenSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.OpenParen().GetString();

            // Assert
            Assert.AreEqual("statement (", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCloseParenSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.CloseParen().GetString();

            // Assert
            Assert.AreEqual("statement )", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOnSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.On().GetString();

            // Assert
            Assert.AreEqual("statement ON", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderInSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.In().GetString();

            // Assert
            Assert.AreEqual("statement IN", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderBetweenSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Between().GetString();

            // Assert
            Assert.AreEqual("statement BETWEEN", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderWhenSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.When().GetString();

            // Assert
            Assert.AreEqual("statement WHEN", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderNotSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Not().GetString();

            // Assert
            Assert.AreEqual("statement NOT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMatchedSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Matched().GetString();

            // Assert
            Assert.AreEqual("statement MATCHED", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderThenSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Then().GetString();

            // Assert
            Assert.AreEqual("statement THEN", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCaseSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Case().GetString();

            // Assert
            Assert.AreEqual("statement CASE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderTruncateSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Truncate().GetString();

            // Assert
            Assert.AreEqual("statement TRUNCATE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAvgSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Avg().GetString();

            // Assert
            Assert.AreEqual("statement AVG", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAscendingSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Ascending().GetString();

            // Assert
            Assert.AreEqual("statement ASC", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderDescendingSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Descending().GetString();

            // Assert
            Assert.AreEqual("statement DESC", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderReplaceSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Replace().GetString();

            // Assert
            Assert.AreEqual("statement REPLACE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderReturningSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Returning().GetString();

            // Assert
            Assert.AreEqual("statement RETURNING", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderConflictSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Conflict().GetString();

            // Assert
            Assert.AreEqual("statement CONFLICT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOnConflictSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.OnConflict().GetString();

            // Assert
            Assert.AreEqual("statement ON CONFLICT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderDoSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Do().GetString();

            // Assert
            Assert.AreEqual("statement DO", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderDoNothingSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.DoNothing().GetString();

            // Assert
            Assert.AreEqual("statement DO NOTHING", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderDoUpdateSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.DoUpdate().GetString();

            // Assert
            Assert.AreEqual("statement DO UPDATE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderGroupBySeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.GroupBy().GetString();

            // Assert
            Assert.AreEqual("statement GROUP BY", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderHavingCountSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.HavingCount().GetString();

            // Assert
            Assert.AreEqual("statement HAVING COUNT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAsSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.As().GetString();

            // Assert
            Assert.AreEqual("statement AS", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderWithSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.With().GetString();

            // Assert
            Assert.AreEqual("statement WITH", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderSetSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Set().GetString();

            // Assert
            Assert.AreEqual("statement SET", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderJoinSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Join().GetString();

            // Assert
            Assert.AreEqual("statement JOIN", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMergeSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Merge().GetString();

            // Assert
            Assert.AreEqual("statement MERGE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderTableSeparatesExistingText()
        {
            // Setup
            var builder = new QueryBuilder().WriteText("statement");

            // Act
            var actual = builder.Table().GetString();

            // Assert
            Assert.AreEqual("statement TABLE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderNewLine()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.NewLine().From().GetString();

            // Assert
            Assert.AreEqual("SELECT" + Environment.NewLine + " FROM", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAverageField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.Average(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("AVG ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMinField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.Min(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("MIN ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMaxField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.Max(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("MAX ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderSumField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.Sum(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SUM ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCountField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.Count(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("COUNT ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCountBigField()
        {
            // Setup
            var builder = new QueryBuilder();
            var field = new Field("Id");

            // Act
            var actual = builder.CountBig(field, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("COUNT_BIG ([Id])", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCountAllFields()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Count(null, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("COUNT (*)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderCountBigAllFields()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.CountBig(null, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("COUNT_BIG (*)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMaxFromField()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.MaxFrom(new Field("Id")).GetString();

            // Assert
            Assert.AreEqual("MAX(Id)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderMinFromField()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.MinFrom(new Field("Id")).GetString();

            // Assert
            Assert.AreEqual("MIN(Id)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAvgFromField()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.AvgFrom(new Field("Id")).GetString();

            // Assert
            Assert.AreEqual("AVG(Id)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderFieldsFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.FieldsFrom<CoverageEntity>(setting).GetString();

            // Assert
            Assert.AreEqual("[Id], [Name]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderTableNameFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.TableNameFrom<CoverageEntity>(setting).GetString();

            // Assert
            Assert.AreEqual("[CoverageEntity]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderParametersFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.ParametersFrom<CoverageEntity>(0, setting).GetString();

            // Assert
            Assert.AreEqual("@Id, @Name", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderFieldsAndParametersFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.FieldsAndParametersFrom<CoverageEntity>(0, setting).GetString();

            // Assert
            Assert.AreEqual("[Id] = @Id, [Name] = @Name", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderParametersAsFieldsFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.ParametersAsFieldsFrom<CoverageEntity>(0, setting).GetString();

            // Assert
            Assert.AreEqual("@Id AS [Id], @Name AS [Name]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderFieldsAndAliasFieldsFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.FieldsAndAliasFieldsFrom<CoverageEntity>("left", "right", setting).GetString();

            // Assert
            Assert.AreEqual("left.[Id] = right.[Id], left.[Name] = right.[Name]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderAliasFieldsFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.AsAliasFieldsFrom<CoverageEntity>("left", setting).GetString();

            // Assert
            Assert.AreEqual("left.[Id], left.[Name]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderFieldFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.FieldFrom(new Field("Id"), setting).GetString();

            // Assert
            Assert.AreEqual("[Id]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderGroupByFromMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.GroupByFrom(new[] { new Field("Id") }, setting).GetString();

            // Assert
            Assert.AreEqual("GROUP BY [Id]", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBuilderOnConflictMapsFields()
        {
            // Setup
            var builder = new QueryBuilder();
            var setting = new CustomDbSetting();

            // Act
            var actual = builder.OnConflict(new[] { new Field("Id") }, setting).GetString();

            // Assert
            Assert.AreEqual("ON CONFLICT ([Id])", actual, StringComparer.Ordinal);
        }

    }
}
