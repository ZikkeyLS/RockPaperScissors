namespace RockPaperScissors.JsonModels
{
    [Serializable]
    public class QueueRequest
    {
        // input
        // int id
        // byte level

        // output
        public string Status; 

        public QueueRequest(string status)
        {
            Status = status;
        }
    }
}
