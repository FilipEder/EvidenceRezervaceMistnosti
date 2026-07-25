using EvidenceRezervaceMistnosti.DTO.Requests;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using EvidenceRezervaceMistnosti.DTO.Response;
using System.Net;

namespace EvidenceRezervaceMistnosti.API
{
    [Route("rooms")]
    [ApiController]
    public class RoomApiController : ControllerBase
    {
        private readonly ILogger<RoomApiController> _logger;
        private readonly ReservationContext _ctx;
        public RoomApiController(ILogger<RoomApiController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoomResponse>>> Get()
        {
            try
            {
                List<Room>? rooms = await _ctx.Room
                    .Include(r => r.Location)
                    .AsNoTracking()
                    .ToListAsync();

                if (rooms == null)
                {
                    _logger.LogWarning("Žádné místnosti nebyly nalezeny");
                    return Problem(
                        type: "",
                        title: "Žádné místnosti nebyly nalezeny",
                        detail: "",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                };

                List<RoomResponse> roomResponses = rooms.Select(r => new RoomResponse
                {
                    RoomId = r.RoomId,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    LocationId = r.LocationId,
                    LocationName = r.Location!.Name,
                    RoomIsActive = r.IsActive,
                    LocationIsActive = r.Location.IsActive 
                }).ToList();

                _logger.LogInformation("Místnosti byly úspěšně načteny");
                return Ok(roomResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získavání místností");
                return Problem(
                        type: "",
                        title: "Chyba při získavání místností",
                        detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                        statusCode: StatusCodes.Status500InternalServerError,
                        instance: HttpContext.Request.Path
                );
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomResponse>> GetById(int id)
        {
            try
            {
                Room? room = await _ctx.Room
                    .Include(e => e.Location)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.RoomId == id && e.IsActive);

                if (room == null)
                {
                    _logger.LogWarning("Žádna místnost se nenašla s ID {id}", id);
                    return Problem(
                        type: "",
                        title: "Daná místnost se nenašla",
                        detail: $"Místnost s ID {id} neexistuje.",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                };

                RoomResponse response = new()
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    LocationId = room.LocationId,
                    LocationName = room.Location!.Name,
                    RoomIsActive = room.IsActive,
                    LocationIsActive = room.Location.IsActive
                };

                _logger.LogInformation("Místnost s ID {id} byla úspěšně načtena", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání místnosti s ID {id}", id);
                return Problem(
                        type: "",
                        title: "Chyba při získávání místnosti",
                        detail: $"Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                        statusCode: StatusCodes.Status500InternalServerError,
                        instance: HttpContext.Request.Path
                    );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RoomRequest request)
        {
            using var transaction = await _ctx.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                bool roomDuplicate = await _ctx.Room
                    .AsNoTracking()
                    .AnyAsync(e => e.Name == request.Name);

                if (roomDuplicate)
                {
                    _logger.LogWarning("Umístění s názvem {name} již existuje.", request.Name);
                    return Problem(
                        type: "",
                        title: "Umístění s tímto názvem již existuje",
                        detail: $"Umístění s názvem {request.Name} již existuje",
                        statusCode: StatusCodes.Status409Conflict,
                        instance: HttpContext.Request.Path
                    );
                }

                // Check Location Validate
                bool locationExist = await _ctx.Location
                    .AnyAsync(l => l.LocationId == request.LocationId && l.IsActive);

                if (!locationExist)
                {
                    _logger.LogWarning("Umístění s ID {locationId} neexistuje", request.LocationId);
                    return Problem(
                        type: "",
                        title: "Umístění neexistuje",
                        detail: $"Umístění s ID {request.LocationId} neexistuje",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                }

                // Check Gear Validate

                if(request.GearIds != null)
                {
                    int[] equip = await _ctx.Equipment
                        .Select(e => e.EquipmentId).ToArrayAsync();

                    int[] notMatchEquipId = request.GearIds
                        .Where(e => !equip.Contains(e.GearId))
                        .Select(e => e.GearId).ToArray();

                    if(notMatchEquipId.Length > 0)
                    {
                        string notMatchstr = string.Join(',', notMatchEquipId);
                        _logger.LogWarning("Vybavení s ID [{}] neexistuje", notMatchstr);
                        return Problem(
                            type:"",
                            title: "Zadané vybavení neexistuje",
                            detail: $"Vybavení s ID {notMatchstr} neexistuje",
                            statusCode: StatusCodes.Status409Conflict,
                            instance: HttpContext.Request.Path
                        );
                    }
                }

                Room room = new()
                {
                    Name = request.Name.Trim(),
                    Capacity = request.Capacity,
                    LocationId = request.LocationId,
                    IsActive = true
                };

                _ctx.Room.Add(room);

                List<RoomEquipment> roomEquipment = request.GearIds?.Select(e => new RoomEquipment
                {
                    EquipmentId = e.GearId,
                    RoomId = room.RoomId,
                    Count = e.Count
                }).ToList() ?? new List<RoomEquipment>();

                _ctx.RoomEquipment.AddRange(roomEquipment);
                await _ctx.SaveChangesAsync();

                await _ctx.Database.CommitTransactionAsync();

                _logger.LogInformation("Vytvořena nová místnost s ID {id}", room.RoomId);
                return Created();
            }
            catch (Exception ex)
            {
                await _ctx.Database.RollbackTransactionAsync();
                _logger.LogError(ex, "Error: creating room");
                return Problem(
                    type: "",
                    title: "Chyba při vytváření místnosti",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path
                );
            }
        }

        [HttpGet("{id:int}/reservations")]
        public async Task<ActionResult<List<ReservationResponse>>> GetReservations(int id)
        {
            try
            {
                bool RoomExist = await _ctx.Room
                    .AnyAsync(e => e.IsActive & e.RoomId == id);

                if (!RoomExist)
                {
                    _logger.LogWarning("Místnost s ID {id} neexistuje", id);
                    return Problem(
                        type: "",
                        title: "Místnost neexistuje",
                        detail: $"Místnost s ID {id} neexistuje",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                }

                List<Reservation>? reservations = await _ctx.Reservation
                    .Include(e => e.Room)
                    .AsNoTracking()
                    .Where(e => e.RoomId == id)
                    .ToListAsync();

                if (reservations.Count == 0)
                {
                    return Problem(
                        type: "",
                        title: "Nenašly se rezervace",
                        detail: $"Místnost s id {id} nemá žádnou rezervaci.",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                }

                List<ReservationResponse> response = reservations
                    .Select(e => new ReservationResponse
                    { 
                        Email = e.Email,
                        LastName = e.LastName,
                        ReservationName = e.Name, 
                        DateReservation = e.DateReservation.ToString("dd.MM.yyyy"),
                        RoomCapacity = e.NumberOfPeople,
                        RoomName = e.Name,
                        Description = e.Description,
                        LocationId = e.Room!.LocationId,
                        TimeFrom = e.TimeFrom.ToString("HH:mm"),
                        TimeTo = e.TimeTo.ToString("HH:mm"),
                        NumberOfPeople = e.NumberOfPeople,
                        ReservationId = e.ReservationId,
                        ReservationIsActive = e.IsActive,
                        RoomIsActive = e.Room.IsActive,
                        RoomId = e.Room.RoomId
                    }).ToList();

                _logger.LogInformation("Rezervace pro místnost s ID {id} byly úspěšně načteny", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání rezervací pro místnost s ID {id}", id);
                return Problem(
                    type: "",
                    title: $"Chyba při získávání rezervací pro místnost {id}",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path
                );
            }
        }
    }
}
