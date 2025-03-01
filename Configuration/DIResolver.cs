using DPCV_API.BAL.Services.Activities;
using DPCV_API.BAL.Services.Committees;
using DPCV_API.BAL.Services.Districts;
using DPCV_API.BAL.Services.EntityCount;
using DPCV_API.BAL.Services.Events;
using DPCV_API.BAL.Services.Homestays;
using DPCV_API.BAL.Services.Products;
using DPCV_API.BAL.Services.SearchFilter;
using DPCV_API.BAL.Services.Users;
using DPCV_API.BAL.Services.Users.Auth;
using DPCV_API.Configuration.DbContext;

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
            services.AddScoped<IHomestayService, HomestayService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISearchfilterService, SearchfilterService>();
            services.AddScoped<IEntityService, EntityService>();

            return services;
        }
    }
}
