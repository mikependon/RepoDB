namespace RepoDb.SqlServer.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps (by convention) to the [dbo].[TriggerCompatibilityTable] table,
    /// which is created with an enabled AFTER INSERT trigger. Used to prove that InsertAll/Merge/
    /// MergeAll no longer throw the SQL Server "OUTPUT clause without INTO" error (#trigger-bug)
    /// on tables that have enabled triggers.
    /// </summary>
    public class TriggerCompatibilityTable
    {
        public System.Int32 Id { get; set; }
        public System.String Name { get; set; }
    }
}
