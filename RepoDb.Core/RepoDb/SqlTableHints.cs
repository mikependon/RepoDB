namespace RepoDb
{
    /// <summary>
    /// A class that contains the SQL Server table hints (query optimizers) when querying a record. See Microsoft documentation <see href="https://docs.microsoft.com/en-us/sql/t-sql/queries/hints-transact-sql-table?view=sql-server-2017">here</see>.
    /// </summary>
    public static class SqlTableHints
    {
        /// <summary>
        /// Specifies that any indexed views are not expanded to access underlying tables when the query optimizer processes the query.
        /// </summary>
        public const string NoExpand = "WITH (NOEXPAND)";

        /// <summary>
        /// The query optimizer considers only index seek operations to access the table or view through any relevant index.
        /// </summary>
        public const string ForceSeek = "WITH (FORCESEEK)";

        /// <summary>
        /// Specifies that the query optimizer use only an index scan operation as the access path to the referenced table or view. 
        /// </summary>
        public const string ForceScan = "WITH (FORCESCAN)";

        /// <summary>
        /// HOLDLOCK applies only to the table or view for which it is specified and only for the duration of the transaction 
        /// defined by the statement that it is used in. Is equivalent to SERIALIZABLE.
        /// </summary>
        public const string HoldLock = "WITH (HOLDLOCK)";

        /// <summary>
        /// Specifies that dirty reads are allowed. No shared locks are issued to prevent other transactions from modifying data 
        /// read by the current transaction, and exclusive locks set by other transactions do not block the current transaction 
        /// from reading the locked data. Is equivalent to READUNCOMMITTED.
        /// </summary>
        public const string NoLock = "WITH (NOLOCK)";

        /// <summary>
        /// Instructs the Database Engine to return a message as soon as a lock is encountered on the table.
        /// </summary>
        public const string NoWait = "WITH (NOWAIT)";

        /// <summary>
        /// Takes page locks either where individual locks are ordinarily taken on rows or keys, or where a single table lock is ordinarily taken.
        /// </summary>
        public const string PagLock = "WITH (PAGLOCK)";

        /// <summary>
        /// Specifies that read operations comply with the rules for the READ COMMITTED isolation level by using either locking or row versioning. 
        /// </summary>
        public const string ReadCommitted = "WITH (READCOMMITTED)";

        /// <summary>
        /// Specifies that read operations comply with the rules for the READ COMMITTED isolation level by using locking.
        /// </summary>
        public const string ReadCommittedLock = "WITH (READCOMMITTEDLOCK)";

        /// <summary>
        /// Specifies that the Database Engine not read rows that are locked by other transactions.
        /// </summary>
        public const string ReadPast = "WITH (READPAST)";

        /// <summary>
        /// Specifies that dirty reads are allowed. No shared locks are issued to prevent other transactions from modifying data 
        /// read by the current transaction, and exclusive locks set by other transactions do not block the current transaction 
        /// from reading the locked data.
        /// </summary>
        public const string ReadUncommitted = "WITH (READUNCOMMITTED)";

        /// <summary>
        /// Specifies that a scan is performed with the same locking semantics as a transaction running at REPEATABLE READ isolation level.
        /// </summary>
        public const string RepeatableRead = "WITH (REPEATABLEREAD)";

        /// <summary>
        /// Specifies that row locks are taken when page or table locks are ordinarily taken. When specified in transactions operating at the SNAPSHOT 
        /// isolation level, row locks are not taken unless ROWLOCK is combined with other table hints that require locks, such as UPDLOCK and HOLDLOCK.
        /// </summary>
        public const string RowLock = "WITH (ROWLOCK)";

        /// <summary>
        /// Makes shared locks more restrictive by holding them until a transaction is completed, instead of releasing the 
        /// shared lock as soon as the required table or data page is no longer needed, whether the transaction has been completed or not. The scan is 
        /// performed with the same semantics as a transaction running at the SERIALIZABLE isolation level. Is equivalent to HOLDLOCK.
        /// </summary>
        public const string Serializable = "WITH (SERIALIZABLE)";

        /// <summary>
        /// The memory-optimized table is accessed under SNAPSHOT isolation. SNAPSHOT can only be used with memory-optimized tables (not with disk-based tables).
        /// Applies to: SQL Server 2014 (12.x) through SQL Server 2017.
        /// </summary>
        public const string Snapshot = "WITH (SNAPSHOT)";

        /// <summary>
        /// Specifies that the acquired lock is applied at the table level. The type of lock that is acquired depends on the statement being executed.
        /// </summary>
        public const string TabLock = "WITH (TABLOCK)";

        /// <summary>
        /// Specifies that an exclusive lock is taken on the table.
        /// </summary>
        public const string TabLockX = "WITH (TABLOCKX)";

        /// <summary>
        /// Specifies that update locks are to be taken and held until the transaction completes.
        /// </summary>
        public const string UpdLock = "WITH (UPDLOCK)";

        /// <summary>
        /// Specifies that exclusive locks are to be taken and held until the transaction completes.
        /// </summary>
        public const string XLock = "WITH (XLOCK)";
    }
}
