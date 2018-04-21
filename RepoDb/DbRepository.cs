using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using RepoDb.Exceptions;
using System.Reflection;
using RepoDb.Reflection;

namespace RepoDb
{
    public class DbRepository<TDbConnection> : IDbRepository<TDbConnection>
        where TDbConnection : DbConnection
    {
        private readonly string _connectionString;
        private readonly int? _commandTimeout;

        public DbRepository(string connectionString)
            : this(connectionString, null, null, null, null)
        {
        }

        public DbRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null)
        {
        }

        public DbRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

        public DbRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
        {
            // Fields
            _connectionString = connectionString;
            _commandTimeout = commandTimeout;

            // Properties
            Cache = (cache ?? new MemoryCache());
            Trace = trace;
            StatementBuilder = (statementBuilder ??
                StatementBuilderMapper.Get(typeof(TDbConnection))?.StatementBuilder ??
                new SqlDbStatementBuilder());
        }

        // CreateConnection (TDbConnection)

        public TDbConnection CreateConnection()
        {
            var connection = Activator.CreateInstance<TDbConnection>();
            connection.ConnectionString = _connectionString;
            return connection;
        }

        // DbCache

        public ICache Cache { get; }

        // Trace
        public ITrace Trace { get; }

        // StamentBuilder
        public IStatementBuilder StatementBuilder { get; }

        // GuardPrimaryKey
        private PropertyInfo GetAndGuardPrimaryKey<TEntity>()
            where TEntity : IDataEntity
        {
            var property = PrimaryPropertyCache.Get<TEntity>();
            if (property == null)
            {
                throw new PrimaryFieldNotFoundException($"{typeof(TEntity).FullName} ({ClassMapNameCache.Get<TEntity>()})");
            }
            return property;
        }

        // GuardBatchQueryable

