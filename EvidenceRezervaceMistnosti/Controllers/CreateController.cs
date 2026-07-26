using Microsoft.AspNetCore.Mvc;

namespace EvidenceRezervaceMistnosti.Controllers
{
    public class CreateController : Controller
    {
        public IActionResult Room()
        {
            return View();
        }
    }
}
