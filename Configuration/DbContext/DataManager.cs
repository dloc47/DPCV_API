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
                using (var command = _dbContext.GetCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dataTable)); // Fill DataTable
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
        /// Use this when making changes to the database without expecting a result set.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string query, CommandType commandType)
        {
            try
            {
                using (var command = _dbContext.GetCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;
                    return await command.ExecuteNonQueryAsync() > 0; // Execute query
                }
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
                var command = _dbContext.GetCommand(); // Don't use `using` to keep reader open
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
        /// Use this when you need just one value from the database.
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
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
