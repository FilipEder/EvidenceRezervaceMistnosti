using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EvidenceRezervaceMistnosti.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ReservationContext _ctx;
        public HomeController(ILogger<HomeController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
