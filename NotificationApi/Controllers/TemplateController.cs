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
    public class TemplateController : ControllerBase
    {
        private IDatabaseAccess<Templates> _database;
        private ConcurrentDictionary<int, Templates> _cache;
        public TemplateController(IDatabaseAccess<Templates> database, CacheLists cache)
        {
            _database = database;
            _cache = cache.CachedTemplates;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(){
            try
            {
                if(!_cache.IsEmpty) return Ok(_cache.Values.ToList());
                var response = await _database.GetAllRecords(new Templates());
                if (response.Count() == 0) return BadRequest("No Templates found");    
                foreach (var template in response)
                {
                    if(!_cache.TryAdd(template.Id, template)) 
                    {
                        _cache.Clear();
                        throw new Exception("Caching mechanism failed to load all Users"); 
                    }               
                }          
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract templates." +
                " Error: "+ ex +" }");
                return Ok(new List<Templates>());
            }            
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleTemplate(string id)
        {
            try
            {
                Templates cachedTemplate;
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(Convert.ToInt32(id), out cachedTemplate)) 
                    return Ok(cachedTemplate); 
                var response = await _database.GetSingleRecord(new Templates(), Convert.ToInt32(id));
                if (response == null) return BadRequest("User cannot be found");
                return Ok(response);                 
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single template." +
                " Error: "+ ex +" }");
                return Ok(new Templates());                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTemplate([FromBody] Templates template){
            try
            {
                Templates cachedTemplate;
                await _database.DeleteItem(template);
                if(!_cache.IsEmpty && 
                    _cache.Remove(Convert.ToInt32(template.Id), out cachedTemplate)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when deleting template. Please reload templates");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete template." +
                " Error: "+ ex +" }");
                return StatusCode(500);            
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate([FromBody] Templates template){
            try
            {
                await _database.AddItem(template);
                if(!_cache.IsEmpty && 
                    _cache.TryAdd(Convert.ToInt32(template.Id), template)) 
                    return Ok(); 
                return StatusCode(500, "An error may have occured when adding template. Please reload templates");         
            }
            catch (Exception ex) 
            {
                Log.Error($"An error reaching the db to add template." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditTemplate([FromBody] Templates template){
            try
            {
                Templates cachedTemplate;
                await _database.EditItem(template);
                if(!_cache.IsEmpty && 
                    _cache.TryGetValue(template.Id, out cachedTemplate) &&
                    _cache.TryUpdate(Convert.ToInt32(template.Id), template, cachedTemplate)) 
                    return Ok();                
                return StatusCode(500, "An error may have occured when editing template. Please reload template");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit temlate." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
        }        
    }
}