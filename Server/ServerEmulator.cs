using RockPaperScissors.DB;

namespace RockPaperScissors.Server
{
    public static class ServerEmulator
    {
        private readonly static List<Player> players = new();
        private readonly static List<Player> queue = new();
        private readonly static List<Round> rounds = new();
        public readonly static FlexibleDB Database = new("rock_paper_scissors");

        public static bool Running { get; private set; } = true;

        public static readonly Dictionary<byte, byte> ScoreTable = new() 
        {
            // Score => Level
            { 200, 20 },
            { 150, 19 },
            { 125, 18 },
            { 110, 17 },
            { 105, 16 },
            { 100, 15 },
            { 95, 14 },
            { 90, 13 },
            { 84, 12 },
            { 78, 11 },
            { 72, 10 },
            { 64, 9 },
            { 48, 8 },
            { 32, 7 },
            { 24, 6 },
            { 16, 5 },
            { 8, 4 },
            { 4, 3 },
            { 2, 2 },
            { 1, 1 },
            { 1, 0 }
        };

        public static readonly Dictionary<byte, byte> LevelTable = new()
        {
            // Level => Score
            { 20, 200 },
            { 19, 150 },
            { 18, 125 },
            { 17, 110 },
            { 16, 105 },
            { 15, 100 },
            { 14, 95 },
            { 13, 90 },
            { 12, 84 },
            { 11, 78 },
            { 10, 72 },
            { 9, 64 },
            { 8, 48 },
            { 7, 32 },
            { 6, 24 },
            { 5, 16 },
            { 4, 8 },
            { 3, 4 },
            { 2, 2 },
            { 1, 1 },
            { 0, 1 }
        };


        public static async void RunCycle()
        {
            await Task.Factory.StartNew(() =>
            {
                while (Running)
                {
                    Task.Delay(5000);

                    while (queue.Count >= 3)
                    {
                        Dictionary<byte, List<Player>> sortedLevelTable = new();

                        for (int i = 0; i < queue.Count; i++)
                        {
                            Player player = queue[i];

                            if (!sortedLevelTable.ContainsKey(player.Level))
                                sortedLevelTable.Add(player.Level, new List<Player>());

                            sortedLevelTable[player.Level].Add(player);
                        }

                        foreach (List<Player> players in sortedLevelTable.Values)
                        {
                            while(players.Count >= 3)
                            {
                                Player first = players[0];
                                Player second = players[1];
                                Player third = players[2];

                                CreateRound(new Player[] { first, second, third });

                                players.Remove(first);
                                players.Remove(second);
                                players.Remove(third);

                                queue.Remove(first);
                                queue.Remove(second);
                                queue.Remove(third);
                            }
                        }
                    }
                }
            });
        }

        public static void StopCycle()
        {
            Running = false;
        }

        public static void AddToQueue(int id)
        {
            Player player = players.Find((element) => element.Id == id);

            foreach(byte value in LevelTable.Keys)
                if(player.Score > value)
                    player.Level = LevelTable[value];

            if (player != null)
                queue.Add(player);
        }

        public static void RemoveFromQueue(int id)
        {
            Player player = players.Find((element) => element.Id == id);

            if (player != null)
                if (players.Contains(player))
                    players.Remove(player);
        }

        public static void AddPlayer(int id, string name, int score)
        {
            players.Add(new Player(id, name, score));
        }

        public static void RemovePlayer(Player player)
        {
            if (players.Contains(player))
                players.Remove(player);
        }

        public static bool ContainsPlayer(Player player)
        {
            return players.Contains(player);
        }

        public static void CreateRound(Player[] players)
        {
            rounds.Add(new Round(players));
        }

        public static void RemoveRound(Round round)
        {
            rounds.Remove(round);
            round.Dispose();
        }
    }
}
