using System.Data;

namespace RockPaperScissors.Server
{
    public class Round : IDisposable
    {
        public enum Status : byte
        {
            readress = 0,
            waiting,
            complete,
            denied,
            initialized
        }

        public enum Figure : byte
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

        private static readonly List<FigureStatus> figureStatuses = new(7)
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

        private PlayerInput[] _inputs = new PlayerInput[3];
        private int _inputsCount = 0;

        private int _waitSeconds = 5;
        private byte _level = 0;
        private int _score = 0;

        private Status _status = Status.initialized;

        public Status GameStatus => _status;
        public Player[] Winners => _winners.ToArray();
        public bool HasWinner => Winners != null;

        public Round(Player[] players)
        {
            _players = players;
        }

        public async void Open(Player[] players, int waitSeconds, byte level)
        {
            _players = players;
            _waitSeconds = waitSeconds;

            _level = level;
            _score = ServerEmulator.LevelTable[level];



            await Task.Factory.StartNew(() =>
            {
                if (_status == Status.readress)
                    Thread.Sleep(5000);

                _status = Status.waiting;
                Thread.Sleep(_waitSeconds * 1000);

                List<PlayerInput> checkedInputs = new();

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
                    _status = Status.denied;
                    return;
                }

                if (loserCount == 0 && nonCount == 0)
                {
                    _status = Status.readress;
                    Open(players, waitSeconds, level);
                    return;
                }

                for (int i = 0; i < checkedInputs.Count; i++)
                {
                    PlayerInput input = checkedInputs[i];

                    if (input.Winner == 1)
                    {
                        if (_winners == null)
                            _winners = new List<Player>();

                        _winners.Add(input.Player);
                        DataRowCollection collection = ServerEmulator.Database.CreateGetRequest("users", new DB.FlexibleDB.Value[] { new DB.FlexibleDB.Value("id", input.Player.Id) });
                        ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("points", ((int)collection[0][2]) + _score));
                    }
                    else
                    {
                        DataRowCollection collection = ServerEmulator.Database.CreateGetRequest("users", new DB.FlexibleDB.Value[] { new DB.FlexibleDB.Value("id", input.Player.Id) });
                        ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("points", Math.Clamp((int)collection[0][2] - _score, 0, 9999)));
                    }
                }

                _status = Status.complete;

                Thread.Sleep(30000);
                ServerEmulator.RemoveRound(this);
            });
        }

        public bool ContainsPlayer(Player player)
        {
            return _players.Contains(player);
        }

        public void AddPlayerInput(Player player, int value)
        {
            _inputs[_inputsCount] = new PlayerInput(player, value);
            _inputsCount += 1;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
