using Microsoft.Extensions.DependencyInjection;
using NotificationDatabase.Cache;
using NotificationDatabase.StartUpTasks.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationDatabase.StartUpTasks
{
    public class LoadCacheStartUpTask : IStartupTask
    {
        public IServiceProvider _provider;
        public LoadCacheStartUpTask(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _provider.CreateScope())
            {
                var cachedDict = scope.ServiceProvider
                    .GetRequiredService<CacheDictionaries>();

                await cachedDict.Populate();
            }
        }
    }
}
