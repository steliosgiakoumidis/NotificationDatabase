using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public TemplateController(IDatabaseAccess<Templates> database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IEnumerable<Templates>> GetTemplates(){
            try
            {
                return await _database.GetAllRecords(new Templates());
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract templates." +
                " Error: "+ ex +" }");
                return new List<Templates>(){};
            }
            
        }

        [HttpGet("{Id}")]
        public async Task<Templates> GetSingleTemplate(string id)
        {
            try
            {
                return await _database.GetSingleRecord(new Templates(), Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single template." +
                " Error: "+ ex +" }");
                return new Templates();                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTemplate([FromBody] Templates template){
            try
            {
                await _database.DeleteItem(template);
                return Ok();
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
                return Ok();
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
                await _database.EditItem(template);
                return Ok();
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