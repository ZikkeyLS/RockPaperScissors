namespace RockPaperScissors.Server.Components
{
    public class QueueComponent
    {
        private readonly List<Player> queue = new();

        public int Lenght => queue.Count;
        public List<Player> Raw => queue;

        public void Add(int id)
        {
            Player player = ServerEmulator.Players.Find((element) => element.Id == id);

            if (player != null)
                queue.Add(player);
        }

        public bool PlayerInQueue(Player player)
        {
            return queue.Contains(player);
        }

        public void Remove(int id)
        {
            Player player = ServerEmulator.Players.Find((element) => element.Id == id);

            Remove(player);
        }

        public void Remove(Player player)
        {
            if (player != null)
                if (ServerEmulator.Players.Contains(player))
                    ServerEmulator.Players.Remove(player);
        }
    }
}
