namespace RockPaperScissors.Models
{
    public class PlayFreeModel
    {
        public int MinLvl { get; set; } = 0;
        public string ViewText => $"Бесплатная игра доступна с {MinLvl} уровня.";
    }
}
