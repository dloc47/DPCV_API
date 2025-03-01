namespace DPCV_API.BAL.Services.SearchFilter
{
    public interface ISearchfilterService
    {
        Task<List<Dictionary<string, object>>> GetFilteredDataAsync(string category, int? districtId, int? villageId, string? searchTerm);
    }
}
