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
    public class UsersController : ControllerBase
    {
        private IDatabaseAccess<Users> _database;

        public UsersController(IDatabaseAccess<Users> database, CacheDictionaries dictionaries)
        {
            var instatiated = dictionaries;
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            try
            {
                if(!CacheDictionaries.CachedUsers.IsEmpty) return Ok(CacheDictionaries.CachedUsers.Values.ToList());
                var records =  await _database.GetAllRecords(new Users());
                var usersDto = records.Select(r => DbEntityDtoTransformer.UserDbEntryToDto(r));
                foreach (var user in usersDto)
                {
                    if(!CacheDictionaries.CachedUsers.TryAdd(user.Id, user)) 
                    {
                        CacheDictionaries.CachedUsers.Clear();
                        throw new Exception("Caching mechanism failed to load all Users"); 
                    }               
                }          
                return Ok(usersDto);
                
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
                User cachedUser;
                if(!CacheDictionaries.CachedUsers.IsEmpty &&
                    CacheDictionaries.CachedUsers.TryGetValue(Convert.ToInt32(id), out cachedUser)) 
                    return Ok(cachedUser); 
                var response =  await _database.GetSingleRecord(new Users(), Convert.ToInt32(id));
                if (response == null) return BadRequest("User cannot be found");
                return Ok(DbEntityDtoTransformer.UserDbEntryToDto(response));                     
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single user." +
                " Error: "+ ex +" }");
                return StatusCode(500, "An error has occured please try again");                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] User user){
            try
            {
                User cachedUser;               
                await _database.DeleteItem(DbEntityDtoTransformer
                        .UserDtoToDbEntry(user));
                if(!CacheDictionaries.CachedUsers.IsEmpty &&
                    CacheDictionaries.CachedUsers.Remove(Convert.ToInt32(user.Id), out cachedUser)) 
                    return Ok();
                CacheDictionaries.CachedUsers.Clear();
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
        public async Task<IActionResult> AddUser([FromBody] User user){
            try
            {
                user.Id = await _database.AddItem(DbEntityDtoTransformer.UserDtoToDbEntry(user));
                if(CacheDictionaries.CachedUsers.TryAdd(Convert.ToInt32(user.Id), user)) 
                    return Ok();
                CacheDictionaries.CachedUsers.Clear();
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
        public async Task<IActionResult> EditUser([FromBody] User user){
            try
            {
                User cachedUser;
                await _database.EditItem(DbEntityDtoTransformer.UserDtoToDbEntry(user));
                if(!CacheDictionaries.CachedUsers.IsEmpty &&
                    CacheDictionaries.CachedUsers.TryGetValue(user.Id, out cachedUser) &&
                    CacheDictionaries.CachedUsers.TryUpdate(Convert.ToInt32(user.Id), user, cachedUser)) 
                    return Ok();
                CacheDictionaries.CachedUsers.Clear();
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