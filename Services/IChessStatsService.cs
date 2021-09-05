using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Models;

namespace API.Services {
    public interface IChessStatsService {
        Task<ChessStats> GetStats(string username, IList<Config> configs);
        // Task<Dictionary<string, Dictionary<string, Dictionary<string, int>>>> GetStats(string username, IList<Config> configs);
        Task<IEnumerable<Game>> GetGames(string username);
    }
}