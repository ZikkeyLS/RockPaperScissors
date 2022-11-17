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
            Water,
            Air,
            Paper, 
            Sponge,
            Scissors,
            Fire
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
            public readonly int Value = -1;
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
        private List<Player> _winners = new();

        private PlayerInput[] _inputs = new PlayerInput[3];
        private int _inputsCount = 0;

        private int _waitSeconds = 5;
        private byte _level = 0;
        private int _score = 0;
        private int _iteration = 1;
        private DateTime _startTime;

        private Status _status = Status.initialized;

        public Status GameStatus => _status;
        public byte Level => _level;
        public Player[] Winners => _winners.ToArray();
        public Player[] Players => _players;
        public bool HasWinner => Winners != null;
        public int Iteration => _iteration;
        public DateTime StartTime => _startTime;
        public int WaitSeconds => _waitSeconds;
        public PlayerInput[] Inputs => _inputs;

        public Round(Player[] players, int waitSeconds, byte level)
        {
            _players = players;
            Open(players, waitSeconds, level);
        }

        public async Task Open(Player[] players, int waitSeconds, byte level)
        {
            _players = players;
            _waitSeconds = waitSeconds;

            _level = level;
            _score = ServerEmulator.LevelTable[level];
            _inputs = new PlayerInput[3];

            if (_status == Status.readress)
                await Task.Delay(5000);

            _startTime = DateTime.Now;
            _status = Status.waiting;
            await Task.Delay(_waitSeconds * 1000);

            List<PlayerInput> checkedInputs = new();

            int loserCount = 0;
            int nonCount = 0;

            for (int i = 0; i < _inputs.Length; i++)
                if (_inputs[i] != null)
                    checkedInputs.Add(_inputs[i]);


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
                        Figure vs = (Figure)checkInput.Value;
                        if (checkInput.Value != -1 && status.BeatenBy.Contains(vs))
                        {
                            input.Winner = 0;
                            loserCount += 1;
                        }
                    }
                }
            }

            if ((nonCount != 0 && loserCount == 0) || checkedInputs.Count == 0)
            {
                _status = Status.denied;
                await Task.Delay(2000);
                ServerEmulator.Rounds.Remove(this);
                return;
            }

            if (loserCount == 0 || loserCount == 3)
            {
                if(nonCount != 0)
                {
                    _status = Status.denied;
                    await Task.Delay(2000);
                    ServerEmulator.Rounds.Remove(this);
                    return;
                }

                _status = Status.readress;
                _inputsCount = 0;

                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] != null)
                        players[i].WriteLastStatusData();
                }

                _iteration += 1;
                await Task.Delay(2000);

                await Open(players, waitSeconds, level);
                return;
            }

            for (int i = 0; i < checkedInputs.Count; i++)
            {
                PlayerInput input = checkedInputs[i];
                DataRowCollection collection = ServerEmulator.Database.CreateGetRequest("users", new DB.FlexibleDB.Value[] { new DB.FlexibleDB.Value("id", input.Player.Id) });

                if (input.Winner == 1)
                {
                    _winners.Add(input.Player);
                    ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("points", ((int)collection[0][2]) + (_score * 2)), new DB.FlexibleDB.Value("id", input.Player.Id));
                }
                else if(level != 0)
                {
                    ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("points", Math.Clamp((int)collection[0][2] - _score, 0, 9999)), new DB.FlexibleDB.Value("id", input.Player.Id));
                }

                ServerEmulator.Database.CreateChangeRequest("users", new DB.FlexibleDB.Value("games", ((int)collection[0][3]) + 1), new DB.FlexibleDB.Value("id", input.Player.Id));
            }

            for(int i = 0; i < players.Length; i++)
            {
                if(players[i] != null)
                    players[i].WriteLastStatusData();
            }

            _status = Status.complete;

            await Task.Delay(2000);
            ServerEmulator.Rounds.Remove(this);
        }

        public bool ContainsPlayer(Player player)
        {
            return _players.Contains(player);
        }

        public void AddPlayerInput(Player player, int value)
        {
            if (_status != Status.waiting)
                return;

            for (int i = 0; i < _inputs.Length; i++)
                if (_inputs[i] != null && _inputs[i].Player == player)
                {
                    _inputs[i] = new PlayerInput(player, value);
                    return;
                }

            _inputs[_inputsCount] = new PlayerInput(player, value);
            _inputsCount += 1;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
