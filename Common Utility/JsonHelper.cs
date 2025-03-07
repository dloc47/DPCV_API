using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DPCV_API.Helpers
{
    public class JsonHelper
    {
        private static readonly ILogger<JsonHelper> _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<JsonHelper>();

        public static T DeserializeJsonSafely<T>(object dbValue, string fieldName)
        {
            if (dbValue == DBNull.Value || dbValue == null || string.IsNullOrWhiteSpace(dbValue.ToString()))
            {
                return Activator.CreateInstance<T>(); // Return an empty object of the required type
            }

            try
            {
                return JsonSerializer.Deserialize<T>(dbValue.ToString()!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing {fieldName}: {ex.Message}");
                return Activator.CreateInstance<T>(); // Return an empty object on failure
            }
        }
    }
}
