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
    
    public class RegularSendOutController : ControllerBase
    {
        private IDatabaseAccess<RegularSendout> _database;
        public RegularSendOutController(IDatabaseAccess<RegularSendout> database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IEnumerable<RegularSendout>> GetSendouts(){
            try
            {                
                return await _database.GetAllRecords(new RegularSendout());
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract sendouts." +
                " Error: "+ ex +" }");
                return new List<RegularSendout>(){};
            }
            
        }

        [HttpGet("{Id}")]
        public async Task<RegularSendout> GetSingleSendout(string id)
        {
            try
            {
                return await _database.GetSingleRecord(new RegularSendout(), Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single sendout." +
                " Error: "+ ex +" }");
                return new RegularSendout();                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSendout([FromBody] RegularSendout sendout){
            try
            {
                await _database.DeleteItem(sendout);
                return Ok();
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
                await _database.AddItem(sendout);
                return Ok();
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
                await _database.EditItem(sendout);
                return Ok();
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