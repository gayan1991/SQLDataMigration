namespace Db.Infrastructure.Model
{
    public class DataVerficiationQueries
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public int? SourceCount { get; set; }
        public int? DestinationCount { get; set; }
        public DataVerificationModel VerificationModel { get; set; }

        public DataVerficiationQueries() { }

        public DataVerficiationQueries(DataVerificationModel model, string query) 
        { 
            VerificationModel = model;
            Query = query;
        }
    }
}
