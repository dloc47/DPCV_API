namespace DPCV_API.Configuration.DbContext
{
    public class AppConfiguration
    {
        private readonly IConfigurationRoot _configuration;
        public string ConnectionString { get; }

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }
    }
}
