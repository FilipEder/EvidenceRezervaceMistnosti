using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using EvidenceRezervaceMistnosti.Models.Shared;

namespace EvidenceRezervaceMistnosti.Controllers
{
    public class DetailController : Controller
    {
        private readonly ILogger<DetailController> _logger;
        private readonly ReservationContext _ctx;
        public DetailController(ILogger<DetailController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }
        public async Task<IActionResult> Room(int id)
        {
            try
            {
                Room? room = await _ctx.Room
                    .FindAsync(id);

                if (room == null)
                {
                    _logger.LogWarning("Při získávání místnosti s ID {id} nastala chyba", id);
                    return View("CstmError", new CstmErrorViewModel
                    {
                        Title = "Při získávání místnosti nastala chyba",
                        Description = "Zkontroluj, zda daná místnost existuje. " +
                        "Pokud chyba přetrvává, kontaktuj administrátora."
                    });
                }

                _logger.LogInformation("Získávání detailů místnosti proběhlo úspěšně.");
                return View(room);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Chyba při stavění detailu místnosti s {id}");
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Při získávání místnosti nastala chyba",
                    Description = "Zkontroluj, zda daná místnost existuje. " +
                       "Pokud chyba přetrvává, kontaktuj administrátora."
                });
            }
        }
    }
}
