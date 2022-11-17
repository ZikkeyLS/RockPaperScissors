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

        public int Level { get; set; }
        public int Iteration { get; set; }
        public PlayerData[] Players { get; set; } = new PlayerData[3] { new(), new(), new() };
        public bool PlayerWinner { get; set; }
    }
}
