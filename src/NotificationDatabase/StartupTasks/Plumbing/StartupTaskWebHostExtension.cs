using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NotificationDatabase.StartUpTasks.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationDatabase.StartupTasks.Plumbing
{
    public static class StartupTaskWebHostExtension
    {
        public static async Task RunWithTaskAsync(this IWebHost webhost,
            CancellationToken token = default)
        {
            var startupTasks = webhost.Services.GetServices<IStartupTask>();
            foreach (var task in startupTasks)
            {
                await task.ExecuteAsync(token);
            }
            await webhost.RunAsync(token);
        }
    }
}
