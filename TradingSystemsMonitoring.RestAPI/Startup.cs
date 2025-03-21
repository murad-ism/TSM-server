using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.DbContext.Infrastructure;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Services;
using TradingSystemsMonitoring.RestAPI.Services.Handlers;
using TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver;


namespace TradingSystemsMonitoring.RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            TradingDataDbSettings.ReadConfiguration(Configuration);
            TradingLogRecordsDbSettings.ReadConfiguration(Configuration);
            
            services.AddDbContext<TsmUsersDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("UsersDbConection")));

            services.AddDbContext<TradingDataDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TradingDataDbConnection")));

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
                    builder
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials()
                         .SetIsOriginAllowed(host => true);
                }));

            services.AddSingleton<ITrackingDataReceiver, TrackingDataReceiver>();
            services.AddHostedService<TrackingDataHandler>();
            services.AddSingleton<TsmExceptionHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trading system monitoring API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                var exHandler = app.ApplicationServices.GetRequiredService<TsmExceptionHandler>();
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = ctx => exHandler.HandleException(ctx),
                });
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading system monitoring API V1");
            });

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TradingDataMonitoringHub>("/api/tradingDataMonitoring");
            });

#if DEBUG
            //UserDbInitializer.SeedDataAsync(Configuration, userManager, roleManager).Wait();
#endif

        }
    }
}
