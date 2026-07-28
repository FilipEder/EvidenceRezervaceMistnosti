using EvidenceRezervaceMistnosti.DTO.Requests;
using EvidenceRezervaceMistnosti.DTO.Response;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EvidenceRezervaceMistnosti.API
{
    [Route("reservations")]
    [ApiController]
    public class ReservationApiController : ControllerBase
    {
        private readonly ReservationContext _ctx;
        private readonly ILogger<ReservationApiController> _logger;
        public ReservationApiController(ReservationContext ctx, ILogger<ReservationApiController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationResponse>>> Get([FromQuery] DateOnly? date)
        {
            try
            {
                List<Reservation>? reservations = await _ctx.Reservation
                    .Include(r => r.Room)
                    .AsNoTracking()
                    .Where(e => e.IsActive)
                    .ToListAsync();

                if(date.HasValue)
                {
                    reservations = reservations
                        .Where(r => r.DateReservation == date.Value)
                        .ToList();
                }

                if (reservations.Count == 0)
                {
                    _logger.LogWarning("Žádné rezervace nebyly nalezeny");
                    return Problem(
                            type: "",
                            statusCode: StatusCodes.Status404NotFound,
                            title: "Žádné rezervace nebyly nalezeny",
                            detail: "Vytvořte novou rezervaci",
                            instance: HttpContext.Request.Path
                        );
                };

                List<ReservationResponse> response = reservations.Select(r => new ReservationResponse
                {
                    ReservationId = r.ReservationId,
                    ReservationName = r.Name,
                    LastName = r.LastName,
                    Email = r.Email,
                    DateReservation = r.DateReservation.ToString("dd.MM.yyyy"),
                    TimeFrom = r.TimeFrom.ToString("HH:mm"),
                    TimeTo = r.TimeTo.ToString("HH:mm"),
                    NumberOfPeople = r.NumberOfPeople,
                    Description = r.Description,
                    RoomId = r.RoomId,
                    RoomName = r.Room!.Name,
                    RoomCapacity = r.Room!.Capacity,
                    LocationId = r.Room!.LocationId,
                    ReservationIsActive = r.IsActive,
                    RoomIsActive = r.Room!.IsActive
                }).ToList();

                _logger.LogInformation("Získání rezervací proběhlo úspěšně");
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání rezervací");
                return Problem(
                    type: "",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Chyba při získávání rezervací",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    instance: HttpContext.Request.Path
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponse>> GetById(int id)
        {
            try
            {
                Reservation? reservation = await _ctx.Reservation
                    .Include(e => e.Room)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.ReservationId == id && e.IsActive);

                if (reservation == null)
                {
                    _logger.LogWarning("Rezervace nebyla nalezena");
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Rezervace nebyla nalezena",
                        detail: "Zkuste jiný identifikátor rezervace.",
                        instance: HttpContext.Request.Path
                    );
                }

                ReservationResponse response = new ReservationResponse
                {
                    ReservationId = reservation.ReservationId,
                    ReservationName = reservation.Name,
                    LastName = reservation.LastName,
                    Email = reservation.Email,
                    DateReservation = reservation.DateReservation.ToString("dd.MM.yyyy"),
                    TimeFrom = reservation.TimeFrom.ToString("HH:mm"),
                    TimeTo = reservation.TimeTo.ToString("HH:mm"),
                    NumberOfPeople = reservation.NumberOfPeople,
                    Description = reservation.Description,
                    RoomId = reservation.RoomId,
                    RoomName = reservation.Room!.Name,
                    RoomCapacity = reservation.Room!.Capacity,
                    LocationId = reservation.Room!.LocationId,
                    ReservationIsActive = reservation.IsActive,
                    RoomIsActive = reservation.Room!.IsActive
                };

                _logger.LogInformation("Získání rezervace proběhlo úspěšně {reservationId}", reservation.ReservationId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání rezervace {reservationId}", id);
                return Problem(
                    type: "",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Chyba při získávání rezervace",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    instance: HttpContext.Request.Path
                );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ReservationRequest request)
        {
            await using var transaction = await _ctx.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                // Validation
                if (request.TimeTo < request.TimeFrom)
                {
                    _logger.LogWarning("Čas není ve správném formátu: {timeFrom} - {timeTo}", request.TimeFrom, request.TimeTo);
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Čas není ve správném formátu",
                        detail: "Čas do musí být po čase od, např. (14:30 - 16:30)",
                        instance: HttpContext.Request.Path
                    );
                }

                Reservation? conflictReservation = await _ctx.Reservation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e =>
                    e.RoomId == request.RoomId &&
                    e.DateReservation == request.DateReservation &&
                    (e.TimeFrom < request.TimeTo &&
                        request.TimeFrom < e.TimeTo));

                if (conflictReservation != null)
                {
                    _logger.LogWarning("Termín rezervace je obsazený: {timeFrom} - {timeTo}", request.TimeFrom, request.TimeTo);
                    return Problem(
                    type: "",
                    title: "Termín rezervace je obsazený",
                    statusCode: StatusCodes.Status409Conflict,
                    detail:
                        $"Požadovaný čas " +
                        $"{request.TimeFrom.ToString("HH:mm")}-{request.TimeTo.ToString("HH:mm")} " +
                        $"se překrývá s rezervací " +
                        $"{conflictReservation.TimeFrom.ToString("HH:mm")}-" +
                        $"{conflictReservation.TimeTo.ToString("HH:mm")}.",
                    instance: HttpContext.Request.Path
                    );
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
                    RoomId = request.RoomId,
                    IsActive = true
                };
                _ctx.Reservation.Add(reservation);
                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Rezervace byla vytvořena úspěšně {reservationId}", reservation.ReservationId);
                return Created();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Chyba při vytváření rezervace");
                return Problem(
                    type: "",
                    title: "Chyba při vytváření rezervace",
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    instance: HttpContext.Request.Path
                    );
            }
        }
    }
}
