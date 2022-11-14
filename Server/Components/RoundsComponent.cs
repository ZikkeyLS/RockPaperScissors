namespace RockPaperScissors.Server.Components
{
    public class RoundsComponent
    {
        private readonly List<Round> rounds = new();

        public Round GetPlayersRound(Player player)
        {
            for (int i = 0; i < rounds.Count; i++)
            {
                Round round = rounds[i];

                if (round.ContainsPlayer(player))
                    return round;
            }

            return null;
        }

        public void Create(Player[] players, int waitSeconds, byte level)
        {
            rounds.Add(new Round(players, waitSeconds, level));
        }

        public void Remove(Round round)
        {
            rounds.Remove(round);
            round.Dispose();
        }
    }
}
