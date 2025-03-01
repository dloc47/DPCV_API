using DPCV_API.BAL.Services.EntityCount;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.EntityCount;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

public class EntityService : IEntityService
{
    private readonly DataManager _dataManager;
    private readonly ILogger<EntityService> _logger;

    public EntityService(DataManager dataManager, ILogger<EntityService> logger)
    {
        _dataManager = dataManager;
        _logger = logger;
    }

    public async Task<EntityCountDTO> GetEntityCountsAsync()
    {
        try
        {
            string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM committees) AS VillageCommunities,
                    (SELECT COUNT(*) FROM homestays) AS HomeStays,
                    (SELECT COUNT(*) FROM activities) AS ThingsToDo,
                    (SELECT COUNT(*) FROM products) AS LocalProducts,
                    (SELECT COUNT(*) FROM events) AS Events";

            DataTable resultTable = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);

            if (resultTable.Rows.Count > 0)
            {
                var row = resultTable.Rows[0];
                return new EntityCountDTO
                {
                    VillageCommunities = Convert.ToInt32(row["VillageCommunities"]),
                    HomeStays = Convert.ToInt32(row["HomeStays"]),
                    ThingsToDo = Convert.ToInt32(row["ThingsToDo"]),
                    LocalProducts = Convert.ToInt32(row["LocalProducts"]),
                    Events = Convert.ToInt32(row["Events"])
                };
            }

            return new EntityCountDTO();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching entity counts.");
            throw new ApplicationException("An error occurred while retrieving entity counts. Please try again later.");
        }
    }
}
