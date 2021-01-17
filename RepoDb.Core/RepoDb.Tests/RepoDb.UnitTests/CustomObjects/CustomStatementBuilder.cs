using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateAverage(QueryBuilder queryBuilder, string tableName, Field field, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateAverageAll(QueryBuilder queryBuilder, string tableName, Field field, string hints = null)
        {
            return string.Empty;
        }

        public string CreateBatchQuery(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateCount(QueryBuilder queryBuilder, string tableName, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateCountAll(QueryBuilder queryBuilder, string tableName, string hints = null)
        {
            return string.Empty;
        }

        public string CreateDelete(QueryBuilder queryBuilder, string tableName, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateDeleteAll(QueryBuilder queryBuilder, string tableName, string hints = null)
        {
            return string.Empty;
        }

        public string CreateExists(QueryBuilder queryBuilder, string tableName, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateInsert(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields = null, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateInsertAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields = null, int batchSize = 10, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMax(QueryBuilder queryBuilder, string tableName, Field field, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMaxAll(QueryBuilder queryBuilder, string tableName, Field field, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMerge(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMergeAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMin(QueryBuilder queryBuilder, string tableName, Field field, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateMinAll(QueryBuilder queryBuilder, string tableName, Field field, string hints = null)
        {
            return string.Empty;
        }

        public string CreateQuery(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, QueryGroup where = null, IEnumerable<OrderField> orderBy = null, int? top = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateQueryAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<OrderField> orderBy = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateSum(QueryBuilder queryBuilder, string tableName, Field field, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateSumAll(QueryBuilder queryBuilder, string tableName, Field field, string hints = null)
        {
            return string.Empty;
        }

        public string CreateTruncate(QueryBuilder queryBuilder, string tableName)
        {
            return string.Empty;
        }

        public string CreateUpdate(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, QueryGroup where = null, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public string CreateUpdateAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }
    }

    public class CustomBaseStatementBuilder : BaseStatementBuilder
    {
        public CustomBaseStatementBuilder()
            : this(new CustomDbSetting(), null, null)
        {
        }

        public CustomBaseStatementBuilder(IDbSetting dbSetting)
            : this(dbSetting, null, null)
        {
        }

        public CustomBaseStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  convertFieldResolver,
                  averageableClientTypeResolver)
        {}

        public override string CreateBatchQuery(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup where = null, string hints = null)
        {
            return string.Empty;
        }

        public override string CreateMerge(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }

        public override string CreateMergeAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, int batchSize = 10, DbField primaryField = null, DbField identityField = null, string hints = null)
        {
            return string.Empty;
        }
    }

    public class CustomNonHintsSupportingBaseStatementBuilder : CustomBaseStatementBuilder
    {
        public CustomNonHintsSupportingBaseStatementBuilder()
            : base(new CustomNonHintsSupportingDbSetting())
        { }
    }

    public class CustomSingleStatementSupportBaseStatementBuilder : CustomBaseStatementBuilder
    {
        public CustomSingleStatementSupportBaseStatementBuilder()
            : base(new CustomSingleStatementSupportDbSetting())
        { }
    }

    public class CustomDefinedBaseStatementBuilder : CustomBaseStatementBuilder
    {
        public CustomDefinedBaseStatementBuilder()
            : base(new CustomDbSetting(),
                new SqlServerConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver())
        { }
    }
}
