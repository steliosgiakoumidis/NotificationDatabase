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

namespace NotificationDatabase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var confBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
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
            services.AddDbContext<NotificationServiceContext>(options => 
                options.UseSqlServer("Server=stelios\\sqlexpress;Database=NotificationService;Trusted_Connection=True;"));
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
