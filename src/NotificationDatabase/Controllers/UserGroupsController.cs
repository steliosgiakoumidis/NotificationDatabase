using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationDatabase.Cache;
using NotificationDatabase.DatabaseLayer;
using NotificationDatabase.Model;
using NotificationCommon.Models;
using Serilog;

namespace NotificationDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private IDatabaseAccess<UserGroups> _database;
        public UserGroupsController(IDatabaseAccess<UserGroups> database, CacheDictionaries dictionaries)
        {
            var instatiated = dictionaries;
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGroups()
        {
            try
            {
                if (!CacheDictionaries.CachedUserGroups.IsEmpty) return Ok(CacheDictionaries.CachedUserGroups.Values.ToList());
                var records = await _database.GetAllRecords(new UserGroups());
                var userGroupsDto = records.Select(u => DbEntityDtoTransformer.UserGroupDbEntryToDto(u));
                foreach (var userGroup in userGroupsDto)
                {
                    if (!CacheDictionaries.CachedUserGroups.TryAdd(userGroup.Id, userGroup))
                    {
                        CacheDictionaries.CachedUserGroups.Clear();
                        throw new Exception("Caching mechanism failed to load all users");
                    }
                }
                return Ok(userGroupsDto);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract groups." +
                " Error: " + ex + " }");
                return Ok(new List<UserGroups>());
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleUSerGroup(string id)
        {
            try
            {
                UserGroup cachedGroup;
                if (!CacheDictionaries.CachedUserGroups.IsEmpty &&
                    CacheDictionaries.CachedUserGroups.TryGetValue(Convert.ToInt32(id), out cachedGroup))
                    return Ok(cachedGroup);
                var response = await _database.GetSingleRecord(new UserGroups(),
                 Convert.ToInt32(id));
                if (response == null) return BadRequest("User group cannot be found");
                return Ok(DbEntityDtoTransformer.UserGroupDbEntryToDto(response));
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single group." +
                " Error: " + ex + " }");
                return StatusCode(500, "An error has occured please try again");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserGroup([FromBody] UserGroup group)
        {
            try
            {
                UserGroup cachedGroup;
                await _database.DeleteItem(DbEntityDtoTransformer.UserGroupDtoToDbEntry(group));
                if (!CacheDictionaries.CachedUserGroups.IsEmpty &&
                    CacheDictionaries.CachedUserGroups.Remove(Convert.ToInt32(group.Id), out cachedGroup))
                    return Ok();
                CacheDictionaries.CachedUserGroups.Clear();
                return StatusCode(500, "An error may have occured when deleting user group. Please reload user groups");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete group." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddUserGroup([FromBody] UserGroup group)
        {
            try
            {
                group.Id = await _database.AddItem(DbEntityDtoTransformer.UserGroupDtoToDbEntry(group));
                if (CacheDictionaries.CachedUserGroups.TryAdd(Convert.ToInt32(group.Id), group))
                    return Ok();
                CacheDictionaries.CachedUserGroups.Clear();
                return StatusCode(500, "An error may have occured when adding user group. Please reload user groups");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to add group." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }

        }

        [HttpPut]
        public async Task<IActionResult> EditUserGroup([FromBody] UserGroup group)
        {
            try
            {
                UserGroup cachedGroup;
                await _database.EditItem(DbEntityDtoTransformer.UserGroupDtoToDbEntry(group));
                if (!CacheDictionaries.CachedUserGroups.IsEmpty &&
                    CacheDictionaries.CachedUserGroups.TryGetValue(group.Id, out cachedGroup) &&
                    CacheDictionaries.CachedUserGroups.TryUpdate(Convert.ToInt32(group.Id), group, cachedGroup))
                    return Ok();
                CacheDictionaries.CachedUserGroups.Clear();
                return StatusCode(500, "An error may have occured when editing user group. Please reload user groups");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit group." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }
        }
    }
}