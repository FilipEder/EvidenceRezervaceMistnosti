using EvidenceRezervaceMistnosti.DTO.Select;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DetailController : Controller
    {
        private readonly ILogger<DetailController> _logger;
        private readonly ReservationContext _ctx;
        public DetailController(ILogger<DetailController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        [Route("detail/room/{id}")]
        public async Task<IActionResult> RoomDetail(int id)
        {
            try
            {
                Room? room = await _ctx.Room
                    .FindAsync(id);

                if (room == null)
                {
                    _logger.LogWarning("Při získávání místnosti s ID {id} nastala chyba", id);
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return View("CstmError", new CstmErrorViewModel
                    {
                        Title = "An error occurred while retrieving the room",
                        Description = "Check whether the room exists. If the error persists, contact the administrator."
                    });
                }

                RoomDetailViewModel model = new()
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    SelectedLocationId = room.LocationId,
                    SelectedGearId = await _ctx.RoomEquipment
                        .AsNoTracking()
                        .Where(item => item.RoomId == room.RoomId)
                        .Select(item => item.EquipmentId)
                        .ToListAsync(),
                    EquipmentSelect = await _ctx.Equipment
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.EquipmentId)
                    .Select(e => new EquipmentSelectDTO
                    {
                        EquipmentId = e.EquipmentId,
                        EquipmentName = e.Name
                    })
                    .ToListAsync(),
                    LocationSelect = await _ctx.Location
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .Select(e => new LocationSelectDTO
                    {
                        LocationId = e.LocationId,
                        LocationName = e.Name
                    })
                    .ToListAsync()
                };


                _logger.LogInformation("Získávání detailů místnosti proběhlo úspěšně.");
                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání místnosti s {id}");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "An error occurred while retrieving the room",
                    Description = "Check whether the room exists. If the error persists, contact the administrator."
                });
            }
        }

        [HttpGet]
        [Route("detail/reservation/{id}")]
        public async Task<IActionResult> ReservationDetail(int id)
        {
            try
            {
                Reservation? reservation = await _ctx.Reservation
                    .AsNoTracking()
                    .Include(r => r.Room)
                    .FirstOrDefaultAsync(r => r.ReservationId == id && r.IsActive);

                if (reservation == null)
                {
                    _logger.LogWarning("Při získávání rezervace s ID {id} nastala chyba", id);
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return View("CstmError", new CstmErrorViewModel
                    {
                        Title = "An error occurred while retrieving the reservation",
                        Description = "Check whether the reservation exists. If the error persists, contact the administrator."
                    });
                }

                ReservationDetailViewModel model = new()
                {
                    ReservationId = reservation.ReservationId,
                    Name = reservation.Name,
                    LastName = reservation.LastName,
                    Email = reservation.Email,
                    SelectedRoomId = reservation.RoomId,
                    NumberOfPeople = reservation.NumberOfPeople,
                    Day = reservation.DateReservation,
                    TimeFrom = reservation.TimeFrom,
                    TimeTo = reservation.TimeTo,
                    Description = reservation.Description,
                    Rooms = await _ctx.Room
                        .AsNoTracking()
                        .Where(r => r.IsActive)
                        .Select(r => new RoomSelectDTO
                        {
                            RoomId = r.RoomId,
                            RoomName = r.Name,
                            Capacity = r.Capacity
                        })
                        .ToListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získání detailu rezervace s {id}");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "An error occurred while retrieving the reservation",
                    Description = "Check whether the reservation exists. If the error persists, contact the administrator."
                });
            }
        }
    }
}
