using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NotificationDatabase.StartupTasks.Plumbing;
using Serilog;

namespace NotificationDatabase
{
    public class Program
    {
        public static async Task Main(string[] args)
        {       
            await CreateWebHostBuilder(args).Build().RunWithTaskAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5005")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseSerilog();    
                }
}
