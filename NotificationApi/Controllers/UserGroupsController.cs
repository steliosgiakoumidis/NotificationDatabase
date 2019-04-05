using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationApi.Cache;
using NotificationApi.DatabaseLayer;
using NotificationApi.Model;
using Serilog;

namespace NotificationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private IDatabaseAccess<UserGroups> _database;
        private ConcurrentDictionary<int, UserGroups> _cache;
        public UserGroupsController(IDatabaseAccess<UserGroups> database, CacheLists cache)
        {
            _database = database;
            _cache = cache.CachedUserGroups;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGroups(){
            try
            {
                if(!_cache.IsEmpty) return Ok(_cache.Values.ToList());
                var response = await _database.GetAllRecords(new UserGroups());
                if(response.Count() == 0)
                {
                    return BadRequest("No user groups found");
                }
                foreach (var userGroup in response)
                {
                    if(!_cache.TryAdd(userGroup.Id, userGroup))
                    {
                        _cache.Clear();
                        throw new Exception("Caching mechanism failed to load all users");
                    } 
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract groups." +
                " Error: "+ ex +" }");
                return Ok(new List<UserGroups>());
            }     
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleUSerGroup(string id)
        {
            try
            {
                UserGroups cachedGroup;
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(Convert.ToInt32(id), out cachedGroup)) 
                    return Ok(cachedGroup); 
                var response = _database.GetSingleRecord(new UserGroups(),
                 Convert.ToInt32(id));
                if(response == null) return BadRequest("User group cannot be found");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single group." +
                " Error: "+ ex +" }");
                return StatusCode(500, "An error has occured please try again");                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserGroup([FromBody] UserGroups group){
            try
            {
                UserGroups cachedGroup;
                await _database.DeleteItem(group);
                if(!_cache.IsEmpty && 
                    _cache.Remove(Convert.ToInt32(group.Id), out cachedGroup)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when deleting user group. Please reload user groups");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete group." +
                " Error: "+ ex +" }");
                return StatusCode(500);            
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddUserGroup([FromBody] UserGroups group){
            try
            {
                await _database.AddItem(group);
                if(!_cache.IsEmpty && 
                    _cache.TryAdd(Convert.ToInt32(group.Id), group)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when adding user group. Please reload user groups");
            }
            catch (Exception ex) 
            {
                Log.Error($"An error reaching the db to add group." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }

        }

        [HttpPut]
        public async Task<IActionResult> EditUserGroup([FromBody] UserGroups group){
            try
            {
                UserGroups cachedGroup;
                await _database.EditItem(group);
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(group.Id, out cachedGroup) &&
                    _cache.TryUpdate(Convert.ToInt32(group.Id), group, cachedGroup)) 
                    return Ok();   
                return StatusCode(500, "An error may have occured when editing user group. Please reload user groups");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit group." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }
    }
}