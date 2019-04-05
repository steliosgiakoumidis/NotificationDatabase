using System.Collections.Concurrent;
using NotificationApi.Model;

namespace NotificationApi.Cache
{
    public interface ICacheLists
    {
         ConcurrentDictionary<int, Users> CachedUsers { get; set; }
         ConcurrentDictionary<int, UserGroups> CachedUserGroups { get; set; }
         ConcurrentDictionary<int, Templates> CachedTemplates { get; set; }
         ConcurrentDictionary<int, RegularSendout> CachedSendouts { get; set; }
    }
}