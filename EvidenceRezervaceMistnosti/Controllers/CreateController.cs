using EvidenceRezervaceMistnosti.DTO.Form;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.DTO.Select;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CreateController : Controller
    {
        private readonly ILogger<CreateController> _logger;
        private readonly ReservationContext _ctx;
        public CreateController(ILogger<CreateController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        [Route("create/room")]
        public async Task<IActionResult> CreateRoom()
        {
            try
            {
                RoomFormDTO model = new()
                {
                    EquipmentSelect = await _ctx.Equipment
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.EquipmentId)
                    .Select(e => new EquipmentSelectDTO
                    {
                        EquipmentId = e.EquipmentId,
                        EquipmentName = e.Name,
                    }).ToListAsync(),

                    LocationSelect = await _ctx.Location
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .Select(e => new LocationSelectDTO
                    {
                        LocationId = e.LocationId,
                        LocationName = e.Name,
                    }).ToListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při načítání formuláře pro vytvoření místnosti");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Form could not be loaded",
                    Description = "Reload the page. If the error persists, contact the administrator."
                });
            }
        }
        [HttpGet]
        [Route("create/reservation")]
        public async Task<IActionResult> CreateReservation()
        {
            try
            {
                ReservationFormDTO model = new()
                {
                    Rooms = await _ctx.Room
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .Select(e => new RoomSelectDTO
                    {
                        RoomId = e.RoomId,
                        RoomName = e.Name,
                        Capacity = e.Capacity
                    }).ToListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při načítání formuláře pro vytvoření rezervace");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Form could not be loaded",
                    Description = "Reload the page. If the error persists, contact the administrator."
                });
            }
        }
    }
}
