using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Models;
using System.Diagnostics;

namespace RockPaperScissors.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int id, string username)
        {
            Console.WriteLine(id);
            Console.WriteLine(username);

            return View();
        }

        public IActionResult PlayFree()
        {
            return View(new PlayFreeModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public JsonResult GetNames()
        {
            var names = new string[3]
            {
                "Fuck",
                "You",
                "Slave"
            };

            
            return new JsonResult(Ok(names));
        }

        [HttpPost]
        public JsonResult PostName(string name)
        {
            return new JsonResult(Ok());
        }
    }
}
