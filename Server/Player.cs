namespace RockPaperScissors.Server
{
    public class Player : IDisposable
    {
        public readonly int Id;
        public readonly string Name;
        public byte Level = 0;

        private DateTime lastTime;

        public Player(int id, string name, byte level = 0)
        {
            Id = id;
            Name = name;
            Level = level;

            Live();
        }

        public async Task Live()
        {
            while (true)
            {
                await Task.Delay(3000);

                long lastSeconds = lastTime.Ticks / TimeSpan.TicksPerSecond;
                long seconds = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

                Console.WriteLine(lastSeconds + " " + seconds);

                if (lastSeconds == 0)
                    lastSeconds = seconds;

                if (seconds - lastSeconds >= 3)
                {
                    ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("logged_in_device", ""), new DB.FlexibleDB.Value("id", Id));
                    ServerEmulator.Players.Remove(this);
                    Dispose();
                    return;
                }

                lastSeconds = seconds;
            }
        }

        public void SetLastTime(DateTime time)
        {
            lastTime = time;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
