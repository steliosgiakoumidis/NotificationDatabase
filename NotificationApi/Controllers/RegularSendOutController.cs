using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationApi.Cache;
using NotificationApi.DatabaseLayer;
using NotificationApi.Model;
using NotificationApi.Utilities;
using Serilog;

namespace NotificationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegularSendOutController : ControllerBase
    {
        private IDatabaseAccess<RegularSendout> _database;
        private ConcurrentDictionary<int, RegularSendout> _cache;
        private ConcurrentDictionary<int, Templates> _cachedTemplates;
        private ConcurrentDictionary<int, Users> _cachedUsers;
        private ConcurrentDictionary<int, UserGroups> _cachedUserGroups;

        public RegularSendOutController(IDatabaseAccess<RegularSendout> database, CacheDictionaries cache)
        {
            _database = database;
            _cache = cache.CachedSendouts;
            _cachedTemplates = cache.CachedTemplates;
            _cachedUserGroups = cache.CachedUserGroups;
            _cachedUsers = cache.CachedUsers;
        }

        [HttpGet]
        public async Task<IActionResult> GetSendouts(){
            try
            { 
                if(!_cache.IsEmpty) return Ok(_cache.Values.ToList());               
                var response = await _database.GetAllRecords(new RegularSendout());
                if (response.Count() == 0) return BadRequest("No Templates found");    
                foreach (var sendout in response)
                {
                    if(!_cache.TryAdd(sendout.Id, sendout)) 
                    {
                        _cache.Clear();
                        throw new Exception("Caching mechanism failed to load all sendouts"); 
                    }               
                }          
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract sendouts." +
                " Error: "+ ex +" }");
                return Ok(new List<RegularSendout>());
            }        
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleSendout(string id)
        {
            try
            {
                RegularSendout cachedSendout;
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(Convert.ToInt32(id), out cachedSendout)) 
                    return Ok(cachedSendout); 
                var response = await _database.GetSingleRecord(new RegularSendout(), Convert.ToInt32(id));
                if (response == null) return BadRequest("Sendout cannot be found");
                return Ok(response);            
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single sendout." +
                " Error: "+ ex +" }");
                return Ok(new RegularSendout());                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSendout([FromBody] RegularSendout sendout){
            try
            {
                RegularSendout cachedSendout;
                await _database.DeleteItem(sendout);
                if(!_cache.IsEmpty && 
                    _cache.Remove(Convert.ToInt32(sendout.Id), out cachedSendout)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when deleting sendout. Please reload sendouts");        
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete sendout." +
                " Error: "+ ex +" }");
                return StatusCode(500);            
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSendout([FromBody] RegularSendout sendout){
            try
            {
                if(AddSendoutValidation.CheckUserTemplateAndGroupExist(_cachedUsers.Values.ToList(), 
                    _cachedUserGroups.Values.ToList(), _cachedTemplates.Values.ToList(),
                    sendout)) return BadRequest("Please check that User, User Group and Template exist.");
                await _database.AddItem(sendout);
                if(!_cache.IsEmpty && 
                    _cache.TryAdd(Convert.ToInt32(sendout.Id), sendout)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when adding sendout. Please reload sendouts");             
            }
            catch (Exception ex) 
            {
                Log.Error($"An error reaching the db to add sendout." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditSendout([FromBody] RegularSendout sendout){
            try
            {
                RegularSendout cachedSendout;
                await _database.EditItem(sendout);
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(sendout.Id, out cachedSendout) &&
                    _cache.TryUpdate(Convert.ToInt32(sendout.Id), sendout, cachedSendout)) 
                    return Ok();                
                return StatusCode(500, "An error may have occured when editing sendout. Please reload sendouts");           
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit sendout." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }
    }
}