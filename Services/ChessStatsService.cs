using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; // JsonConvert
using Newtonsoft.Json.Linq; // JObject, JArray
using API.Extensions;
using System.Diagnostics;

namespace API.Services {
    public class ChessStatsService : IChessStatsService {
        private readonly DataContext _context;
        private HttpClient _client;
        private const string baseUrl = "https://api.chess.com/pub/player/";
        private const string archives = "/games/archives";
        private string endpoint;


        // All known configurations we care about (double check to make sure none are missing)

        public ChessStatsService(IHttpClientFactory httpFactory, DataContext context) {
            _context = context;
            _client = httpFactory.CreateClient();
        }

        public async Task<ChessStats> GetStats(string username, IList<Config> configs) {
        // public async Task<Dictionary<string, Dictionary<string, Dictionary<string, int>>>> GetStats(string username, IList<Config> configs) {
            // username = username.ToLower();
            await UpdateGamesIfNeeded(username);

            return await BuildStatsFromDatabase(username, configs);
        }


        // Returns all the games a user has played
        public async Task<IEnumerable<Game>> GetGames(string username) {
            // username = username.ToLower();
            await UpdateGamesIfNeeded(username);

            return await _context.Games.ToListAsync();
        }

        private async Task UpdateGamesIfNeeded(string username) {
            // Stopwatch stopWatch = new Stopwatch();
            // stopWatch.Start();
            Console.WriteLine(username);

            this.endpoint = baseUrl + username + archives;

            // Get the months of data that still need to be parsed and at the minimum the current month
            // Endpoint should be empty array of games if there is a month where there were no games played
            // i.e. user played in 10/2020, but not in 11/2020, but then played in 12/2020. 11/2020 should through an empty array
            // We avoid this by only fetching endpoint data where games are returned because that's what https://api.chess.com/pub/player/{username}/games/archives" returns

            // The last played game a user played that is stored in the database
            int lastPlayedGameEpoch = await GetMostRecentGameEpoch(username);
            // Parse any months of games that weren't parsed yet and update database
            await RetrieveArchives(username, lastPlayedGameEpoch);
            await _context.SaveChangesAsync();

            // stopWatch.Stop();
            // // Get the elapsed time as a TimeSpan value.
            // TimeSpan ts = stopWatch.Elapsed;

            // // Format and display the TimeSpan value.
            // string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //     ts.Hours, ts.Minutes, ts.Seconds,
            //     ts.Milliseconds / 10);
            // Console.WriteLine("RunTime " + elapsedTime);
        }


        // TODO: Once tables are separated with User table that contains Game table reference, modify LINQ queries to return updated structure according to schema
        // TODO: The LINQ queries used are probably not efficient at all
        // TODO: Some way to specify rules in case in the future different chess rules besides "chess" are allowed (probably not)
        private async Task<ChessStats> BuildStatsFromDatabase(string username, IList<Config> configs) {
        // private async Task<Dictionary<string, Dictionary<string, Dictionary<string, int>>>> BuildStatsFromDatabase(string username, IList<Config> configs) {

            // TODO: Shouldn't be storing the entire database in memory, 
            // Improve the performance of the database and then change code back to multiple database queries on the DataContext
            List<Game> games = await _context.Games.Select(game =>
                new Game {
                    Username = game.Username,
                    Result = game.Result,
                    TimeClass = game.TimeClass,
                    TimeControl = game.TimeControl,
                    Rules = game.Rules
                }).ToListAsync();


            // // TODO: Some way to abstract to separate objects?
            // Dictionary<string, Dictionary<string, Dictionary<string, int>>> stats = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            // foreach (Config config in configs) {
            //     string rules = config.Rules;
            //     string timeClass = config.TimeClass;
            //     string timeControl = config.TimeControl;
            //     var resultTypes = Enum.GetNames(typeof(GameResultType));
            //     if (!stats.ContainsKey(timeClass)) {
            //         stats.Add(timeClass, new Dictionary<string, Dictionary<string, int>>());
            //         stats[timeClass].Add(timeControl, new Dictionary<string, int>());
            //     } else {
            //         if (!stats[timeClass].ContainsKey(timeControl)) {
            //             stats[timeClass].Add(timeControl, new Dictionary<string, int>());
            //         }
            //     }

            //     foreach (var result in resultTypes) {
            //         if (!stats[timeClass][timeControl].ContainsKey(result)) {
            //             int count = games.Count(game => game.Username == username && game.Result == result && game.TimeClass == timeClass && game.TimeControl == timeControl && game.Rules == rules);
            //             stats[timeClass][timeControl].Add(result, count);
            //         }
            //     }
            // }
            // return stats;

            ChessStats stats = new ChessStats();

            // TODO: When adding filtering on game configs, replace validGameConfigurations with that list/selection (i.e. user only wants blitz stats, or a subset of each)
            foreach (Config config in configs) {
                string rules = config.Rules;
                string timeClass = config.TimeClass;
                string timeControl = config.TimeControl;
                var resultTypes = Enum.GetNames(typeof(GameResultType));

                foreach (var result in resultTypes) {
                    int count = games.Count(game => game.Username == username && game.Result == result && game.TimeClass == timeClass && game.TimeControl == timeControl && game.Rules == rules);
                    Dictionary<string, ResultStats> tc = GetTimeClass(stats, timeClass);

                    if (!tc.ContainsKey(timeControl)) {
                        tc.Add(timeControl, new ResultStats());
                    }
                    switch (result) {
                        case nameof(GameResultType.WonByResignation):
                            tc[timeControl].WonByResignation = count;
                            break;
                        case nameof(GameResultType.WonByTimeout):
                            tc[timeControl].WonByTimeout = count;
                            break;
                        case nameof(GameResultType.WonByCheckmate):
                            tc[timeControl].WonByCheckmate = count;
                            break;
                        case nameof(GameResultType.WonByAbandonment):
                            tc[timeControl].WonByAbandonment = count;
                            break;
                        case nameof(GameResultType.DrawByAgreement):
                            tc[timeControl].DrawByAgreement = count;
                            break;
                        case nameof(GameResultType.DrawByStalemate):
                            tc[timeControl].DrawByStalemate = count;
                            break;
                        case nameof(GameResultType.DrawByRepetition):
                            tc[timeControl].DrawByRepetition = count;
                            break;
                        case nameof(GameResultType.DrawByInsufficientMaterial):
                            tc[timeControl].DrawByInsufficientMaterial = count;
                            break;
                        case nameof(GameResultType.DrawByTimeoutVsInsufficientMaterial):
                            tc[timeControl].DrawByTimeoutVsInsufficientMaterial = count;
                            break;
                        case nameof(GameResultType.DrawBy50Move):
                            tc[timeControl].DrawBy50Move = count;
                            break;
                        case nameof(GameResultType.LostByResignation):
                            tc[timeControl].LostByResignation = count;
                            break;
                        case nameof(GameResultType.LostByTimeout):
                            tc[timeControl].LostByTimeout = count;
                            break;
                        case nameof(GameResultType.LostByCheckmate):
                            tc[timeControl].LostByCheckmate = count;
                            break;
                        case nameof(GameResultType.LostByAbandonment):
                            tc[timeControl].LostByAbandonment = count;
                            break;
                    }
                }
            }
            return stats;
        }

