using System;
using System.Collections.Concurrent;
using NotificationApi.DatabaseLayer;
using NotificationApi.Model;

namespace NotificationApi.Cache
{
    public class CacheDictionaries
    {
        public ConcurrentDictionary<int, Users> CachedUsers {get;set;}
            = new ConcurrentDictionary<int, Users>();
        public ConcurrentDictionary<int, UserGroups> CachedUserGroups {get;set;}
            = new ConcurrentDictionary<int, UserGroups>();
        public ConcurrentDictionary<int, Templates> CachedTemplates {get;set;}
            = new ConcurrentDictionary<int, Templates>();
        public ConcurrentDictionary<int, RegularSendout> CachedSendouts {get;set;} 
            = new ConcurrentDictionary<int, RegularSendout>();
    }
}