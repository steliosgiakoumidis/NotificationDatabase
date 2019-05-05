using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationDatabase.DatabaseLayer;
using NotificationDatabase.Model;
using NotificationCommon.Models;
using Serilog;

namespace NotificationDatabase.Cache
{
    public class CacheDictionaries
    {
        private IDatabaseAccess<Templates> _dbTemplates;
        private IDatabaseAccess<RegularSendout> _dbSendouts;
        private IDatabaseAccess<Users> _dbUsers;
        private IDatabaseAccess<UserGroups> _dbUserGroups;
        public static ConcurrentDictionary<int, User> CachedUsers { get; set; }
            = new ConcurrentDictionary<int, User>();
        public static ConcurrentDictionary<int, UserGroup> CachedUserGroups { get; set; }
            = new ConcurrentDictionary<int, UserGroup>();
        public static ConcurrentDictionary<int, Template> CachedTemplates { get; set; }
            = new ConcurrentDictionary<int, Template>();
        public static ConcurrentDictionary<int, Sendout> CachedSendouts { get; set; }
            = new ConcurrentDictionary<int, Sendout>();

        public CacheDictionaries(IDatabaseAccess<Templates> dbTemplates,
            IDatabaseAccess<RegularSendout> dbSendouts, IDatabaseAccess<Users> dbUsers,
            IDatabaseAccess<UserGroups> dbUserGroups)
        {
            _dbSendouts = dbSendouts;
            _dbTemplates = dbTemplates;
            _dbUserGroups = dbUserGroups;
            _dbUsers = dbUsers;
            if (CachedSendouts.IsEmpty) PopulateSendoutsDictionary().Wait();
            if (CachedTemplates.IsEmpty) PopulateTemplatesDictionary().Wait();
            if (CachedUserGroups.IsEmpty) PopulateUserGroupsDictionary().Wait();
            if (CachedUsers.IsEmpty) PopulateUsersDictionary().Wait();
        }

        private async Task PopulateUsersDictionary()
        {
            try
            {
                var records = await _dbUsers.GetAllRecords(new Users());
                var userDto = records.Select(u => DbEntityDtoTransformer.UserDbEntryToDto(u));
                foreach (var user in userDto)
                {
                    if (!CachedUsers.TryAdd(user.Id, user))
                    {
                        CachedUsers.Clear();
                        throw new Exception("Caching mechanism failed to load all Users");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to extract all users." +
                " Error: " + ex + " }");
            }
        }

        private async Task PopulateUserGroupsDictionary()
        {
            try
            {
                var records = await _dbUserGroups.GetAllRecords(new UserGroups());
                var userGroupDto = records.Select(u => DbEntityDtoTransformer.UserGroupDbEntryToDto(u));
                foreach (var userGroup in userGroupDto)
                {
                    if (!CachedUserGroups.TryAdd(userGroup.Id, userGroup))
                    {
                        CachedUserGroups.Clear();
                        throw new Exception("Caching mechanism failed to load all users");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract groups." +
                " Error: " + ex + " }");
            }
        }

        private async Task PopulateTemplatesDictionary()
        {
            try
            {
                var records = await _dbTemplates.GetAllRecords(new Templates());
                var templatesDto = records.Select(t => DbEntityDtoTransformer.TemplateDbEntityToDto(t));
                foreach (var template in templatesDto)
                {
                    if (!CachedTemplates.TryAdd(template.Id, template))
                    {
                        CachedTemplates.Clear();
                        throw new Exception("Caching mechanism failed to load all Users");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract templates." +
                " Error: " + ex + " }");
            }
        }

        private async Task PopulateSendoutsDictionary()
        {
            try
            {
                var records = await _dbSendouts.GetAllRecords(
                    new RegularSendout());
                var templatesDto = records.Select(r => DbEntityDtoTransformer.SendoutDbEntityToDto(r));
                foreach (var sendout in templatesDto)
                {
                    if (!CachedSendouts.TryAdd(sendout.Id, sendout))
                    {
                        CachedSendouts.Clear();
                        throw new Exception("Caching mechanism failed to load all sendouts");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured when loading in memory the sendout lists. Error: {ex}");
            }
        }
    }
}