        // The single time class property as defined on the ChessStats class so we can avoid unnecessary switch statements
        private Dictionary<string, ResultStats> GetTimeClass(ChessStats stats, string timeClass) {
            return timeClass switch
            {
                "bullet" => stats.Bullet,
                "blitz" => stats.Blitz,
                "rapid" => stats.Rapid,
                "daily" => stats.Daily,
                _  => null
            };

            // switch (timeClass) {
            //     case "bullet":
            //         return stats.Bullet;
            //     case "blitz":
            //         return stats.Blitz;
            //     case "rapid":
            //         return stats.Rapid;
            //     case "daily":
            //         return stats.Daily;
            //     default:
            //         return null;
            // }
        }

        // Epoch time of most recent game played
        private async Task<int> GetMostRecentGameEpoch(string username) {
            // Last played game time or 0 if no games exist
            var games = await _context.Games.Where(game => game.Username == username).ToListAsync();
            if (games.Count <= 0) {
                return 0;
            } else {
                return games.Max(game => game.EndTime);
            }
        }

        // Most recent game date with just year and month
        // 05/2021
        private DateTime GetMostRecentGameDate(int epochDate) {
            // int mostRecentGameTime = await GetMostRecentGameEpoch();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epochDate);
            DateTime mostRecentGameDate = new DateTime(dateTimeOffset.DateTime.Year, dateTimeOffset.DateTime.Month, 1);

            return mostRecentGameDate;
        }

        private async Task RetrieveArchives(string username, int lastPlayedGameEpoch) {
            DateTime mostRecentGameDate = GetMostRecentGameDate(lastPlayedGameEpoch);

            // Get the list of archives
            HttpResponseMessage chessArchives = await _client.GetAsync(this.endpoint, HttpCompletionOption.ResponseContentRead);

            // Read as string
            string chessArchivesString = await chessArchives.Content.ReadAsStringAsync();
            chessArchives.Dispose();

            // Deserialize so we can extract data from JSON
            JObject chessArchivesJson = JsonConvert.DeserializeObject<JObject>(chessArchivesString);

            JArray chessArchivesList = chessArchivesJson.Value<JArray>("archives");

            // The below comment might be misleading because if the games aren't all entirely parsed, there will be issues, don't do it
            // More efficient to parse backwards from the list of archives to so we don't needlessly hit endpoints we don't need to
            foreach (string archive in chessArchivesList.FastReverse()) {
                // foreach (string archive in chessArchivesList) {
                string dateTime = archive.Split($"{baseUrl}{username}/games/")[1];
                DateTime archiveDate;
                DateTime.TryParseExact(dateTime, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out archiveDate);

                Console.WriteLine($"Current archive date: {archiveDate}");

                // TODO: Maybe can add a break statement to exit if the game isn't >= the most recent game date, but make sure that the order is guaranteed
                // Only parse archives if we haven't parsed it yet, or it's the most recently played month (user may have played more games in that month since last parsed)
                if (archiveDate >= mostRecentGameDate) {
                    await RetrieveArchive(archive, username, lastPlayedGameEpoch);
                }
            }
        }


