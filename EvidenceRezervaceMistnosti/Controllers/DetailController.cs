using EvidenceRezervaceMistnosti.DTO.Select;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                    return View("CstmError", new CstmErrorViewModel
                    {
                        Title = "Při získávání místnosti nastala chyba",
                        Description = "Zkontroluj, zda daná místnost existuje. " +
                        "Pokud chyba přetrvává, kontaktuj administrátora."
                    });
                }

                RoomDetailViewModel model = new()
                {
                    Name = room.Name,
                    Capacity = room.Capacity,
                    SelectedLocationId = room.LocationId,
                    EquipmentSelect = await _ctx.Equipment
                    .AsNoTracking()
                    .Select(e => new EquipmentSelectDTO
                    {
                        EquipmentId = e.EquipmentId,
                        EquipmentName = e.Name
                    })
                    .ToListAsync(),
                    LocationSelect = await _ctx.Location
                    .AsNoTracking()
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
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Při získávání místnosti nastala chyba",
                    Description = "Zkontroluj, zda daná místnost existuje. " +
                       "Pokud chyba přetrvává, kontaktuj administrátora."
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
                    .FirstOrDefaultAsync(r => r.ReservationId == id);

                if (reservation == null)
                {
                    _logger.LogWarning("Při získávání rezervace s ID {id} nastala chyba", id);
                    return View("CstmError", new CstmErrorViewModel
                    {
                        Title = "Při získávání rezervace nastala chyba",
                        Description = "Zkontroluj, zda daná rezervace existuje. " +
                        "Pokud chyba přetrvává, kontaktuj administrátora."
                    });
                }

                ReservationDetailViewModel model = new()
                {
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
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Při získávání rezervace nastala chyba",
                    Description = "Zkontroluj, zda daná rezervace existuje. " +
                       "Pokud chyba přetrvává, kontaktuj administrátora."
                });
            }
        }
    }
}
