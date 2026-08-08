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
                List<Reservation> reservations = await _ctx.Reservation
                    .AsNoTracking()
                    .Include(r => r.Room)
                    .Where(e => e.IsActive)
                    .ToListAsync();

                if (date.HasValue)
                {
                    reservations = reservations.Where(r => r.DateReservation == date.Value).ToList();
                }

                if (reservations.Count == 0)
                {
                    _logger.LogWarning("Žádné rezervace nebyly nalezeny");
                    return Problem(
                            type: "",
                            statusCode: StatusCodes.Status200OK,
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
                    DateReservation = r.DateReservation,
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

        [HttpGet("availability")]
        public async Task<ActionResult> GetAvailability(
            [FromQuery] int roomId,
            [FromQuery] DateOnly date,
            [FromQuery] int? excludeReservationId)
        {
            bool roomExists = await _ctx.Room
                .AsNoTracking()
                .AnyAsync(room => room.RoomId == roomId && room.IsActive);

            if (!roomExists)
            {
                return Problem(
                    type: "",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Místnost nebyla nalezena",
                    detail: $"Aktivní místnost s ID {roomId} neexistuje.",
                    instance: HttpContext.Request.Path
                );
            }

            var reservations = await _ctx.Reservation
                .AsNoTracking()
                .Where(reservation =>
                    reservation.IsActive &&
                    reservation.RoomId == roomId &&
                    reservation.DateReservation == date &&
                    (!excludeReservationId.HasValue || reservation.ReservationId != excludeReservationId.Value))
                .OrderBy(reservation => reservation.TimeFrom)
                .Select(reservation => new
                {
                    reservation.ReservationId,
                    reservation.TimeFrom,
                    reservation.TimeTo
                })
                .ToListAsync();

            return Ok(reservations.Select(reservation => new
            {
                reservation.ReservationId,
                TimeFrom = reservation.TimeFrom.ToString("HH:mm"),
                TimeTo = reservation.TimeTo.ToString("HH:mm")
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponse>> GetById(int id)
        {
            try
            {
                Reservation? reservation = await _ctx.Reservation
                    .AsNoTracking()
                    .Include(e => e.Room)
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
                    DateReservation = reservation.DateReservation,
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
                if (request.TimeTo <= request.TimeFrom)
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

                if (!IsHalfHour(request.TimeFrom) || !IsHalfHour(request.TimeTo))
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatný čas rezervace",
                        detail: "Začátek i konec rezervace musí být po 30 minutách.",
                        instance: HttpContext.Request.Path
                    );
                }

                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                if (request.DateReservation < today || request.DateReservation > today.AddYears(1))
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatné datum rezervace",
                        detail: "Datum rezervace musí být ode dneška nejvýše za jeden rok.",
                        instance: HttpContext.Request.Path
                    );
                }

                Room? requestedRoom = await _ctx.Room
                    .AsNoTracking()
                    .FirstOrDefaultAsync(room => room.RoomId == request.RoomId && room.IsActive);

                if (requestedRoom == null || request.NumberOfPeople > requestedRoom.Capacity)
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatná místnost nebo počet osob",
                        detail: "Vyberte aktivní místnost a nepřekračujte její kapacitu.",
                        instance: HttpContext.Request.Path
                    );
                }

                Reservation? conflictReservation = await _ctx.Reservation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e =>
                    e.IsActive &&
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

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ReservationRequest request)
        {
            await using var transaction = await _ctx.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                Reservation? reservation = await _ctx.Reservation
                    .FirstOrDefaultAsync(item => item.ReservationId == id && item.IsActive);

                if (reservation == null)
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Rezervace nebyla nalezena",
                        detail: $"Aktivní rezervace s ID {id} neexistuje.",
                        instance: HttpContext.Request.Path
                    );
                }

                if (request.TimeTo <= request.TimeFrom ||
                    !IsHalfHour(request.TimeFrom) ||
                    !IsHalfHour(request.TimeTo))
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatný čas rezervace",
                        detail: "Konec musí být po začátku a oba časy musí být po 30 minutách.",
                        instance: HttpContext.Request.Path
                    );
                }

                Room? requestedRoom = await _ctx.Room
                    .AsNoTracking()
                    .FirstOrDefaultAsync(room => room.RoomId == request.RoomId && room.IsActive);

                if (requestedRoom == null || request.NumberOfPeople > requestedRoom.Capacity)
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatná místnost nebo počet osob",
                        detail: "Vyberte aktivní místnost a nepřekračujte její kapacitu.",
                        instance: HttpContext.Request.Path
                    );
                }

                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                if (request.DateReservation < today || request.DateReservation > today.AddYears(1))
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Neplatné datum rezervace",
                        detail: "Datum rezervace musí být ode dneška nejvýše za jeden rok.",
                        instance: HttpContext.Request.Path
                    );
                }

                Reservation? conflictReservation = await _ctx.Reservation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item =>
                        item.IsActive &&
                        item.ReservationId != id &&
                        item.RoomId == request.RoomId &&
                        item.DateReservation == request.DateReservation &&
                        item.TimeFrom < request.TimeTo &&
                        request.TimeFrom < item.TimeTo);

                if (conflictReservation != null)
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status409Conflict,
                        title: "Termín rezervace je obsazený",
                        detail: $"Požadovaný čas se překrývá s rezervací " +
                            $"{conflictReservation.TimeFrom:HH\\:mm}-{conflictReservation.TimeTo:HH\\:mm}.",
                        instance: HttpContext.Request.Path
                    );
                }

                reservation.Name = request.Name.Trim();
                reservation.LastName = request.LastName.Trim();
                reservation.Email = request.Email.Trim();
                reservation.RoomId = request.RoomId;
                reservation.NumberOfPeople = request.NumberOfPeople;
                reservation.DateReservation = request.DateReservation;
                reservation.TimeFrom = request.TimeFrom;
                reservation.TimeTo = request.TimeTo;
                reservation.Description = request.Description?.Trim();

                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Rezervace s ID {id} byla upravena", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Chyba při úpravě rezervace s ID {id}", id);
                return Problem(
                    type: "",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Chyba při úpravě rezervace",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    instance: HttpContext.Request.Path
                );
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Reservation? reservation = await _ctx.Reservation
                    .FirstOrDefaultAsync(item => item.ReservationId == id && item.IsActive);

                if (reservation == null)
                {
                    return Problem(
                        type: "",
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Rezervace nebyla nalezena",
                        detail: $"Aktivní rezervace s ID {id} neexistuje nebo již byla zrušena.",
                        instance: HttpContext.Request.Path
                    );
                }

                reservation.IsActive = false;
                await _ctx.SaveChangesAsync();

                _logger.LogInformation("Rezervace s ID {id} byla zrušena", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při rušení rezervace s ID {id}", id);
                return Problem(
                    type: "",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Chyba při rušení rezervace",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    instance: HttpContext.Request.Path
                );
            }
        }

        private static bool IsHalfHour(TimeOnly time)
        {
            return time.Minute % 30 == 0 && time.Second == 0;
        }
    }
}
