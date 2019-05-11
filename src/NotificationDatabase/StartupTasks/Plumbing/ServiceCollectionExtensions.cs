﻿using Microsoft.Extensions.DependencyInjection;
using NotificationDatabase.StartUpTasks.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationDatabase.StartupTasks.Plumbing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }
}