        // Gets game stats given archives of games (from https://api.chess.com/pub/player/{username}/games/archives)
        private async Task RetrieveArchive(string archive, string username, int lastPlayedGameEpoch) {
            // Get the games from a single archive
            HttpResponseMessage chessArchive = await _client.GetAsync(archive, HttpCompletionOption.ResponseContentRead);

            // Read as string
            string chessArchiveString = await chessArchive.Content.ReadAsStringAsync();
            chessArchive.Dispose();

            // Deserialize so we can extract data from JSON
            JObject chessArchiveJson = JsonConvert.DeserializeObject<JObject>(chessArchiveString);

            await ParseGameArchive(chessArchiveJson.Value<JArray>("games"), username, lastPlayedGameEpoch);
        }

        // Parse all the games from a single archive (games in a single month)
        private async Task ParseGameArchive(JArray gamesFromSingleArchive, string username, int lastPlayedGameEpoch) {
            // Loop backwards from gamesFromSingleArchive to avoid needlessly parsing games we don't need to
            foreach (var game in gamesFromSingleArchive.FastReverse()) {
                int endTime = game.Value<int>("end_time");
                // TODO: Once the endtime is less than or equal to lastPlayedGameEpoch, we can break and stop processing any more games assuming order is guaranteed
                if (endTime <= lastPlayedGameEpoch) {
                    continue;
                }
                string rules = game.Value<string>("rules");
                string timeControl = game.Value<string>("time_control");
                string timeClass = game.Value<string>("time_class");

                // TODO: Handle any errors (not parsed correctly)
                // Only check valid game configurations
                if (Config.TryParse(rules, timeClass, timeControl, out Config config)) {
                    if (!ValidGameConfigurations.Contains(config)) {
                        continue;
                    }
                }
                
                // Determine outcome
                JObject white = game.Value<JObject>("white");
                JObject black = game.Value<JObject>("black");

                // Determine if we were white or black
                bool currentPlayerIsWhite = white.Value<string>("username").ToLower().Equals(username.ToLower());
                JObject currentPlayer = currentPlayerIsWhite ? white : black;
                JObject otherPlayer = currentPlayerIsWhite ? black : white;

                string result = "";
                // TODO: The pgn contains a lot of useful data and this switch statement become invalidated really..
                switch (currentPlayer.Value<string>("result")) {
                    // Game was a draw, no one won
                    case GameResult.DrawByAgreement:
                        result = nameof(GameResultType.DrawByAgreement);
                        break;
                    case GameResult.DrawByRepetition:
                        result = nameof(GameResultType.DrawByRepetition);
                        break;
                    case GameResult.DrawByStalemate:
                        result = nameof(GameResultType.DrawByStalemate);
                        break;
                    case GameResult.DrawByInsufficientMaterial:
                        result = nameof(GameResultType.DrawByInsufficientMaterial);
                        break;
                    case GameResult.DrawByTimeoutVsInsufficientMaterial:
                        result = nameof(GameResultType.DrawByTimeoutVsInsufficientMaterial);
                        break;
                    case GameResult.DrawBy50Move:
                        result = nameof(GameResultType.DrawBy50Move);
                        break;
                    // We won
                    case GameResult.Win:
                        switch (otherPlayer.Value<string>("result")) {
                            case GameResult.CheckMated:
                                result = nameof(GameResultType.WonByCheckmate);
                                break;
                            case GameResult.Resigned:
                                result = nameof(GameResultType.WonByResignation);
                                break;
                            case GameResult.Abandoned:
                                result = nameof(GameResultType.WonByAbandonment);
                                break;
                            case GameResult.Timeout:
                                result = nameof(GameResultType.WonByTimeout);
                                break;
                            default:
                                continue;
                        }
                        break;
                    // Otherwise we lost
                    case GameResult.CheckMated:
                        result = nameof(GameResultType.LostByCheckmate);
                        break;
                    case GameResult.Resigned:
                        result = nameof(GameResultType.LostByResignation);
                        break;
                    case GameResult.Abandoned:
                        result = nameof(GameResultType.LostByAbandonment);
                        break;
                    case GameResult.Timeout:
                        result = nameof(GameResultType.LostByTimeout);
                        break;
                    default:
                        continue;
                }


                // TODO: There is more useful information within the pgn like opening played etc
                _context.Games.Add(new Game {
                    Username = username.ToLower(),
                    Rules = rules,
                    TimeControl = timeControl,
                    TimeClass = timeClass,
                    Result = result,
                    Opponent = currentPlayerIsWhite ? black.Value<string>("username").ToLower() : white.Value<string>("username").ToLower(),
                    Rated = game.Value<bool>("rated"),
                    Pgn = game.Value<string>("pgn"),
                    Fen = game.Value<string>("fen"),
                    EndTime = endTime
                });
            }
            // await _context.SaveChangesAsync();
        }
    }
}