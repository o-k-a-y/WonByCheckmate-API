using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    public class ChessStatsController : BaseApiController {
        private readonly IChessStatsService _chessStatsService;

        public ChessStatsController(DataContext context, IChessStatsService chessStatsService) {
            _chessStatsService = chessStatsService;
        }

        [HttpGet("games/{username}")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames(string username) {
            // TODO: Some http interceptor to take care of casting username to lower case?
            return Ok(await _chessStatsService.GetGames(username.ToLower()));
        }

        // [FromQuery(Name = "config")] allows passing multiple query parameters with the same key
        // The parameter is renamed to config so something like ?config=1&config=2&config=3 is possible
        [HttpGet("{username}")]
        public async Task<ActionResult<ChessStats>> GetStats(string username, [FromQuery(Name = "configs")] string configsStr) {
            // TODO: fix this awful mess
            if (configsStr != null) {
                string[] configArr = configsStr.Split(',');

                ConfigFactory factory = new ConfigFactory();
                List<Config> configs = (List<Config>) factory.GetConfigs(configArr);
                
                return Ok(await _chessStatsService.GetStats(username.ToLower(), configs));
            }

            // It might be a good idea to do something else if there are no configs passed in as a query parameter
            return NotFound();
        }

    }
}