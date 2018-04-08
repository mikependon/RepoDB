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
            StatementBuilder = (statementBuilder ?? new SqlDbStatementBuilder());
        }

        // CreateConnection

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

        // QueryBuilder
        public IStatementBuilder StatementBuilder { get; }

        // GuardPrimaryKey
        private PropertyInfo GetAndGuardPrimaryKey<TEntity>()
            where TEntity : IDataEntity
        {
            var primaryKey = DataEntityExtension.GetPrimaryProperty<TEntity>();
            if (primaryKey == null)
            {
                throw new PrimaryFieldNotFoundException($"{typeof(TEntity).FullName} ({DataEntityExtension.GetMappedName<TEntity>()})");
            }
            return primaryKey;
        }

        // GuardQueryable

        private void GuardQueryable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsQueryable<TEntity>())
            {
                throw new EntityNotQueryableException(DataEntityExtension.GetMappedName<TEntity>());
            }
        }

        // Query

        public IEnumerable<TEntity> Query<TEntity>(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(top: top,
                orderFields: orderFields,
                where: (IQueryGroup)null,
                transaction: transaction,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0, 
            IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Query<TEntity>(where: where != null ? new QueryGroup(where) : (IQueryGroup)null,
                top: top,
                orderFields: orderFields,
                transaction: transaction,
                cacheKey: cacheKey);
        }

        public IEnumerable<TEntity> Query<TEntity>(object where, IDbTransaction transaction = null, int? top = 0, 
            IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            if (where is QueryField)
            {
                return Query<TEntity>(where: ((IQueryField)where).AsEnumerable(),
                    top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey);
            }
            else if (where is IQueryGroup)
            {
                return Query<TEntity>(where: (IQueryGroup)where,
                    top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey);
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    return Query<TEntity>(where: QueryGroup.Parse(where),
                        top: top,
                        orderFields: orderFields,
                        transaction: transaction,
                        cacheKey: cacheKey);
                }
                else
                {
                    var primaryKey = GetAndGuardPrimaryKey<TEntity>();
                    return Query<TEntity>(where: new QueryField(primaryKey.Name, where).AsEnumerable(),
                        top: top,
                        orderFields: orderFields,
                        transaction: transaction,
                        cacheKey: cacheKey);
                }
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(IQueryGroup where, IDbTransaction transaction = null, int? top = 0, 
            IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
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
            var commandType = DataEntityExtension.GetCommandType<TEntity>();
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>() : 
                StatementBuilder.CreateQuery<TEntity>(where, top, orderFields);
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

            // Actual Execution
            var result = ExecuteReader<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
            }

            // Set Cache
            if (cacheKey != null && result != null && result.Any())
            {
                Cache?.Set(cacheKey, result);
            }

            // Result
            return result;
        }
        
        // QueryAsync

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IDbTransaction transaction = null, int? top = 0, 
            IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                Query<TEntity>(top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null, 
            int? top = 0, IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                Query<TEntity>(where: where,
                    top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(object where, IDbTransaction transaction = null, 
            int? top = 0, IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                Query<TEntity>(where: where,
                    top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey));
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null, 
            int? top = 0, IEnumerable<IOrderField> orderFields = null, string cacheKey = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                Query<TEntity>(where: where,
                    top: top,
                    orderFields: orderFields,
                    transaction: transaction,
                    cacheKey: cacheKey));
        }

        // GuardInsertable

        private void GuardInsertable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsInsertable<TEntity>())
            {
                throw new EntityNotInsertableException(DataEntityExtension.GetMappedName<TEntity>());
            }
        }

        // Insert

        public object Insert<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInsertable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateInsert<TEntity>();
            var param = entity?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
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

            // Actual Execution
            var result = ExecuteScalar(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
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
                throw new EntityNotUpdateableException(DataEntityExtension.GetMappedName<TEntity>());
            }
        }

        // Update

        public int Update<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var primary = GetAndGuardPrimaryKey<TEntity>();
            return Update<TEntity>(entity: entity,
                where: primary?.AsQueryField(entity).AsEnumerable(),
                transaction: transaction);
        }

        public int Update<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Update(entity: entity,
                where: where != null ? new QueryGroup(where) : (IQueryGroup)null,
                transaction: transaction);
        }

        public int Update<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            if (where is IQueryField)
            {
                return Update<TEntity>(entity: entity,
                    where: ((IQueryField)where).AsEnumerable(),
                    transaction: transaction);
            }
            else if (where is IQueryGroup)
            {
                return Update<TEntity>(entity: entity,
                    where: (IQueryGroup)where,
                    transaction: transaction);
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    return Update<TEntity>(entity: entity,
                        where: QueryGroup.Parse(where),
                        transaction: transaction);
                }
                else
                {
                    var primaryKey = GetAndGuardPrimaryKey<TEntity>();
                    return Update<TEntity>(entity: entity,
                        where: new QueryField(primaryKey.Name, where).AsEnumerable(),
                        transaction: transaction);
                }
            }
        }

        public int Update<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardUpdateable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateUpdate<TEntity>(where);
            var param = entity?.AsObject(where);

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                Trace.BeforeUpdate(cancellableTraceLog);
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

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
            }

            // Result
            return result;
        }

        // UpdateAsync
        public Task<int> UpdateAsync<TEntity>(TEntity entity, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Update(entity: entity,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Update(entity: entity,
                    where: where,
                    transaction: transaction));
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
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
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>());
            }
        }

        // Delete

        public int Delete<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Delete<TEntity>(where: where != null ? new QueryGroup(where) : (IQueryGroup)null,
                transaction: transaction);
        }

        public int Delete<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            if (where is QueryField)
            {
                return Delete<TEntity>(where: ((IQueryField)where).AsEnumerable(),
                    transaction: transaction);
            }
            else if (where is IQueryGroup)
            {
                return Delete<TEntity>(where: (IQueryGroup)where,
                    transaction: transaction);
            }
            else if (where is TEntity)
            {
                var primaryKey = GetAndGuardPrimaryKey<TEntity>();
                return Delete<TEntity>(where: new QueryField(primaryKey.Name, primaryKey.GetValue(where)).AsEnumerable(),
                    transaction: transaction);
            }
            else
            {
                if ((bool)where?.GetType().IsGenericType)
                {
                    return Delete<TEntity>(where: QueryGroup.Parse(where),
                        transaction: transaction);
                }
                else
                {
                    var primaryKey = GetAndGuardPrimaryKey<TEntity>();
                    return Delete<TEntity>(where: new QueryField(primaryKey.Name, where).AsEnumerable(),
                        transaction: transaction);
                }
            }
        }

        public int Delete<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletable<TEntity>();

            // Variables
            var commandText = StatementBuilder.CreateDelete<TEntity>(where);
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

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(),
                commandTimeout: _commandTimeout);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
            }

            // Result
            return result;
        }

        // DeleteAsync

        public Task<int> DeleteAsync<TEntity>(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> DeleteAsync<TEntity>(object where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        public Task<int> DeleteAsync<TEntity>(IQueryGroup where, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Delete<TEntity>(where: where,
                    transaction: transaction));
        }

        // GuardMergeable

        private void GuardMergeable<TEntity>()
            where TEntity : IDataEntity
        {
            if (!DataEntityExtension.IsMergeable<TEntity>())
            {
                throw new EntityNotMergeableException(DataEntityExtension.GetMappedName<TEntity>());
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
            var commandText = StatementBuilder.CreateMerge<TEntity>(qualifiers);
            var param = entity?.AsObject();

            // Before Execution
            if (Trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
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

            // Actual Execution
            var result = ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: DataEntityExtension.GetCommandType<TEntity>(),
                commandTimeout: _commandTimeout,
                transaction: transaction);

            // After Execution
            if (Trace != null)
            {
                Trace.AfterMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
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
                throw new EntityNotBulkInsertableException(DataEntityExtension.GetMappedName<TEntity>());
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
                var table = entities.AsDataTable<TEntity>(connection);

                // Actual Execution
                var sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)connection);
                var result = entities.Count();
                sqlBulkCopy.DestinationTableName = table.TableName;
                sqlBulkCopy.WriteToServer(table);

                // After Execution
                if (Trace != null)
                {
                    Trace.AfterBulkInsert(new TraceLog(MethodBase.GetCurrentMethod(), Constant.BulkInsert, table, result));
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
                BulkInsert<TEntity>(entities: entities,
                    transaction: transaction));
        }

        // ExecuteReader

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var connection = (transaction?.Connection ?? CreateConnection());
            var result = connection.ExecuteReader<TEntity>(commandText: commandText,
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

        // ExecuteReaderAsync

        public Task<IEnumerable<TEntity>> ExecuteReaderAsync<TEntity>(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
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