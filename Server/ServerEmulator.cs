﻿using RockPaperScissors.DB;
using RockPaperScissors.Server.Components;

namespace RockPaperScissors.Server
{
    public static class ServerEmulator
    {
        public readonly static FlexibleDB Database = new("rock_paper_scissors");

        public readonly static QueueComponent Queue = new();
        public readonly static PlayersComponent Players = new();
        public readonly static RoundsComponent Rounds = new();

        public readonly static List<Tuple<string, string>> TestPlayers = new()
        {
            new Tuple<string, string>("1219923", "Zik234"),
            new Tuple<string, string>("1419923", "Z6ik"),
            new Tuple<string, string>("1219923", "Zi213k"),
            new Tuple<string, string>("1216923", "Zi123k"),
            new Tuple<string, string>("1219923", "Z21ik"),
            new Tuple<string, string>("1219923", "Zi6k"),
            new Tuple<string, string>("12199423", "Zik0"),
            new Tuple<string, string>("12193923", "Zi09k"),
            new Tuple<string, string>("12119923", "Zik6"),
            new Tuple<string, string>("12192923", "Zik1"),
            new Tuple<string, string>("12199623", "Zik2"),
            new Tuple<string, string>("12198923", "Zi3k"),
            new Tuple<string, string>("12190923", "Z3ik"),
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
                await Task.Delay(5000);

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

                    int j = 0;

                    foreach (List<Player> players in sortedLevelTable.Values)
                    {
                        j += 1;

                        while (players.Count >= 3)
                        {
                            Player first = players[0];
                            Player second = players[1];
                            Player third = players[2];

                            Rounds.Create(new Player[] { first, second, third }, 3, first.Level);

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
