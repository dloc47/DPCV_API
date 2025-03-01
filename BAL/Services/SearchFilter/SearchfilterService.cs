using System.Data;
using DPCV_API.Configuration.DbContext;

namespace DPCV_API.BAL.Services.SearchFilter
{
    public class SearchfilterService : ISearchfilterService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<SearchfilterService> _logger;

        public SearchfilterService(DataManager dataManager, ILogger<SearchfilterService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> GetFilteredDataAsync(
            string category, int? districtId, int? villageId, string? searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    _logger.LogWarning("Category is required.");
                    throw new ArgumentException("Category is required.");
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_category", category);
                _dataManager.AddParameter("@p_district_id", districtId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_village_id", villageId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_search", string.IsNullOrWhiteSpace(searchTerm) ? DBNull.Value : searchTerm);

                // Execute stored procedure
                var dataTable = await _dataManager.ExecuteQueryAsync("FilterData", CommandType.StoredProcedure);

                if (dataTable.Rows.Count == 0)
                {
                    _logger.LogInformation("No records found for category: {Category}", category);
                    return new List<Dictionary<string, object>>();
                }

                // Convert DataTable to List of Dictionaries
                var result = dataTable.AsEnumerable()
                    .Select(row => dataTable.Columns.Cast<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col] is DBNull ? null : row[col] as object))
                    .ToList();

                _logger.LogInformation("Filter query executed successfully for category: {Category}", category);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering data for category: {Category}", category);
                return new List<Dictionary<string, object>>();
            }
        }
    }
}
