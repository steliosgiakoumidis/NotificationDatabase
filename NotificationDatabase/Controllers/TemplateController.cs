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
    public class TemplateController : ControllerBase
    {
        private IDatabaseAccess<Templates> _database;
        public TemplateController(IDatabaseAccess<Templates> database, CacheDictionaries dictionaries)
        {
            var instatiated = dictionaries;
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(){
            try
            {
                if(!CacheDictionaries.CachedTemplates.IsEmpty) return Ok(CacheDictionaries.CachedTemplates.Values.ToList());
                var records = await _database.GetAllRecords(new Templates());
                var templatesDto = records.Select(r => DbEntityDtoTransformer.TemplateDbEntityToDto(r));
                foreach (var template in templatesDto)
                {
                    if(!CacheDictionaries.CachedTemplates.TryAdd(template.Id, template)) 
                    {
                        CacheDictionaries.CachedTemplates.Clear();
                        throw new Exception("Caching mechanism failed to load all Users"); 
                    }               
                }          
                return Ok(templatesDto);
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
                Template cachedTemplate;
                if(!CacheDictionaries.CachedTemplates.IsEmpty &&
                    CacheDictionaries.CachedTemplates.TryGetValue(Convert.ToInt32(id), out cachedTemplate)) 
                    return Ok(cachedTemplate); 
                var response = await _database.GetSingleRecord(new Templates(), Convert.ToInt32(id));
                if (response == null) return BadRequest("User cannot be found");
                return Ok(DbEntityDtoTransformer.TemplateDbEntityToDto(response));                 
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single template." +
                " Error: "+ ex +" }");
                return Ok(new Template());                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTemplate([FromBody] Template template){
            try
            {
                Template cachedTemplate;
                await _database.DeleteItem(DbEntityDtoTransformer
                    .TemplateDtoToDbEntity(template));
                if(!CacheDictionaries.CachedTemplates.IsEmpty &&
                    CacheDictionaries.CachedTemplates.Remove(Convert.ToInt32(template.Id), out cachedTemplate)) 
                    return Ok();
                CacheDictionaries.CachedTemplates.Clear();
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
        public async Task<IActionResult> AddTemplate([FromBody] Template template){
            try
            {
                await _database.AddItem(DbEntityDtoTransformer.TemplateDtoToDbEntity(template));
                if(!CacheDictionaries.CachedTemplates.IsEmpty &&
                    CacheDictionaries.CachedTemplates.TryAdd(Convert.ToInt32(template.Id), template)) 
                    return Ok();
                CacheDictionaries.CachedTemplates.Clear();
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
        public async Task<IActionResult> EditTemplate([FromBody] Template template){
            try
            {
                Template cachedTemplate;
                await _database.EditItem(DbEntityDtoTransformer.TemplateDtoToDbEntity(template));
                if(!CacheDictionaries.CachedTemplates.IsEmpty &&
                    CacheDictionaries.CachedTemplates.TryGetValue(template.Id, out cachedTemplate) &&
                    CacheDictionaries.CachedTemplates.TryUpdate(Convert.ToInt32(template.Id), template, cachedTemplate)) 
                    return Ok();
                CacheDictionaries.CachedTemplates.Clear();
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