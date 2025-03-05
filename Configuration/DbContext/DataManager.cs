using MySql.Data.MySqlClient;
using System.Data;

namespace DPCV_API.Configuration.DbContext
{
    public class DataManager : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private MySqlCommand _command;

        public DataManager()
        {
            _dbContext = new AppDbContext();
            _dbContext.OpenContext();
            _command = _dbContext.GetCommand();
        }

        /// <summary>
        /// Executes a SQL query and returns the result as a DataTable.
        /// Use this when you need to retrieve multiple rows and columns.
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string query, CommandType commandType)
        {
            try
            {
                using var command = CreateCommand(query, commandType);
                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                await Task.Run(() => adapter.Fill(dataTable));
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a SQL command (INSERT, UPDATE, DELETE) and returns true if successful.
        /// Use this when making changes to the database without expecting a result set.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string query, CommandType commandType)
        {
            try
            {
                using var command = CreateCommand(query, commandType);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database command failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and returns a MySqlDataReader for streaming data row by row.
        /// Use this when fetching large datasets for better performance.
        /// </summary>
        public async Task<MySqlDataReader> ExecuteReaderAsync(string query, CommandType commandType)
        {
            try
            {
                var command = CreateCommand(query, commandType);
                return (MySqlDataReader)await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database reader failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and returns a single value of type T (e.g., int, string).
        /// Use this when you need just one value from the database.
        /// </summary>
        public async Task<T?> ExecuteScalarAsync<T>(string query, CommandType commandType)
        {
            try
            {
                using var command = CreateCommand(query, commandType);
                object? result = await command.ExecuteScalarAsync();
                return (result == null || result == DBNull.Value) ? default : (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"Database scalar query failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds a parameter to the command.
        /// Must be called before executing a query.
        /// </summary>
        public void AddParameter(string paramName, object value)
        {
            _command.Parameters.AddWithValue(paramName, value ?? DBNull.Value);
        }

        /// <summary>
        /// Clears all parameters from the command.
        /// Call this before executing a new query with different parameters.
        /// </summary>
        public void ClearParameters()
        {
            _command.Parameters.Clear();
        }

        /// <summary>
        /// Creates a new command with the stored parameters.
        /// Ensures that parameters are applied correctly.
        /// </summary>
        private MySqlCommand CreateCommand(string query, CommandType commandType)
        {
            var command = _dbContext.GetCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            foreach (MySqlParameter param in _command.Parameters)
            {
                command.Parameters.Add(new MySqlParameter(param.ParameterName, param.Value));
            }
            return command;
        }

        /// <summary>
        /// Disposes of database resources.
        /// </summary>
        public void Dispose()
        {
            _command?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
