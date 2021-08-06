namespace RepoDb.PostgreSql.IntegrationTests.Models
{
    public class EnumTable
    {

        public System.Int64 Id { get; set; }
        public Hands? ColumnEnum { get; set; }
        public enum Hands
        {
            Unidentified,
            Left,
            Right
        }
    }

}
