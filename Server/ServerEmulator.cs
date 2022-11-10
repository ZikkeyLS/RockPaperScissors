namespace RockPaperScissors.Server
{
    public static class ServerEmulator
    {
        private readonly static List<Player> players = new();
        private readonly static List<Player> queue = new();
        private readonly static List<Round> rounds = new();

        public static bool Running = true;

        public static async void RunCicle()
        {
            await Task.Factory.StartNew(() => 
            {
                while (Running)
                {
                    Task.Delay(5000);

                    while(queue.Count > 3)
                    {
                        // Check players level. Then create room if 3 players have same level.
                    }
                }
            });
        }

        public static void StopCicle()
        {
            Running = false;
        }

        public static void AddToQueue(int id)
        {
            Player player = players.Find((element) => element.Id == id);

            if(player != null)
                queue.Add(player);
        }

        public static void RemoveFromQueue(int id)
        {
            Player player = players.Find((element) => element.Id == id);

            if (player != null)
                if (players.Contains(player))
                    players.Remove(player);
        }

        public static void AddPlayer(int id, string name)
        {
            players.Add(new Player(id, name));
        }

        public static void RemovePlayer(Player player)
        {
            if (players.Contains(player))
                players.Remove(player);
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
