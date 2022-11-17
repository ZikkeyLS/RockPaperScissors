using RockPaperScissors.JsonModels;

namespace RockPaperScissors.Server
{
    public class Player : IDisposable
    {
        public class WinData
        {
            public class WinPlayerData
            {
                public string Name { get; set; }
                public int Choice { get; set; }
                public bool Winner { get; set; }
            }

            public WinPlayerData Players { get; set; }
            public int Round { get; set; }
            public int Iteration { get; set; }
            public bool YouWon { get; set; }
        }

        public readonly int Id;
        public readonly string Name;
        public byte Level = 0;

        private DateTime lastTime;
        private DateTime queueStart;
       // private WinData winData;
        private StatusData status;

        public DateTime QueueStart => queueStart;
        //public WinData LastWinData => winData;
        public StatusData LastStatusData => status;

        public Player(int id, string name, byte level = 0)
        {
            Id = id;
            Name = name;
            Level = level;

            Live();
        }

        public void SetQueueStart()
        {
            queueStart = DateTime.Now;
        }

        public async Task Live()
        {
            while (true)
            {
                await Task.Delay(3000);

                long lastSeconds = lastTime.Ticks / TimeSpan.TicksPerSecond;
                long seconds = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

                if (lastSeconds == 0)
                    lastSeconds = seconds;

                if (seconds - lastSeconds >= 3)
                {
                    ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("logged_in_device", ""), new DB.FlexibleDB.Value("id", Id));
                    ServerEmulator.Players.Remove(this);
                    Dispose();
                    return;
                }
            }
        }

        public void SetLastTime(DateTime time)
        {
            lastTime = time;
        }

        public void WriteLastStatusData()
        {
            status.PlayerWinner = false;

            Player player = ServerEmulator.Players.Get(Id);
            Round round = ServerEmulator.Rounds.GetPlayersRound(player);

            status.Level = round.Level;
            status.Iteration = round.Iteration;

            for (int i = 0; i < round.Inputs.Length; i++)
            {
                StatusData.PlayerData playerData = status.Players[i];

                playerData.Name = round.Inputs[i].Player.Name;
                playerData.Choice = (byte)round.Inputs[i].Value;
                playerData.Winner = false;
            }

            for (int i = 0; i < round.Winners.Length; i++)
            {
                StatusData.PlayerData playerData = status.Players[i];

                playerData.Winner = true;

                if (Id == round.Winners[i].Id)
                    status.PlayerWinner = true;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
