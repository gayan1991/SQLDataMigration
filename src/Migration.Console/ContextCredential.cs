using Db.Infrastructure.DatabaseServices.Interface;
using Db.Infrastructure.Enums;
using Db.Infrastructure.Migrations;
using Migration.Core;
using Migration.Core.Interface;

namespace Migration.Console
{
    public class ContextCredential : IContextCredential
    {
        private readonly IMigrationLogger _logger;
        private KeyValuePair<string, string> _credential;

        public ContextCredential(IMigrationLogger logger) => _logger = logger;

        public ConnectionModel GetCredential(MigrationConfiguration config, DbEnd dbEnd)
        {
            if (string.IsNullOrWhiteSpace(_credential.Key) || string.IsNullOrWhiteSpace(_credential.Key))
            {
                ExtractCredentials();
            }
            return config.ToConnectionModel(dbEnd, _credential.Key, _credential.Value);
        }

        #region Private

        private void ExtractCredentials()
        {
            _logger.WriteLine(@"Please Enter Login : ", false);
            var user = _logger.ReadLine();

            if (string.IsNullOrWhiteSpace(user))
            {
                ShowInvalidCrentialsError();
                return;
            }

            _logger.WriteLine(@"Password: ", false);
            var password = _logger.ReadSecret();

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowInvalidCrentialsError();
                return;
            }
            else
                _credential = new KeyValuePair<string, string>(user, password);
        }

        private void ShowInvalidCrentialsError()
        {
            _logger.WriteLine("Invalid Crentials!", false);
            _logger.WriteLine("Do you want to exit the console? Press Y and Enter to exit", false);
            var response = _logger.ReadLine();

            if (response != null && response.ToUpper() == "Y")
            {
                Environment.Exit(0);
            }
            else
                ExtractCredentials();
        }

        #endregion
    }
}
