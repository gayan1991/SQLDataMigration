namespace Migration.Core.Interface
{
    public interface IMigration
    {
        Task MigrateAsync(MigrationConfiguration connection);
    }
}
