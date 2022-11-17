using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.DB;
using RockPaperScissors.JsonModels;
using RockPaperScissors.Models;
using RockPaperScissors.Server;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace RockPaperScissors.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Menu(int id, string username)
        {
            if (id != 0 && ServerEmulator.TestPlayersUsed < ServerEmulator.TestPlayers.Count)
            {
                Tuple<int, string> data = ServerEmulator.TestPlayers[ServerEmulator.TestPlayersUsed];

                HttpContext.Session.SetInt32("user_id", data.Item1);
                HttpContext.Session.SetString("user_name", data.Item2);

                ServerEmulator.TestPlayersUsed += 1;
                // HttpContext.Session.SetInt32("user_id", id);
                // HttpContext.Session.SetString("user_name", username);
            }



            return View();
        }

        public IActionResult PlayFree()
        {
            return View(new PlayFreeModel());
        }

        public IActionResult LevelSelector()
        {
            return View();
        }

        public IActionResult Queue()
        {
            return View();
        }

        public IActionResult Round()
        {
            return View();
        }

        public IActionResult Status()
        {
            return View();
        }

        public IActionResult Win()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProveAuth()
        {
            if (CheckBanned())
                return Ok("Index");

            if (!CheckAuth())
                return Ok("Index");

            return Ok();
        }

        public IActionResult Hearthbeat()
        {
            int? id = HttpContext.Session.GetInt32("user_id");

            CompactRedirect status = new();

            if (id == null)
            {
                status.Status = "FakeData";
                status.UrlIndex = "Index";
                return Ok(JsonSerializer.Serialize(status));
            }

            Player player = ServerEmulator.Players.Get(id.Value);
            Round round = ServerEmulator.Rounds.GetPlayersRound(player);
            bool inQueue = ServerEmulator.Queue.PlayerInQueue(player);

            if (id != null)
            {
                if (player != null)
                {
                    player.SetLastTime(DateTime.Now);
                    status.Status = "Beat";
                }
            }

            status.UrlIndex = null;

            if (inQueue)
            {
                status.UrlIndex = "Queue";
            }
            else if (round != null && player != null)
            {
                switch (round.GameStatus)
                {
                    case Server.Round.Status.waiting:
                    case Server.Round.Status.initialized:
                        status.UrlIndex = "Round";
                        break;
                    case Server.Round.Status.complete:
                    case Server.Round.Status.readress:
                        status.UrlIndex = "Status";
                        break;
                    case Server.Round.Status.denied:
                        status.UrlIndex = "Menu";
                        break;
                }
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public bool CheckAuth()
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            HttpContext.Session.SetString("user_agent", userAgent);

            int? userId = HttpContext.Session.GetInt32("user_id");

            if (userId != null)
            {
                DataRowCollection dataRow = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

                object? loggedIn = dataRow[0][5];

                if (loggedIn != null)
                {
                    string loggedInDevice = (string)loggedIn;

                    if (loggedInDevice == "")
                    {
                        if (ServerEmulator.Players.Find((element) => element.Id == userId.Value) == null)
                        {
                            ServerEmulator.Database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", userAgent), new FlexibleDB.Value("id", userId));

                            ServerEmulator.Players.Add(userId.Value, (string)dataRow[0][1]);
                        }

                        return true;
                    }
                    else if (loggedInDevice == userAgent.Replace(" ", ""))
                    {
                        return true;
                    }
                    else
                    {
                        // Error

                        return false;
                    }
                }
                else
                {
                    ServerEmulator.Database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", userAgent), new FlexibleDB.Value("id", userId));
                    return true;
                }
            }
            else
            {
                // Error

                return false;
            }
        }

        public bool CheckBanned()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");

            if (userId != null)
            {
                DataRowCollection dataRow = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

                object? banned = dataRow[0][4];

                if (banned != null)
                {
                    return (int)banned != 0;
                }
            }
            else
            {
                // Error

                return false;
            }

            return true;
        }

        public IActionResult SendQueueRequest(byte level)
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            DataRowCollection playerData = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[] { new FlexibleDB.Value("id", id) });

            QueueRequest request = new();

            int points = (int)playerData[0][2];

            if (level == 0 || points >= ServerEmulator.LevelTable[level])
            {
                ServerEmulator.Players.Get(id).Level = level;
                ServerEmulator.Queue.Add(id);

                request.UrlIndex = "Queue";
                request.Status = "Success";
            }
            else
            {
                request.Status = "FakeData";
            }

            return Ok(JsonSerializer.Serialize(request));
        }

        public IActionResult GetQueueStatus()
        {
            int? id = HttpContext.Session.GetInt32("user_id");

            CompactRedirect status = new();

            if (id == null)
            {
                status.Status = "FakeData";
                return Ok(JsonSerializer.Serialize(status));
            }

            Player player = ServerEmulator.Players.Get(id.Value);
            Round round = ServerEmulator.Rounds.GetPlayersRound(player);

            if (player != null && ServerEmulator.Queue.PlayerInQueue(player))
            {
                status.Status = "InQueue";
            }
            else if (player != null && round != null)
            {
                status.Status = "Complete";
                status.UrlIndex = "Round";
            }
            else if (!ServerEmulator.Queue.PlayerInQueue(player) && round == null)
            {
                status.Status = "Unaccessable";
                status.UrlIndex = "Menu";
            }
            else
            {
                status.Status = "FakeData";
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult QuitQueueRequest()
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            CompactRedirect status = new();

            status.Status = "fakeData";

            Player player = ServerEmulator.Players.Get(id);

            if (player != null && ServerEmulator.Queue.PlayerInQueue(player))
            {
                ServerEmulator.Queue.Remove(player);
                status.Status = "Complete";
                status.UrlIndex = "Menu";
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult SendInput(byte input)
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            Player player = ServerEmulator.Players.Get(id);
            Round round = ServerEmulator.Rounds.GetPlayersRound(player);

            Input status = new();

            if (round == null || player == null)
            {
                status.Status = "fake data";
            }
            else
            {
                status.Status = "all working";
                round.AddPlayerInput(player, input);
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult VerifyTextState()
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            DataRowCollection playerData = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[] { new FlexibleDB.Value("id", id) });

            int points = (int)playerData[0][2];

            return Ok(points);
        }

        public IActionResult VerifyButtonState(byte level)
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            DataRowCollection playerData = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[] { new FlexibleDB.Value("id", id) });

            int points = (int)playerData[0][2];

            bool result = ServerEmulator.LevelTable[level] <= points;

            return Ok(!result);
        }

        public IActionResult RoundDataRequest()
        {
            object rawId = HttpContext.Session.GetInt32("user_id");

            RoundData status = new();

            try
            {
                if (rawId != null)
                {
                    int id = (int)rawId;

                    Player player = ServerEmulator.Players.Get(id);
                    Round round = ServerEmulator.Rounds.GetPlayersRound(player);

                    status.Level = round.Level;
                    status.Score = ServerEmulator.LevelTable[round.Level];
                    status.RoundNumber = round.Iteration;
                    status.LeftTime = round.WaitSeconds - (DateTime.Now - round.StartTime).Seconds;
                }

            }
            catch
            {

            }


            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult QueueDataRequest()
        {
            object rawId = HttpContext.Session.GetInt32("user_id");

            QueueData status = new();

            if (rawId != null)
            {
                int id = (int)rawId;

                Player player = ServerEmulator.Players.Get(id);
                DateTime time = DateTime.Now;

                status.Level = player.Level;
                status.PlayersCount = ServerEmulator.Queue.Lenght;

                TimeSpan result = time - player.QueueStart;

                status.WaitTime = $"{result.Minutes}:{result.Seconds}";
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult GetMenuPageStatus()
        {
            object rawId = HttpContext.Session.GetInt32("user_id");

            MenuPageData status = new();

            if (rawId != null)
            {
                int id = (int)rawId;

                DataRowCollection collection = ServerEmulator.Database.CreateGetRequest("users", new DB.FlexibleDB.Value[] { new DB.FlexibleDB.Value("id", id) });

                status.Score = (int)collection[0][2];
                status.Games = (int)collection[0][3];
            }

            return Ok(JsonSerializer.Serialize(status));
        }

        public IActionResult GetGameStatusStatus()
        {
            object rawId = HttpContext.Session.GetInt32("user_id");

            StatusData status = new();

            if (rawId != null)
            {
                int id = (int)rawId;

                Player player = ServerEmulator.Players.Get(id);
                status = player.LastStatusData;
            }

            return Ok(JsonSerializer.Serialize(status));
        }
    }
}
