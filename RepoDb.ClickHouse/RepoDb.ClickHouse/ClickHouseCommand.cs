using ClickHouse.Driver.ADO.Parameters;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ClickHouseCommand : ClickHouse.Driver.ADO.ClickHouseCommand
    {
        public ClickHouseCommand(
            ClickHouse.Driver.ADO.ClickHouseConnection connection)
            : base(connection)
        { }

        private void NormalizeParameterNames()
        {
            foreach (var parameter in Parameters)
            {
                if (parameter is ClickHouseDbParameter clickHouseParameter &&
                    clickHouseParameter.ParameterName?.StartsWith("@") == true)
                {
                    clickHouseParameter.ParameterName = clickHouseParameter.ParameterName.Substring(1);
                }
            }
        }

        public override int ExecuteNonQuery()
        {
            NormalizeParameterNames();
            return base.ExecuteNonQuery();
        }

        public override Task<int> ExecuteNonQueryAsync(
            CancellationToken cancellationToken)
        {
            NormalizeParameterNames();
            return base.ExecuteNonQueryAsync(cancellationToken);
        }

        public override object ExecuteScalar()
        {
            NormalizeParameterNames();
            return base.ExecuteScalar();
        }

        public override Task<object> ExecuteScalarAsync(
            CancellationToken cancellationToken)
        {
            NormalizeParameterNames();
            return base.ExecuteScalarAsync(cancellationToken);
        }

        protected override DbDataReader ExecuteDbDataReader(
            CommandBehavior behavior)
        {
            NormalizeParameterNames();
            return base.ExecuteDbDataReader(behavior);
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
        {
            NormalizeParameterNames();
            return await base.ExecuteDbDataReaderAsync(behavior, cancellationToken);
        }
    }
}
