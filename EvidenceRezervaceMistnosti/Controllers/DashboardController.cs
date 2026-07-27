using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.Models;
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
        public async Task<IActionResult> Reservation(string? search, int? numberOfPeople,
            DateOnly? day, TimeOnly? timeFrom, TimeOnly? timeTo)
        {
            try
            {
                DashboardReservationViewModel model = new()
                {
                    ReservationDashboard = new()
                    {
                        ReservationRows = await _ctx.Reservation
                        .Include(e => e.Room)
                        .Where(e => e.IsActive)
                        .OrderByDescending(e => e.DateReservation)
                        .Select(e => new ReservationRowDTO
                        {
                            UserName = $"{e.Name} {e.LastName}",
                            ReservatioName = e.Name,
                            NumberOfPeople = e.NumberOfPeople,
                            RoomName = e.Room!.Name,
                            DayReservation = e.DateReservation.ToString("dd.MM.yyyy"),
                            TimeReservation = $"{e.TimeFrom.ToString("HH:mm")}-{e.TimeTo.ToString("HH:mm")}"
                        })
                        .ToListAsync()
                    },
                    Search = search,
                    NumberOfPeople = numberOfPeople,
                    Day = day,
                    TimeFrom = timeFrom,
                    TimeTo = timeTo
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
        public async Task<IActionResult> Room(string? search, int? locationId,
            int? equipmentId, int? CapacityFrom, int? CapacityTo)
        {
            try
            {
                //List<RoomDashboardDTO> RoomDashboard = new RoomDashboardDTO
                //{
                //    RoomRows = await _ctx.Room
                //        .Include(e => e.Location)
                //        .Where(e => e.IsActive)
                //        .Select(e => new RoomRowDTO
                //        {
                //            RoomName = e.Name,
                //            Capacity = e.Capacity,
                //            LocationName = e.Location.Name,
                //            EquipmentText = string.Join(",", e.RoomEquipment.Select(e => e.Equipment.Name))
                //        }).ToListAsync(),
                //};

                return View();
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
