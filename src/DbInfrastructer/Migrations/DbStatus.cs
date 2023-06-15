namespace Db.Infrastructure.Migrations
{
    public class DbStatus
    {
        public Status Status { get; internal set; }
    }

    public enum Status
    {
        New,
        Exist
    }
}
