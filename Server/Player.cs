namespace RockPaperScissors.Server
{
    public class Player
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Score;
        public byte Level;

        public Player(int id, string name, int score)
        {
            Id = id;
            Name = name;
            Score = score;
        }
    }
}
