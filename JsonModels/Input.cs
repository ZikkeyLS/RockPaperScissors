namespace RockPaperScissors.JsonModels
{
    [Serializable]
    public class Input
    {
        public string Status;

        public Input(string status)
        {
            Status = status;
        }
    }
}
