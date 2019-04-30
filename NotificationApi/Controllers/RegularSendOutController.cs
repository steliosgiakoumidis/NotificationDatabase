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
using NotificationCommon;
using NotificationCommon.Models;
using Serilog;

namespace NotificationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegularSendOutController : ControllerBase
    {
        private IDatabaseAccess<RegularSendout> _database;
        public RegularSendOutController(IDatabaseAccess<RegularSendout> database, CacheDictionaries dictionaries)
        {
            var instantiated = dictionaries;
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetSendouts()
        {
            try
            {
                if (!CacheDictionaries.CachedSendouts.IsEmpty) return Ok(CacheDictionaries.CachedSendouts.Values.ToList());
                var records = await _database.GetAllRecords(new RegularSendout());
                var sendoutsDto = records.Select(r => DbEntityDtoTransformer.SendoutDbEntityToDto(r));
                foreach (var sendout in sendoutsDto)
                {
                    if (!CacheDictionaries.CachedSendouts.TryAdd(sendout.Id, sendout))
                    {
                        CacheDictionaries.CachedSendouts.Clear();
                        throw new Exception("Caching mechanism failed to load all sendouts");
                    }
                }
                return Ok(sendoutsDto);
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract sendouts." +
                " Error: " + ex + " }");
                return Ok(new List<RegularSendout>());
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSingleSendout(string id)
        {
            try
            {
                Sendout cachedSendout;
                if (!CacheDictionaries.CachedSendouts.IsEmpty &&
                    CacheDictionaries.CachedSendouts.TryGetValue(Convert.ToInt32(id), out cachedSendout))
                    return Ok(cachedSendout);
                var response = await _database.GetSingleRecord(new RegularSendout(), Convert.ToInt32(id));
                if (response == null) return BadRequest("Sendout cannot be found");
                return Ok(DbEntityDtoTransformer.SendoutDbEntityToDto(response));
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to extract single sendout." +
                " Error: " + ex + " }");
                return Ok(new Sendout());
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSendout([FromBody] Sendout sendout)
        {
            try
            {
                Sendout cachedSendout;
                await _database.DeleteItem(DbEntityDtoTransformer
                        .SendoutDtoToDbEntity(sendout));
                if (!CacheDictionaries.CachedSendouts.IsEmpty &&
                    CacheDictionaries.CachedSendouts
                        .Remove(Convert.ToInt32(sendout.Id), out cachedSendout))
                    return Ok();
                CacheDictionaries.CachedSendouts.Clear();
                return StatusCode(500, "An error may have occured when deleting sendout. Please reload sendouts");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to delete sendout." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSendout([FromBody] Sendout sendout)
        {
            try
            {
                if (!AddSendoutValidation.CheckUserTemplateAndGroupExist(CacheDictionaries.CachedUsers.Values.ToList(),
                    CacheDictionaries.CachedUserGroups.Values.ToList(), CacheDictionaries.CachedTemplates.Values.ToList(),
                    sendout)) return BadRequest("Please check that User, User Group and Template exist.");
                await _database.AddItem(DbEntityDtoTransformer.SendoutDtoToDbEntity(sendout));
                Log.Error("Add sendout succeeded database");
                if (!CacheDictionaries.CachedSendouts.IsEmpty &&
                    CacheDictionaries.CachedSendouts.TryAdd(sendout.Id, sendout))
                    return Ok();
                CacheDictionaries.CachedSendouts.Clear();
                return StatusCode(500, "An error may have occured when adding sendout. Please reload sendouts");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to add sendout." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditSendout([FromBody] Sendout sendout)
        {
            try
            {
                Sendout cachedSendout;
                await _database.EditItem(DbEntityDtoTransformer.SendoutDtoToDbEntity(sendout));
                Log.Error(CacheDictionaries.CachedSendouts.Keys.ToString());
                if (!CacheDictionaries.CachedSendouts.IsEmpty &&
                    CacheDictionaries.CachedSendouts.TryGetValue(sendout.Id, out cachedSendout) &&
                    CacheDictionaries.CachedSendouts.TryUpdate(sendout.Id, sendout, cachedSendout))
                    return Ok();
                CacheDictionaries.CachedSendouts.Clear();
                return StatusCode(500, "An error may have occured when editing sendout. Please reload sendouts");
            }
            catch (Exception ex)
            {
                Log.Error($"An error reaching the db to edit sendout." +
                " Error: " + ex + " }");
                return StatusCode(500);
            }
        }
    }
}