using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateAverage(string tableName, Field field, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateAverageAll(string tableName, Field field, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateBatchQuery(string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateCount(string tableName, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateCountAll(string tableName, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateDelete(string tableName, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateDeleteAll(string tableName, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateExists(string tableName, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateInsert(string tableName, IEnumerable<Field> fields = null, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateInsertAll(string tableName, IEnumerable<Field> fields = null, int batchSize = 10, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMax(string tableName, Field field, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMaxAll(string tableName, Field field, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMerge(string tableName, IEnumerable<Field> fields, IEnumerable<Field>? qualifiers = null, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMergeAll(string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMin(string tableName, Field field, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateMinAll(string tableName, Field field, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateQuery(string tableName, IEnumerable<Field> fields, QueryGroup? where = null, IEnumerable<OrderField> orderBy = null, int? top = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateQueryAll(string tableName, IEnumerable<Field> fields, IEnumerable<OrderField> orderBy = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateSkipQuery(string tableName, IEnumerable<Field> fields, int skip, int take, IEnumerable<OrderField> orderBy = null, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateSum(string tableName, Field field, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateSumAll(string tableName, Field field, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateTruncate(string tableName)
        {
            return string.Empty;
        }

        public string CreateUpdate(string tableName, IEnumerable<Field> fields, QueryGroup? where = null, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public string CreateUpdateAll(string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
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

        public override string CreateBatchQuery(string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup? where = null, string? hints = null)
        {
            return string.Empty;
        }

        public override string CreateMerge(string tableName, IEnumerable<Field> fields, IEnumerable<Field>? qualifiers = null, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public override string CreateMergeAll(string tableName, IEnumerable<Field> fields, IEnumerable<Field>? qualifiers = null, int batchSize = 10, DbField? primaryField = null, DbField? identityField = null, string? hints = null)
        {
            return string.Empty;
        }

        public override string CreateSkipQuery(string tableName, IEnumerable<Field> fields, int skip, int take, IEnumerable<OrderField> orderBy = null, QueryGroup? where = null, string? hints = null)
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
