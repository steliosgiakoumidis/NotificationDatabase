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
    public class UserGroupsController : ControllerBase
    {
        private IDatabaseAccess<UserGroups> _database;
        public UserGroupsController(IDatabaseAccess<UserGroups> database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IEnumerable<UserGroups>> GetUserGroups(){
            try
            {
                return await _database.GetAllRecords(new UserGroups());
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract groups." +
                " Error: "+ ex +" }");
                return new List<UserGroups>(){};
            }
            
        }

        [HttpGet("{Id}")]
        public async Task<UserGroups> GetSingleUSerGroup(string id)
        {
            try
            {
                return await _database.GetSingleRecord(new UserGroups(),
                 Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single group." +
                " Error: "+ ex +" }");
                return new UserGroups();                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserGroup([FromBody] UserGroups group){
            try
            {
                await _database.DeleteItem(group);
                return Ok();
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
                return Ok();
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
                await _database.EditItem(group);
                return Ok();
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