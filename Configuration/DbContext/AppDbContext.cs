using MySql.Data.MySqlClient;
using System.Data;

namespace DPCV_API.Configuration.DbContext
{
    public class AppDbContext : IDisposable
    {
        private readonly MySqlConnection _connection;
        private MySqlCommand _command;
        private MySqlTransaction? _transaction;
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

        public void BeginTransaction()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            if (_transaction == null) // Prevent multiple transaction instances
            {
                _transaction = _connection.BeginTransaction();
                _command.Transaction = _transaction;
            }
        }


        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null; // Reset transaction
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void CloseContext()
        {
            _transaction?.Dispose();
            _command?.Dispose();

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
