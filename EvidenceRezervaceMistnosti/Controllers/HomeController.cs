using EvidenceRezervaceMistnosti.DTO.PartialView;
using EvidenceRezervaceMistnosti.Models;
using EvidenceRezervaceMistnosti.Models.Filter;
using EvidenceRezervaceMistnosti.Models.Shared;
using EvidenceRezervaceMistnosti.Views.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index(string? search, RoomFilterModel roomFilter, 
            ReservationFilterModel reservationModel,  bool reservation = true)
        {
            try
            {
                HomepageViewModel model = new()
                {
                    ReservationOn = reservation,
                };

                if (reservation)
                {
                    List<ReservationRowDTO> reservationRows = await _ctx.Reservation
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
                        .ToListAsync();

                    model.ReservationDashboard = new ReservationDashboardDTO
                    {
                        ReservationRows = reservationRows
                    };
                }
                else
                {
                    List<RoomRowDTO> roomRows = await _ctx.Room
                        .Include(e => e.Location)
                        .Where(e => e.IsActive)
                        .Select(e => new RoomRowDTO
                        {
                            RoomName = e.Name,
                            Capacity = e.Capacity,
                            LocationName = e.Location.Name,
                            EquipmentText = string.Join(",", e.RoomEquipment.Select(e => e.Equipment.Name))
                        }).ToListAsync();
                    model.RoomDashboard = new RoomDashboardDTO
                    {
                        RoomRows = roomRows
                    };
                }

                _logger.LogInformation("Hlavní stránka se úspěšně načetla.");
                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Naskytla se chyba při načítání hlavní stránky");
                return View("CstmError", new CstmErrorViewModel {
                    Title = "Při načítání hlavní stránky došlo k chybě.",
                    Description = "Kontaktujte administrátora."
                });
            }
        }
    }
}
