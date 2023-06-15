namespace Db.Infrastructure.Migrations
{
    public class ConnectionModel
    {
        private string Server { get; init; }
        private string User { get; init; }
        private string Password { get; init; }
        private string Catalog { get; init; }

        public ConnectionModel(string server, string user, string password, string catalog)
        {
            Server = server;
            User = user;
            Password = password;
            Catalog = catalog;
        }

        public override string ToString()
        {
            return $"Server={Server};Initial Catalog={Catalog};Persist Security Info=False;User ID={User};Password={Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        }
    }
}
