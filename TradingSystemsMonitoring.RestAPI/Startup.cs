using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TradingSystemsMonitoring.DataModel.Identity;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Services;
using TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver;

namespace TradingSystemsMonitoring.RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TsmUsersDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("UsersConection")));
            

            var builder = services.AddIdentityCore<TsmUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddRoles<TsmRole>();
            identityBuilder.AddEntityFrameworkStores<TsmUsersDbContext>();
            identityBuilder.AddSignInManager<SignInManager<TsmUser>>();
           
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                        };
                        /*
                        opt.Events = new JwtBearerEvents()
                        {
                            OnTokenValidated = context =>
                            {
                                var userid = context.Principal.Claims.Where(x => x.Type == "userid").FirstOrDefault().Value;
                                if (true)
                                {
                                    context.Fail("invaild token");
                                }
                                return Task.CompletedTask;
                            }
                        };*/

                    });
            
            services.AddControllers(option =>
            {
                // Отключаем маршрутизацию конечных точек на основе endpoint-based logic из EndpointMiddleware
                // и продолжаем использование маршрутизации на основе IRouter. 
                option.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSignalR();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                }));

            services.AddHostedService<TrackingDataHandler>();

            services.AddSingleton<ITrackingDataReceiver, TrackingDataReceiver>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            UserManager<TsmUser> userManager, RoleManager<TsmRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseCors("AllowCors");
            app.UseCors("CorsPolicy");
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TradingDataMonitoringHub>("/tradingDataMonitoring");
            });
            
#if DEBUG
            UserDbInitializer.SeedDataAsync(Configuration, userManager, roleManager).Wait();
#endif

        }
    }
}
