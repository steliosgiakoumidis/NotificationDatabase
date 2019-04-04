using NotificationApi.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using NotificationApi.DatabaseLayer;
using NotificationApi.Model;
using Serilog;
using Serilog.Settings.Configuration;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;

namespace NotificationApi
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
            services.Configure<DatabaseInfo>((opt) => Configuration.GetSection("DatabaseInfo").Bind(opt));
            services.AddScoped(typeof(IDatabaseAccess<>), typeof(DatabaseAccess<>));
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Notification Api", Version = "v1" });
                });        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI( s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification api"));
            app.UseMvc();

        }
    }
}
