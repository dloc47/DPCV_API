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

        public async Task<DataTable> ExecuteQueryAsync(string query, CommandType commandType)
        {
            DataTable dataTable = new DataTable();
            try
            {
                _command.CommandText = query;
                _command.CommandType = commandType;
                using (var adapter = new MySqlDataAdapter(_command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database query failed: {ex.Message}");
            }
            return dataTable;
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, CommandType commandType)
        {
            try
            {
                _command.CommandText = query;
                _command.CommandType = commandType;
                return await _command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database command failed: {ex.Message}");
            }
        }

        public void AddParameter(string paramName, object value)
        {
            _command.Parameters.AddWithValue(paramName, value);
        }

        public void ClearParameters()
        {
            _command.Parameters.Clear();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
