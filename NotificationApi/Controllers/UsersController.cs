using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        public UsersController(IDatabaseAccess<Users> database)
        {
            Log.Error("Constructor is reached");
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            try
            {
                Log.Information("Get all users endpoint reached");
                var result =  await _database.GetAllRecords(new Users());
                if (result.Count() != 0)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract all users." +
                " Error: "+ ex +" }");
                return StatusCode(500);
            }
            
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleUSer(string id)
        {
            try
            {
                Log.Information("Get single user endpoint reached");
                var result =  await _database.GetSingleRecord(new Users(), Convert.ToInt32(id));
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single user." +
                " Error: "+ ex +" }");
                return StatusCode(500);                                
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Users user){
            try
            {
                Log.Information("Delete user endpoint reached");
                await _database.DeleteItem(user);
                return Ok();
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
                Log.Information("Add user endpoint reached");
                await _database.AddItem(user);
                return Ok();
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
                Log.Information("Edit user: " + user.Username + " endpoint reached");
                await _database.EditItem(user);
                return Ok();
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