namespace API.Entities {
    public class Game {
        public int Id { get; set; }
        // Username of the player
        public string Username { get; set; }
        // chess, bughouse, etc, but we're only concerned with chess as it's the normal playing style
        public string Rules { get; set; }
        // 180 means 3 minute game, 1/86400 means 1 day to make 1 move in the game
        public string TimeControl { get; set; }
        // daily, rapid, blitz, bullet..
        public string TimeClass { get; set; }
        // won by resignation, lost on time, ...
        public string Result { get; set; }
        // Username of the other player
        public string Opponent { get; set; }
        public bool Rated { get; set; }
        public string Pgn { get; set; }
        public string Fen { get; set; }
        public int EndTime { get; set; }
    }
}