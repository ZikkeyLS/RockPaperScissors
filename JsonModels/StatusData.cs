namespace RockPaperScissors.JsonModels
{
    public class StatusData
    {
        public class PlayerData
        {
            public string Name { get; set; }
            public byte Choice { get; set; }
            public bool Winner { get; set; }
        }

        public int Level;
        public int Iteration;
        public PlayerData[] Players = new PlayerData[3];
        public bool PlayerWinner;
    }
}
