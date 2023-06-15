namespace Db.Infrastructure.Model
{
    public class MigrationDataModel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTimeOffset LastUpdatedDateTime { get; set; }

        public MigrationDataModel() { }

        public MigrationDataModel(string tableName)
        {
            TableName = tableName;
            LastUpdatedDateTime = DateTimeOffset.UtcNow;
        }
    }
}
