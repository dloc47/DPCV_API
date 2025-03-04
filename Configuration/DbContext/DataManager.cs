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
        /// Executes a SQL query and returns the result as a DataTable.
        /// Use this when you need to retrieve multiple rows and columns.
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string query, CommandType commandType)
        {
            DataTable dataTable = new DataTable();
            try
            {
                _command.CommandText = query;
                _command.CommandType = commandType;
                using (var adapter = new MySqlDataAdapter(_command))
                {
                    await Task.Run(() => adapter.Fill(dataTable)); // Fills DataTable with query results
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
        /// Use this when making changes to the database without expecting a result set.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string query, CommandType commandType)
        {
            try
            {
                _command.CommandText = query;
                _command.CommandType = commandType;
                return await _command.ExecuteNonQueryAsync() > 0; // Returns true if rows are affected
            }
            catch (Exception ex)
            {
                throw new Exception($"Database command failed: {ex.Message}");
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
                _command.CommandText = query;
                _command.CommandType = commandType;
                return (MySqlDataReader)await _command.ExecuteReaderAsync(); // Returns a data reader for iteration
            }
            catch (Exception ex)
            {
                throw new Exception($"Database reader failed: {ex.Message}");
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
                _command.CommandText = query;
                _command.CommandType = commandType;
                object? result = await _command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return default; // Return default value for the type (null for reference types, 0 for int, etc.)

                return (T)Convert.ChangeType(result, typeof(T)); // Convert result to the specified type
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
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
