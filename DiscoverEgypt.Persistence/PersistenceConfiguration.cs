// Member 6 summary: persistence bootstrapping configures ASP.NET Identity with int user and role keys.
using DiscoverEgypt.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoverEgypt.Persistence
{
    public static class PersistenceConfiguration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DiscoverEgyptDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));

            services
           .AddIdentity<ApplicationUser, IdentityRole<int>>()
           .AddEntityFrameworkStores<DiscoverEgyptDbContext>()
           .AddDefaultTokenProviders();


            return services;
        }
    }
}
