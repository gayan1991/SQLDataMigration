namespace Db.Infrastructure.DatabaseServices.Interface
{
    public interface IMigrationLogger
    {
        string? ReadLine();
        string ReadSecret();
        void WriteLine(string message, bool log = true);
    }
}
