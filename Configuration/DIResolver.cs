using DPCV_API.BAL.Services.Auth;
using DPCV_API.BAL.Services.Webiste.Committees;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Services;
using DPCV_API.Services.DistrictService;

namespace DPCV_API.Configuration
{
    public static class DIResolver
    {
        public static IServiceCollection DIBALResolver(this IServiceCollection services)
        {
            services.AddScoped<DataManager>(); // Register DataManager for DI

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<ICommitteeService, CommitteeService>();

            return services;
        }
    }
}
