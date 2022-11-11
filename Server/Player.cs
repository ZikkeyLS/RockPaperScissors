namespace RockPaperScissors.Server
{
    public class Player
    {
        public readonly int Id;
        public readonly string Name;
        public readonly byte Level;

        public Player(int id, string name, byte level)
        {
            Id = id;
            Name = name;
            Level = level;
        }
    }
}
