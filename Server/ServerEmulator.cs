using RockPaperScissors.DB;
using RockPaperScissors.Server.Components;

namespace RockPaperScissors.Server
{
    public static class ServerEmulator
    {
        public readonly static FlexibleDB Database = new("rock_paper_scissors");

        public readonly static QueueComponent Queue = new();
        public readonly static PlayersComponent Players = new();
        public readonly static RoundsComponent Rounds = new();

        public readonly static List<Tuple<int, string>> TestPlayers = new()
        {
            new Tuple<int, string>(1219923, "Zik234"),
            new Tuple<int, string>(1419923, "Z6ik"),
            new Tuple<int, string>(1219623, "Zi213k"),
        };

        public static int TestPlayersUsed = 0;

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

        public static async Task RunCycle()
        {
            while (Running)
            {
                await Task.Delay(3000);

                while (Queue.Lenght >= 3)
                {
                    Dictionary<byte, List<Player>> sortedLevelTable = new();

                    for (int i = 0; i < Queue.Lenght; i++)
                    {
                        Player player = Queue.Raw[i];

                        if (!sortedLevelTable.ContainsKey(player.Level))
                            sortedLevelTable.Add(player.Level, new List<Player>());

                        sortedLevelTable[player.Level].Add(player);
                    }

                    foreach (List<Player> players in sortedLevelTable.Values)
                    {
                        while (players.Count >= 3)
                        {
                            Player first = players[0];
                            Player second = players[1];
                            Player third = players[2];

                            Rounds.Create(new Player[] { first, second, third }, 5, first.Level);

                            players.Remove(first);
                            players.Remove(second);
                            players.Remove(third);

                            Queue.Remove(first);
                            Queue.Remove(second);
                            Queue.Remove(third);
                        }
                    }
                }

                GC.Collect();
            }
        }

        public static void StopCycle()
        {
            Running = false;
        }
    }
}
