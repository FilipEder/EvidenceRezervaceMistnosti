using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Filter;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ReservationContext _ctx;
        public DashboardController(ILogger<DashboardController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        [Route("")]
        [Route("dashboard/reservation")]
        public async Task<IActionResult> ReservationDashboard([FromQuery] ReservationFilterViewModel? filter)
        {
            try
            {
                ReservationDashboardViewModel model = new()
                {
                    ReservationDashboard = new()
                    {
                        ReservationRows = await _ctx.Reservation
                        .AsNoTracking()
                        .Include(e => e.Room)
                        .Where(e => 
                        (e.IsActive) && 
                        (filter.Search == null ||
                            e.Name.Contains(filter.Search) || 
                            e.LastName.Contains(filter.Search) ||
                            e.Room!.Name.Contains(filter.Search)) &&
                        (filter.NumberOfPeople == null || e.NumberOfPeople == filter.NumberOfPeople) &&
                        (filter.Day == null || e.DateReservation == filter.Day) &&
                        (filter.TimeFrom == null || e.TimeFrom >= filter.TimeFrom) &&
                        (filter.TimeTo == null || e.TimeTo <= filter.TimeTo))
                        .OrderByDescending(e => e.DateReservation)
                        .Select(e => new ReservationRowDTO
                        {
                            ReservationId = e.ReservationId,
                            UserName = $"{e.Name} {e.LastName}",
                            ReservatioName = e.Name,
                            NumberOfPeople = e.NumberOfPeople,
                            RoomName = e.Room!.Name,
                            DayReservation = e.DateReservation.ToString("dd.MM.yyyy"),
                            TimeReservation = $"{e.TimeFrom.ToString("HH:mm")}-{e.TimeTo.ToString("HH:mm")}",
                        })
                        .ToListAsync()
                    },
                    Search = filter.Search,
                    NumberOfPeople = filter.NumberOfPeople,
                    Day = filter.Day,
                    TimeFrom = filter.TimeFrom,
                    TimeTo = filter.TimeTo
                };

                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Naskytla se chyba při načítání hlavní stránky");
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Při načítání hlavní stránky došlo k chybě.",
                    Description = "Kontaktujte administrátora."
                });
            }
        }

        [HttpGet]
        [Route("dashboard/room")]
        public async Task<IActionResult> RoomDashboard([FromQuery] RoomFilterViewModel filter)
        {
            try
            {

                RoomDashboardViewModel model = new() {
                    RoomDashboard = new()
                    {
                        RoomRows = await _ctx.Room
                        .AsNoTracking()
                        .Include(e => e.Location)
                        .Where(e => 
                        (e.IsActive) &&
                        (filter.Search == null || e.Name.Contains(filter.Search)) && 
                        (filter.LocationId == null || e.LocationId == filter.LocationId) &&
                        (filter.EquipmentId == null || e.RoomEquipment.Any(re => re.EquipmentId == filter.EquipmentId)) &&
                        (filter.CapacityFrom == null || e.Capacity >= filter.CapacityFrom) &&
                        (filter.CapacityTo == null || e.Capacity <= filter.CapacityTo))
                        .Select(e => new RoomRowDTO
                        {
                            RoomId = e.RoomId,
                            RoomName = e.Name,
                            Capacity = e.Capacity,
                            LocationName = e.Location.Name,
                            EquipmentText = string.Join(",", e.RoomEquipment.Select(e => e.Equipment.Name))
                        }).ToListAsync(),
                    },
                    Search = filter.Search,
                    LocationId = filter.LocationId,
                    EquipmentId = filter.EquipmentId,
                    CapacityFrom = filter.CapacityFrom,
                    CapacityTo = filter.CapacityTo
                };

                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Naskytla se chyba při načítání hlavní stránky");
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "Při načítání hlavní stránky došlo k chybě.",
                    Description = "Kontaktujte administrátora."
                });
            }
        }
    }
}
