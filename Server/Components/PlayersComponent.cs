namespace RockPaperScissors.Server.Components
{
    public class PlayersComponent
    {
        private readonly List<Player> players = new();

        public void Add(int id, string name, byte level)
        {
            players.Add(new Player(id, name, level));
        }

        public void Remove(Player player)
        {
            if (players.Contains(player))
                players.Remove(player);
        }

        public bool Contains(Player player)
        {
            return players.Contains(player);
        }

        public Player Find(Predicate<Player> match)
        {
            return players.Find(match);
        }

        public Player Get(int id)
        {
            return players.Find((player) => player.Id == id);
        }
    }
}
