using EvidenceRezervaceMistnosti.DTO;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.API
{
    [Route("reservations")]
    [ApiController]
    public class ReservationApiController : ControllerBase
    {
        private readonly ILogger<ReservationApiController> _logger;
        private readonly ReservationContext _ctx;
        public ReservationApiController(ILogger<ReservationApiController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> Get([FromQuery] DateOnly? date)
        {
            try
            {
                List<Reservation>? reservations = await _ctx.Reservation
                    .Include(r => r.Room)
                    .AsNoTracking().ToListAsync();

                if(date.HasValue)
                {
                    reservations = reservations.Where(r => r.DateReservation == date.Value).ToList();
                }

                if (reservations.Count == 0)
                {
                    _logger.LogWarning("No reservations found");
                    return NotFound(new
                    {
                        message = $"Žádné rezervace nebyly nalezeny"
                    });
                };

                return Ok(reservations);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error: fetching reservations");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(int id)
        {
            try
            {
                Reservation? reservation = await _ctx.Reservation.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound(new
                    {
                        message = "Rezervace nebyla nalezena"
                    });
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: fetching reservation by ID");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> Post(ReservationDTORequest request)
        {
            try
            {
                // Validation

                if(request.TimeTo < request.TimeFrom)
                {
                    return Conflict(new
                    {
                        message = "Čas (od) a (do) musí být ve formátu např. (od 14:30 - do 16:30) " +
                        "nemůže být (od 12:30 do 9:00)"
                    });
                }

                Reservation? conflictReservation = await _ctx.Reservation
                    .Where(e => e.DateReservation == request.DateReservation &&
                    (e.TimeFrom <= request.TimeFrom && e.TimeTo >= request.TimeFrom ||
                    e.TimeFrom <= request.TimeTo && e.TimeTo >= request.TimeTo ||
                    request.TimeFrom <= e.TimeFrom && request.TimeTo >= e.TimeFrom ||
                    request.TimeFrom <= e.TimeTo && request.TimeTo >= e.TimeTo))
                    .FirstOrDefaultAsync();

                if (conflictReservation != null)
                {
                    return Conflict(new
                    {
                        message = $"Tento čas {request.TimeFrom.ToString("HH:mm")}-{request.TimeTo.ToString("HH:mm")} se přelína s časem " +
                        $"jiné rezervace {conflictReservation.TimeFrom.ToString("HH:mm")}-{conflictReservation.TimeTo.ToString("HH:mm")}, změň čas či datum"
                    });
                }

                Reservation reservation = new Reservation
                {
                    Name = request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    DateReservation = request.DateReservation,
                    TimeFrom = request.TimeFrom,
                    TimeTo = request.TimeTo,
                    NumberOfPeople = request.NumberOfPeople,
                    Description = request.Description,
                    RoomId = request.RoomId
                };
                _ctx.Reservation.Add(reservation);
                await _ctx.SaveChangesAsync();
                return CreatedAtAction(nameof(Post), new { id = reservation.ReservationId }, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: creating reservation");
                return StatusCode(500);
            }
        }
    }
}
