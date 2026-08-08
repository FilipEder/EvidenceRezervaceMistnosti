using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.DTO.Select;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Filter;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
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
                filter ??= new ReservationFilterViewModel();

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
                        (filter.RoomId == null || e.RoomId == filter.RoomId) &&
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
                            DayReservation = e.DateReservation,
                            TimeReservation = $"{e.TimeFrom.ToString("HH:mm")}-{e.TimeTo.ToString("HH:mm")}",
                        })
                        .ToListAsync()
                    },
                    Rooms = await _ctx.Reservation
                        .AsNoTracking()
                        .Where(e => e.IsActive)
                        .Select(e => new RoomSelectDTO
                        {
                            RoomId = e.RoomId,
                            RoomName = e.Room!.Name,
                            Capacity = e.Room.Capacity
                        })
                        .Distinct()
                        .OrderBy(e => e.RoomName)
                        .ThenBy(e => e.RoomId)
                        .ToListAsync(),
                    Search = filter.Search,
                    RoomId = filter.RoomId,
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
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "An error occurred while loading the dashboard",
                    Description = "Contact the administrator."
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
                            EquipmentKeys = e.RoomEquipment
                                .OrderBy(re => re.EquipmentId)
                                .Select(re => re.Equipment.Name)
                                .ToList()
                        }).ToListAsync(),
                    },
                    Equipments = await _ctx.Equipment
                        .AsNoTracking()
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.EquipmentId)
                        .Select(e => new EquipmentSelectDTO
                        {
                            EquipmentId = e.EquipmentId,
                            EquipmentName = e.Name
                        })
                        .ToListAsync(),
                    Locations = await _ctx.Location
                        .AsNoTracking()
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.LocationId)
                        .Select(e => new LocationSelectDTO
                        {
                            LocationId = e.LocationId,
                            LocationName = e.Name
                        })
                        .ToListAsync(),
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
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("CstmError", new CstmErrorViewModel
                {
                    Title = "An error occurred while loading the dashboard",
                    Description = "Contact the administrator."
                });
            }
        }
    }
}
