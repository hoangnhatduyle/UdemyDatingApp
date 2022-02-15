using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            services.AddScoped<ITokenService, TokenService>();  
            //add a service to create token and inject to our application. This is scoped to the lifttime of the Http Request
            
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}