        private void GuardBatchQueryable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsBatchQueryble<TEntity>())
            {
                throw new EntityNotBatchQueryableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // Count

        public int Count<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Count<TEntity>(where: (IQueryGroup)null,
                transaction: transaction);
        }

        public int Count<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>();
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return Count<TEntity>(where: queryGroup,
                transaction: transaction);
        }

        public int Count<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Count<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        public int Count<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardQueryable<TEntity>();

            // Variables
            var commandType = CommandTypeCache.Get<TEntity>();
            var commandText = commandType == CommandType.StoredProcedure ?
                ClassMapNameCache.Get<TEntity>() :
                StatementBuilder.CreateCount(QueryBuilderCache.Get<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Query);
                    }
                    return default(int);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = Convert.ToInt32(ExecuteScalar(commandText: commandText,
                 param: param,
                 commandType: commandType,
                 commandTimeout: _commandTimeout,
                 transaction: transaction));

            // After Execution
            if (Trace != null)
            {
                Trace.AfterCount(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // CountAsync

        public Task<int> CountAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(transaction: transaction));
        }

        public Task<int> CountAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> CountAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> CountAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(where: where,
                    transaction: transaction));
        }

        // CountBig

        public long CountBig<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return CountBig<TEntity>(where: (IQueryGroup)null,
                transaction: transaction);
        }

        public long CountBig<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>();
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return CountBig<TEntity>(where: queryGroup,
                transaction: transaction);
        }

        public long CountBig<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return CountBig<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        public long CountBig<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardQueryable<TEntity>();

            // Variables
            var commandType = CommandTypeCache.Get<TEntity>();
            var commandText = commandType == CommandType.StoredProcedure ?
                ClassMapNameCache.Get<TEntity>() :
                StatementBuilder.CreateCountBig(QueryBuilderCache.Get<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeCountBig(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Query);
                    }
                    return default(long);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = Convert.ToInt64(ExecuteScalar(commandText: commandText,
                 param: param,
                 commandType: commandType,
                 commandTimeout: _commandTimeout,
                 transaction: transaction));

            // After Execution
            if (Trace != null)
            {
                Trace.AfterCountBig(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // CountBigAsync

        public Task<long> CountBigAsync<TEntity>(IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                CountBig<TEntity>(transaction: transaction));
        }

        public Task<long> CountBigAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                CountBig<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<long> CountBigAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                CountBig<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<long> CountBigAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                CountBig<TEntity>(where: where,
                    transaction: transaction));
        }

        // BatchQuery

        public IEnumerable<TEntity> BatchQuery<TEntity>(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return BatchQuery<TEntity>(where: (IQueryGroup)null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is IQueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                queryGroup = QueryGroup.Parse(where);
            }
            return BatchQuery<TEntity>(where: queryGroup,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery<TEntity>(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return BatchQuery<TEntity>(where: new QueryGroup(where),
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        public IEnumerable<TEntity> BatchQuery<TEntity>(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardBatchQueryable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateBatchQuery(QueryBuilderCache.Get<TEntity>(), where, page, rowsPerBatch, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.BatchQuery);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteReader<TEntity>(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterBatchQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // BatchQueryAsync

        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        public Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    transaction: transaction));
        }

        // GuardQueryable

        private void GuardQueryable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsQueryable<TEntity>())
            {
                throw new EntityNotQueryableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // Query

        public IEnumerable<TEntity> Query<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(where: (IQueryGroup)null,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query<TEntity>(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>();
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return Query<TEntity>(where: queryGroup,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query<TEntity>(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = Cache?.Get(cacheKey);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item;
                }
            }

            // Check
            GuardQueryable<TEntity>();

            // Variables
            var commandType = CommandTypeCache.Get<TEntity>();
            var commandText = commandType == CommandType.StoredProcedure ?
                ClassMapNameCache.Get<TEntity>() :
                StatementBuilder.CreateQuery(QueryBuilderCache.Get<TEntity>(), where, top, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Query);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteReader<TEntity>(commandText: commandText,
                 param: param,
                 commandType: commandType,
                 commandTimeout: _commandTimeout,
                 transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null && result != null && result.Any())
            {
                Cache?.Add(cacheKey, result);
            }

            // Result
            return result;
        }

        // QueryAsync

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null,
            int? top = 0, IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(where: where,
                    transaction: transaction,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey));
        }

        // GuardInsertable

        private void GuardInsertable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsInsertable<TEntity>())
            {
                throw new EntityNotInsertableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // Insert

        public object Insert<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInsertable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateInsert(QueryBuilderCache.Get<TEntity>());
            var param = entity?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Insert);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalar(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InsertAsync
        public Task<object> InsertAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<object>(() =>
                Insert<TEntity>(entity: entity,
                    transaction: transaction));
        }

        // GuardUpdateable

        private void GuardUpdateable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsUpdateable<TEntity>())
            {
                throw new EntityNotUpdateableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // InlineUpdate

        public int InlineUpdate<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is IQueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                queryGroup = QueryGroup.Parse(where);
            }
            return InlineUpdate<TEntity>(entity: entity,
                where: queryGroup,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public int InlineUpdate<TEntity>(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return InlineUpdate<TEntity>(entity: entity,
                where: new QueryGroup(where),
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        public int InlineUpdate<TEntity>(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardUpdateable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateInlineUpdate(QueryBuilderCache.Get<TEntity>(),
                entity.AsFields(), where, overrideIgnore);
            var param = entity?.Merge(where);

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.InlineUpdate);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInlineUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineUpdateAsync

        public Task<int> InlineUpdateAsync<TEntity>(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        public Task<int> InlineUpdateAsync<TEntity>(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        public Task<int> InlineUpdateAsync<TEntity>(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    transaction: transaction));
        }

        // Update

        public int Update<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var property = GetAndGuardPrimaryKey<TEntity>();
            return Update(entity: entity,
                where: new QueryGroup(property.AsQueryField(entity).AsEnumerable()),
                transaction: transaction);
        }

        public int Update<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Update(entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        public int Update<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is IQueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var property = GetAndGuardPrimaryKey<TEntity>();
                    queryGroup = new QueryGroup(new QueryField(property.GetMappedName(), where).AsEnumerable());
                }
            }
            return Update(entity: entity,
                    where: queryGroup,
                    transaction: transaction);
        }

        public int Update<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardUpdateable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateUpdate(QueryBuilderCache.Get<TEntity>(), where);
            var param = entity?.AsObject(where);

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Update);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // UpdateAsync

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        // GuardDeletable

        private void GuardDeletable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsDeletable<TEntity>())
            {
                throw new EntityNotDeletableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // Delete

        public int Delete<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Delete<TEntity>(where: where != null ? new QueryGroup(where) : null,
                transaction: transaction);
        }

        public int Delete<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var queryGroup = (IQueryGroup)null;
            if (where is QueryField)
            {
                queryGroup = new QueryGroup(((IQueryField)where).AsEnumerable());
            }
            else if (where is IQueryGroup)
            {
                queryGroup = (IQueryGroup)where;
            }
            else if (where is TEntity)
            {
                var property = GetAndGuardPrimaryKey<TEntity>();
                queryGroup = new QueryGroup(property.AsQueryField(where).AsEnumerable());
            }
            else
            {
                queryGroup = QueryGroup.Parse(where);
            }
            return Delete<TEntity>(where: queryGroup,
                    transaction: transaction);
        }

        public int Delete<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateDelete(QueryBuilderCache.Get<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Delete);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // DeleteAsync

        public Task<int> DeleteAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> DeleteAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> DeleteAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        // GuardMergeable

        private void GuardMergeable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsMergeable<TEntity>())
            {
                throw new EntityNotMergeableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // Merge

        public int Merge<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Merge<TEntity>(entity: entity,
                qualifiers: null,
                    transaction: transaction);
        }

        public int Merge<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardMergeable<TEntity>();
            GetAndGuardPrimaryKey<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateMerge(QueryBuilderCache.Get<TEntity>(), qualifiers);
            var param = entity?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.Merge);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: CommandTypeCache.Get<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // MergeAsync
        public Task<int> MergeAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Merge<TEntity>(entity: entity,
                    transaction: transaction));
        }

        public Task<int> MergeAsync<TEntity>(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Merge<TEntity>(entity: entity,
                    qualifiers: qualifiers,
                    transaction: transaction));
        }

        // GuardBulkInsert

        private void GuardBulkInsert<TEntity>()
            where TEntity : IDataEntity
        {
            if (typeof(TDbConnection) != typeof(System.Data.SqlClient.SqlConnection))
            {
                throw new EntityNotBulkInsertableException(ClassMapNameCache.Get<TEntity>());
            }
        }

        // BulkInsert

        public int BulkInsert<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInsertable<TEntity>();
            GuardBulkInsert<TEntity>();

            // Variables
            using (var connection = (transaction?.Connection ?? CreateConnection()).EnsureOpen())
            {
                // Before Execution
                if (Trace != null)
                {
                    var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), Constant.BulkInsert, entities, null);
                    Trace.BeforeBulkInsert(cancellableTraceLog);
                    if (cancellableTraceLog.IsCancelled)
                    {
                        if (cancellableTraceLog.IsThrowException)
                        {
                            throw new CancelledExecutionException(Constant.BulkInsert);
                        }
                        return 0;
                    }
                }

                // Convert to table
                var table = entities.AsDataTable(connection);

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Actual Execution
                var sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)connection);
                var result = entities.Count();
                sqlBulkCopy.DestinationTableName = table.TableName;
                foreach (var column in table.Columns.OfType<DataColumn>())
                {
                    sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);

                // After Execution
                if (Trace != null)
                {
                    Trace.AfterBulkInsert(new TraceLog(MethodBase.GetCurrentMethod(), Constant.BulkInsert, table, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }

                // Result
                return result;
            }
        }

        // BulkInsertAsync
        public Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                BulkInsert(entities: entities,
                    transaction: transaction));
        }

        // ExecuteReader

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = (IEnumerable<TEntity>)null;
            using (var reader = connection.ExecuteReader(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: _commandTimeout,
                transaction: transaction,
                trace: Trace))
            {
                result = DataReaderMapper.ToEnumerable<TEntity>((DbDataReader)reader);
            }
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
        }

        // ExecuteReaderAsync

        public Task<IEnumerable<TEntity>> ExecuteReaderAsync<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                ExecuteReader<TEntity>(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        // ExecuteNonQuery

        public int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: _commandTimeout,
                transaction: transaction,
                trace: Trace);
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
        }

        // ExecuteNonQueryAsync

        public Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<int>(() =>
                ExecuteNonQuery(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        // ExecuteScalar

        public object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: _commandTimeout,
                transaction: transaction,
                trace: Trace);
            if (transaction == null)
            {
                connection.Dispose();
            }
            return result;
        }

        // ExecuteScalarAsync

        public Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<object>(() =>
                ExecuteScalar(commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }
    }
}