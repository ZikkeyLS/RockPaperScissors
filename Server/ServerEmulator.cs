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

            if (player != null)
                queue.Add(player);
        }

        public static bool PlayerInQueue(Player player)
        {
            return queue.Contains(player);
        }

        public static Round GetPlayerRound(Player player)
        {
            for(int i = 0; i < rounds.Count; i++)
            {
                Round round = rounds[i];

                if (round.ContainsPlayer(player))
                    return round;
            }

            return null;
        }

        public static void RemoveFromQueue(int id)
        {
            Player player = players.Find((element) => element.Id == id);

            if (player != null)
                if (players.Contains(player))
                    players.Remove(player);
        }

        public static void AddPlayer(int id, string name, byte level)
        {
            players.Add(new Player(id, name, level));
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

        public static Player GetPlayer(int id)
        {
            return players.Find((player) => player.Id == id);
        }
    }
}
