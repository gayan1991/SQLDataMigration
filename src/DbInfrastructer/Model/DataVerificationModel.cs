namespace Db.Infrastructure.Model
{
    public class DataVerificationModel
    {
        public int Id { get; set; }
        public string TableName { get; set; } = null!;
        public List<DataVerficiationQueries> Queries { get; set; } = new();

        public DataVerificationModel() { }

        public DataVerificationModel(string tableName, string schema = "dbo")
        {
            TableName = $"{schema ?? "dbo"}.{tableName}";
            Queries = new List<DataVerficiationQueries>
            {
                new DataVerficiationQueries(this, $"Select Count(*) as [RecordCount] From [{schema ?? "dbo"}].[{tableName}]")
            };
        }

        public void AddVerificationQuery(string query)
        {
            Queries.Add(new DataVerficiationQueries(this, query));
        }
    }
}
