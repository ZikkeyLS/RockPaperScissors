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
            if (id != 0)
                HttpContext.Session.SetInt32("user_id", id);

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
            int? userId = HttpContext.Session.GetInt32("user_id");

            if (userId != null)
            {
                Player player = ServerEmulator.Players.Find((element) => userId == element.Id);

                if (player != null)
                {
                    player.SetLastTime(DateTime.Now);
                    return Ok("Beat");
                }
            }


            return Ok("Error-Beat");
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

        public JsonResult SendInput(byte input)
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            Player player = ServerEmulator.Players.Get(id);
            Round round = ServerEmulator.Rounds.GetPlayersRound(player);

            string status;

            if (round == null || player == null)
            {
                status = "fake data";
            }
            else
            {
                status = "all working";
                round.AddPlayerInput(player, input);
            }

            return Json(new Input(status));
        }

        /*
                 public JsonResult GetRoundStatus()
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");
            // If win - win data. If lose - lose data. If tie - redirect data or cancelation data.
        }
         */

    }
}
