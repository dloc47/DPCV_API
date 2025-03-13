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

        // Constructor initializes the database connection using the connection string from AppConfiguration.
        public AppDbContext()
        {
            var config = new AppConfiguration();
            _connection = new MySqlConnection(config.ConnectionString);
        }

        // Opens the database connection and initializes the command object.
        public void OpenContext()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            _command = _connection.CreateCommand();
        }

        // Begins a new database transaction.
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

        // Commits the current transaction, saving all changes made since the transaction began.
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null; // Reset transaction
        }

        // Rolls back the current transaction, undoing all changes made since the transaction began.
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

        // Closes the database connection and disposes of the command and transaction objects.
        public void CloseContext()
        {
            _transaction?.Dispose();
            _command?.Dispose();

            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }

        // Returns the current MySqlCommand object.
        public MySqlCommand GetCommand() => _command;

        // Implements IDisposable to ensure proper cleanup of resources.
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