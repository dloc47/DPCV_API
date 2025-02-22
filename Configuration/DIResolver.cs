using DPCV_API.BAL.Services.CMS.Auth;
using DPCV_API.BAL.Services.CMS.User;
using DPCV_API.BAL.Services.Website.Activities;
using DPCV_API.BAL.Services.Website.Committees;
using DPCV_API.BAL.Services.Website.EntityCount;
using DPCV_API.BAL.Services.Website.Events;
using DPCV_API.BAL.Services.Website.Homestays;
using DPCV_API.BAL.Services.Website.Products;
using DPCV_API.BAL.Services.Website.SearchFilter;
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
