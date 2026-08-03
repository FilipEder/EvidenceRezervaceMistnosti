using EvidenceRezervaceMistnosti.DTO.Form;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.DTO.Select;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Controllers
{
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
                    .Select(e => new EquipmentSelectDTO
                    {
                        EquipmentId = e.EquipmentId,
                        EquipmentName = e.Name,
                    }).ToListAsync(),

                    LocationSelect = await _ctx.Location
                    .AsNoTracking()
                    .Select(e => new LocationSelectDTO
                    {
                        LocationId = e.LocationId,
                        LocationName = e.Name,
                    }).ToListAsync()
                };

                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "P");
                return View();
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
                    .Select(e => new RoomSelectDTO
                    {
                        RoomId = e.RoomId,
                        RoomName = e.Name,
                        Capacity = e.Capacity
                    }).ToListAsync()
                };

                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "P");
                return View();
            }
        }
    }
}
