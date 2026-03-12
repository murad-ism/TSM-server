using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using TradingSystemsMonitoring.Data.Abstractions;
using TradingSystemsMonitoring.Data.Services;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.DbContext.Factories;
using TradingSystemsMonitoring.DataModel.DbContext.Settings;
using TradingSystemsMonitoring.DataModel.Entities.Identity;
using TradingSystemsMonitoring.RestAPI.Settings;
using TradingSystemsMonitoring.RestAPI.Abstractions;
using TradingSystemsMonitoring.RestAPI.Abstractions.Identity;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Services.Handlers;
using TradingSystemsMonitoring.RestAPI.Services.Identity;
using TradingSystemsMonitoring.RestAPI.Services.TradingData;


namespace TradingSystemsMonitoring.RestAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            TradingDataDbSettings.ReadConfiguration(Configuration);
            TsmUsersDbSettings.ReadConfiguration(Configuration);
            TradingLogRecordsDbSettings.ReadConfiguration(Configuration);
            RedisDbSettings.ReadConfiguration(Configuration);
            KafkaSettings.ReadConfiguration(Configuration);

            services.AddScoped<IDbContextFactory<TradingDataDbContext>, TradingDataDbContextFactory>();
            services.AddScoped<ITradingLogRecordDbFactory, TradingLogRecordDbFactory>();
            services.AddScoped<IAccountClosedTradesService, AccountClosedTradesService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITradingLogsExplorerService, TradingLogsExplorerService>();

            ConfigureIdentity(services);
            services.AddControllers(option =>
            {
                // Use IRouter-based routing instead of endpoint-based routing.
                option.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSignalR();
            services.AddSingleton<ITradingDataSubscriber, NetMqDataSubscriber>();
            services.AddHostedService<TradingLiveDataStreamer>();
            services.AddSingleton<TsmExceptionHandler>();

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(RedisDbSettings.ConnectionString));

            services.AddSingleton(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaTradeConsumer>>();
                return new KafkaTradeConsumer(KafkaSettings.Instance, redis, logger);
            });

            services.AddHostedService<KafkaConsumerHostedService>();
            
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed(host => true);
                }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trading system monitoring API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            var builder = services.AddIdentityCore<TsmUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            services.AddSingleton<IDbContextFactory<TsmUsersDbContext>, TsmUsersDbContextFactory>();
            services.AddScoped<TsmUsersDbContext>(sp => sp.GetRequiredService<IDbContextFactory<TsmUsersDbContext>>().CreateDbContext());

            identityBuilder.AddRoles<TsmRole>();
            identityBuilder.AddEntityFrameworkStores<TsmUsersDbContext>();
            identityBuilder.AddSignInManager<SignInManager<TsmUser>>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<ITsmUsersService, TsmUsersService>();
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Identity:TokenKey"]));
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var exHandler = app.ApplicationServices.GetRequiredService<TsmExceptionHandler>();
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = ctx => exHandler.HandleException(ctx),
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading system monitoring API V1");
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TradingDataMonitoringHub>("/api/tradingDataMonitoring");
            });
        }
    }
}
