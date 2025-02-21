using MySql.Data.MySqlClient;
using System.Data;

namespace DPCV_API.Configuration.DbContext
{
    public class AppDbContext : IDisposable
    {
        private readonly MySqlConnection _connection;
        private MySqlCommand _command;
        private bool _disposed = false;

        public AppDbContext()
        {
            var config = new AppConfiguration();
            _connection = new MySqlConnection(config.ConnectionString);
        }

        public void OpenContext()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            _command = _connection.CreateCommand();
        }

        public void CloseContext()
        {
            if (_command != null)
                _command.Dispose();

            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }

        public MySqlCommand GetCommand() => _command;

        public void Dispose()
        {
            if (!_disposed)
            {
                CloseContext();
                _connection.Dispose();
                _disposed = true;
            }
        }
    }
}
