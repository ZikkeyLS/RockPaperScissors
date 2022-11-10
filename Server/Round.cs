namespace RockPaperScissors.Server
{
    public class Round : IDisposable
    {
        [Flags]
        public enum Status : int
        {
            readress = 0,
            waiting,
            complete
        }

        public enum Figure : int 
        {
            Rock = 0,
            Paper,
            Scissors,
            Water,
            Fire,
            Air,
            Sponge
        }

        public class FigureStatus
        {
            public readonly Figure Figure;
            public readonly Figure[] BeatenBy;

            public FigureStatus(Figure figure, Figure[] beatenBy)
            {
                Figure = figure;
                BeatenBy = beatenBy;
            }
        }

        public class PlayerInput
        {
            public readonly Player Player;
            public readonly int Value = 0;
            public int Winner = 1;

            public PlayerInput(Player player, int value)
            {
                Player = player;
                Value = value;
            }
        }

        private static List<FigureStatus> figureStatuses = new(7)
        {
            new FigureStatus(Figure.Rock, new Figure[3] { Figure.Paper, Figure.Air, Figure.Water }),
            new FigureStatus(Figure.Paper, new Figure[3] { Figure.Fire, Figure.Scissors, Figure.Sponge }),
            new FigureStatus(Figure.Scissors, new Figure[3] { Figure.Fire, Figure.Rock, Figure.Water }),
            new FigureStatus(Figure.Water, new Figure[3] { Figure.Sponge, Figure.Paper, Figure.Air }),
            new FigureStatus(Figure.Fire, new Figure[3] { Figure.Air, Figure.Water, Figure.Rock }),
            new FigureStatus(Figure.Air, new Figure[3] { Figure.Scissors, Figure.Sponge, Figure.Paper }),
            new FigureStatus(Figure.Sponge, new Figure[3] { Figure.Fire, Figure.Scissors, Figure.Rock })
        };

        private Player[] _players = new Player[3];
        private List<Player> _winners;
        private int _playersCount = 0;

        private PlayerInput[] _inputs = new PlayerInput[3];
        private int _inputsCount = 0;

        private int _waitSeconds = 5;
        private int _level = 0;

        private Round _readressRound;
        private Status _status;

        public Round(Player[] players)
        {
            _players = players;
        }

        public async void Open(Player[] players, int waitSeconds, int level)
        {
            _players = players;
            _waitSeconds = waitSeconds;
            _level = level;

            _status = Status.waiting;

            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(_waitSeconds * 1000);

                List<PlayerInput> checkedInputs = new List<PlayerInput>();

                for (int i = 0; i < _inputs.Length; i++)
                    if (_inputs != null)
                        checkedInputs.Add(_inputs[i]);

                int loserCount = 0;
                int nonCount = 0;

                for (int i = 0; i < checkedInputs.Count; i++)
                {
                    PlayerInput input = checkedInputs[i];
                    FigureStatus status = figureStatuses.Find((element) => (Figure)input.Value == element.Figure);

                    if (status == null)
                    {
                        input.Winner = 0;
                        nonCount += 1;
                        continue;
                    }

                    for (int j = 0; j < checkedInputs.Count; j++)
                    {
                        PlayerInput checkInput = checkedInputs[j];

                        if (input != checkInput)
                        {
                            if (status.BeatenBy.Contains((Figure)checkInput.Value))
                            {
                                input.Winner = 0;
                                loserCount += 1;
                            }
                        }
                    }
                }

                if (nonCount != 0 && loserCount == 0)
                {
                    // Stop game
                    return;
                }

                if (loserCount == 0 && nonCount == 0)
                {
                    // Rerun round
                    return;
                }

                for(int i = 0; i < checkedInputs.Count; i++)
                {
                    PlayerInput input = checkedInputs[i];

                    if (input.Winner == 1)
                    {
                        if (_winners == null)
                            _winners = new List<Player>();

                        _winners.Add(input.Player);
                        // give points
                    }
                    else
                    {
                        // remove points
                    }
                }

                Thread.Sleep(30000);
                ServerEmulator.RemoveRound(this);
            });
        }

        public void AddPlayerInput(Player player, int value)
        {
            _inputs[_inputsCount] = new PlayerInput(player, value);
            _inputsCount += 1;
        }

        public void Readress()
        {
            Open(_players, _waitSeconds, _level);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Status GameStatus => _status;

        public Round ReadressRound => _readressRound;
        public bool NeedReadress => _readressRound != null;

        public Player[] Winners => _winners.ToArray();
        public bool HasWinner => Winners != null;
    }
}
