using EvidenceRezervaceMistnosti.DTO;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<ActionResult<List<Room>>> Get()
        {
            try
            {
                List<Room> rooms = await _ctx.Room.Include(r => r.Reservations).ToListAsync();

                if (rooms.Count == 0)
                {
                    _logger.LogWarning("No rooms found");
                    return NotFound(new
                    {
                        message = "Žádné místnosti nebyly nalezeny"
                    });
                };

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: fetching rooms");
                return StatusCode(503);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetById(int id)
        {
            try
            {
                Room? room = await _ctx.Room.FindAsync(id);

                if (room == null)
                {
                    _logger.LogWarning("No room found");
                    return NotFound(new
                    {
                        message = "Daná místnost se nenašla"
                    });
                };

                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: fetching room");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Post(RoomDTORequest request)
        {
            try
            {
                Room room = new()
                {
                    Name = request.Name.Trim(),
                    Capacity = request.Capacity,
                    LocationId = request.LocationId,
                    Gear = request.Gear.Trim()
                };

                _ctx.Room.Add(room);
                await _ctx.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = room.RoomId }, room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: creating room");
                return StatusCode(500);
            }
        }

        [HttpGet("{id:int}/reservations")]
        public async Task<ActionResult<List<Reservation>>> GetReservations(int id)
        {
            try
            {
                if(!await RoomExistsAsync(id))
                {
                    return NotFound(new
                    {
                        message = $"Místnost {id} neexistuje."
                    });
                }
                List<Reservation> res = await _ctx.Reservation.Where(e => e.RoomId == id).ToListAsync();

                if (res.Count == 0)
                {
                    return NotFound(new
                    {
                        message = $"Místnost nemá žádnou rezervaci"
                    });
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: getting reservations");
                return StatusCode(500);
            }
        }

        public bool RoomExists(int id) => _ctx.Room.Any(e => e.RoomId == id);
        public Task<bool> RoomExistsAsync(int id) => _ctx.Room.AnyAsync(e => e.RoomId == id);
    }
}
