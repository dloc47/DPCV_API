using MySql.Data.MySqlClient;
using System.Data;

namespace DPCV_API.Configuration.DbContext
{
    public class DataManager
    {
        private readonly AppDbContext _dbContext;
        private readonly MySqlCommand _command;

        public DataManager()
        {
            _dbContext = new AppDbContext();
            _dbContext.OpenContext();
            _command = _dbContext.GetCommand();
        }

        /// <summary>
        /// Begins a new database transaction.
        /// Use this when executing multiple database operations that need to be atomic.
        /// If any operation fails, changes can be rolled back to maintain data integrity.
        /// </summary>
        public void BeginTransaction()
        {
            _dbContext.BeginTransaction();
        }

        /// <summary>
        /// Commits the current transaction.
        /// This permanently saves all changes made since the last BeginTransaction() call.
        /// Use this only after all operations have successfully completed.
        /// </summary>
        public void CommitTransaction()
        {
            _dbContext.CommitTransaction();
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// This undoes all database changes made since the last BeginTransaction() call.
        /// Use this when an error occurs to ensure no partial or inconsistent data is saved.
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (_dbContext != null)
                {
                    _dbContext.RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executes a SQL query and returns the result as a DataTable.
        /// Use this when you need to retrieve multiple rows and columns from the database.
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string query, CommandType commandType)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var command = _dbContext.GetCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dataTable)); // Fill DataTable asynchronously
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query failed: {ex.Message}");
            }
            return dataTable;
        }

        /// <summary>
        /// Executes a SQL command (INSERT, UPDATE, DELETE) and returns true if successful.
        /// Use this when modifying data in the database without expecting a result set.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string query, CommandType commandType)
        {
            try
            {
                using (var command = _dbContext.GetCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;
                    return await command.ExecuteNonQueryAsync() > 0; // Execute command and check success
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database command failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a SQL query and returns a MySqlDataReader for streaming data row by row.
        /// Use this when fetching large datasets for better performance and memory efficiency.
        /// </summary>
        public async Task<MySqlDataReader> ExecuteReaderAsync(string query, CommandType commandType)
        {
            try
            {
                var command = _dbContext.GetCommand(); // Don't use `using` to keep the reader open
                command.CommandText = query;
                command.CommandType = commandType;
                return (MySqlDataReader)await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database reader failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a SQL query and returns a single value of type T (e.g., int, string).
        /// Use this when retrieving a single value from the database.
        /// </summary>
        public async Task<T?> ExecuteScalarAsync<T>(string query, CommandType commandType)
        {
            try
            {
                using (var command = _dbContext.GetCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;
                    object? result = await command.ExecuteScalarAsync();
                    return (result == null || result == DBNull.Value) ? default : (T)Convert.ChangeType(result, typeof(T));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database scalar query failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a parameter to the SQL command to prevent SQL injection.
        /// Use this before executing a query that requires parameters.
        /// </summary>
        public void AddParameter(string paramName, object value)
        {
            _command.Parameters.AddWithValue(paramName, value);
        }

        /// <summary>
        /// Clears all parameters added to the SQL command.
        /// Use this to reset parameters before executing a new query.
        /// </summary>
        public void ClearParameters()
        {
            _command.Parameters.Clear();
        }

        /// <summary>
        /// Disposes of database resources when no longer needed.
        /// Ensures proper cleanup of the database connection and command objects.
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
