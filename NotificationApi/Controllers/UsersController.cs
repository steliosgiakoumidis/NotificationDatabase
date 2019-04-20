using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NotificationApi.Cache;
using NotificationApi.Configuration;
using NotificationApi.DatabaseLayer;
using NotificationApi.Model;
using Serilog;

namespace NotificationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IDatabaseAccess<Users> _database;
        private ConcurrentDictionary<int, Users> _cache;

        public UsersController(IDatabaseAccess<Users> database, CacheDictionaries cache)
        {
            _database = database;
            _cache = cache.CachedUsers;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            try
            {
                if(!_cache.IsEmpty) return Ok(_cache.Values.ToList());
                var response =  await _database.GetAllRecords(new Users());
                foreach (var user in response)
                {
                    if(!_cache.TryAdd(user.Id, user)) 
                    {
                        _cache.Clear();
                        throw new Exception("Caching mechanism failed to load all Users"); 
                    }               
                }          
                return Ok(response);
                
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to extract all users." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }        
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleUSer(string id)
        {
            try
            {
                Users cachedUser;
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(Convert.ToInt32(id), out cachedUser)) 
                    return Ok(cachedUser); 
                var response =  await _database.GetSingleRecord(new Users(), Convert.ToInt32(id));
                if (response == null) return BadRequest("User cannot be found");
                return Ok(response);                     
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single user." +
                " Error: "+ ex +" }");
                return StatusCode(500, "An error has occured please try again");                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Users user){
            try
            {
                Users cachedUser;               
                await _database.DeleteItem(user);
                if(!_cache.IsEmpty && 
                    _cache.Remove(Convert.ToInt32(user.Id), out cachedUser)) 
                    return Ok();
                _cache.Clear();
                return StatusCode(500, "An error may have occured when deleting user. Please reload users");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete user." +
                " Error: "+ ex +" }");
                return StatusCode(500);            
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Users user){
            try
            {
                await _database.AddItem(user);
                if(!_cache.IsEmpty && 
                    _cache.TryAdd(Convert.ToInt32(user.Id), user)) 
                    return Ok();
                _cache.Clear();
                return StatusCode(500, "An error may have occured when adding user. Please reload users");
            }
            catch (Exception ex) 
            {
                Log.Error($"An error reaching the db to add user." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] Users user){
            try
            {
                Users cachedUser;
                await _database.EditItem(user);
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(user.Id, out cachedUser) &&
                    _cache.TryUpdate(Convert.ToInt32(user.Id), user, cachedUser)) 
                    return Ok();
                _cache.Clear();
                return StatusCode(500, "An error may have occured when editing user. Please reload users");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit user." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }
    }
}