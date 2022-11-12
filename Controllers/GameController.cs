using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.DB;
using RockPaperScissors.Models;
using RockPaperScissors.Server;
using System.Data;
using System.Diagnostics;
using RockPaperScissors.JsonModels;

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

        public IActionResult Disconnect()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            string? userAgent = HttpContext.Session.GetString("user_agent");

            if (userId != null && userAgent != null)
            {
                DataRowCollection dataRow = ServerEmulator.Database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

                object? loggedIn = dataRow[0][5];

                if (loggedIn != null)
                {
                    string loggedInDevice = (string)loggedIn;

                    if (loggedInDevice == userAgent.Replace(" ", ""))
                        ServerEmulator.Database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", ""), new FlexibleDB.Value("id", userId));
                }
            }

            return Ok();
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
                        ServerEmulator.Database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", userAgent), new FlexibleDB.Value("id", userId));
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

            string status;

            int points = (int)playerData[0][2];

            if (level == 0 || points >= ServerEmulator.LevelTable[level])
            {
                ServerEmulator.AddPlayer(id, playerData[0][1].ToString(), level);
                ServerEmulator.AddToQueue(id);

                return Ok("Queue");

            }

            status = "fake data";

            return Ok(status);
        }

        public JsonResult SendInput(int input) 
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            Player player = ServerEmulator.GetPlayer(id);
            Round round = ServerEmulator.GetPlayerRound(player);

            string status;

            if(round == null || player == null)
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

        public IActionResult GetQueueStatus()
        {
            int id = (int)HttpContext.Session.GetInt32("user_id");

            string status;

            Player player = ServerEmulator.GetPlayer(id);
            Round round = ServerEmulator.GetPlayerRound(player);

            if (ServerEmulator.PlayerInQueue(player))
                status = "inQueue";
            else if (round != null)
                return RedirectToAction("Round", "Game");
            else
                status = "fakeData";

            return Ok(status);
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
