using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbTransaction : DbTransaction, IDbTransaction
    {
        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnection DbConnection { get; }

        public override void Commit()
        {
            /* do nothing */
        }

        public new void Dispose()
        {
            /* do nothing */
        }

        public override void Rollback()
        {
            /* do nothing */
        }
    }
}
