using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.DB;
using RockPaperScissors.Models;
using System.Data;
using System.Diagnostics;

namespace RockPaperScissors.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly FlexibleDB _database;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
            _database = new FlexibleDB("rock_paper_scissors");
        }

        public IActionResult Index(int id, string username)
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
                return RedirectToAction("Index", "Home", new { });

            if (!CheckAuth())
                return RedirectToAction("Index", "Home", new { });

            return Ok();
        }

        public IActionResult Disconnect()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            string? userAgent = HttpContext.Session.GetString("user_agent");

            if (userId != null && userAgent != null)
            {
                DataRowCollection dataRow = _database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

                object? loggedIn = dataRow[0][5];

                if (loggedIn != null)
                {
                    string loggedInDevice = (string)loggedIn;

                    if (loggedInDevice == userAgent.Replace(" ", ""))
                        _database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", ""), new FlexibleDB.Value("id", userId));
                }
            }

            return Ok();
        }

        public bool CheckBanned()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");

            if (userId != null)
            {
                DataRowCollection dataRow = _database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

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

        public bool CheckAuth()
        {
            string userAgent = Request.Headers.UserAgent.ToString();
            HttpContext.Session.SetString("user_agent", userAgent);
           
            int? userId = HttpContext.Session.GetInt32("user_id");

            if (userId != null)
            {
                DataRowCollection dataRow = _database.CreateGetRequest("users", new FlexibleDB.Value[1] { new FlexibleDB.Value("id", userId) });

                object? loggedIn = dataRow[0][5];

                if(loggedIn != null)
                {
                    string loggedInDevice = (string)loggedIn;

                    if (loggedInDevice == "")
                    {
                        _database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", userAgent), new FlexibleDB.Value("id", userId));
                        return true;
                    }
                    else if(loggedInDevice == userAgent.Replace(" ", ""))
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
                    _database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", userAgent), new FlexibleDB.Value("id", userId));
                    return true;
                }
            }
            else
            {
                // Error

                return false;
            }
        }
    }
}
