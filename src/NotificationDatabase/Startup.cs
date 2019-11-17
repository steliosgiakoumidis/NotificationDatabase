using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NotificationDatabase.DatabaseLayer;
using NotificationDatabase.Model;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using NotificationDatabase.Cache;
using NotificationDatabase.StartupTasks.Plumbing;
using NotificationDatabase.StartUpTasks;
using System.Runtime.InteropServices;

namespace NotificationDatabase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var env = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ?
                        "Linux" : "Windows";
            var confBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json", true)
                    .AddEnvironmentVariables();
            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                            .CreateLogger(); 
            Configuration = confBuilder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("ConnectionString");
            services.AddDbContext<NotificationServiceContext>(options => 
                options.UseSqlServer("Server=192.168.0.29,1433;Database=NotificationService;User Id=SA;Password=Stelios2018!"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped(typeof(IDatabaseAccess<>), typeof(DatabaseAccess<>));
            services.AddScoped<CacheDictionaries>();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Notification Api", Version = "v1" });
                });
            services.AddStartupTask<LoadCacheStartUpTask>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/health");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI( s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Database"));
            app.UseMvc();

        }
    }
}
