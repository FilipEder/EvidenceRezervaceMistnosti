using EvidenceRezervaceMistnosti.DTO.Response;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidenceRezervaceMistnosti.API
{
    public class LocationApiController : ControllerBase
    {
        private readonly ILogger<LocationApiController> _logger;
        private readonly ReservationContext _ctx;
        public LocationApiController(ILogger<LocationApiController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<ActionResult<List<LocationResponse>>> Get()
        {
            try
            {
                List<LocationResponse> response = await _ctx.Location
                    .AsNoTracking()
                    .Select(l => new LocationResponse
                    {
                        LocationId = l.LocationId,
                        Name = l.Name,
                        IsActive = l.IsActive
                    })
                    .ToListAsync();

                if (response.Count == 0)
                {
                    _logger.LogWarning("Nebylo nalezeno žádné umístění.");
                    return Problem(
                        type: "",
                        title: "Nebylo nalezeno žádné umístění",
                        detail: "Nebylo nalezeno žádné aktivní umístění.",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                }

                _logger.LogInformation("Získání umístění proběhlo úspěšně.");
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Chyba při získávání umístění");
                return Problem(
                    type: "",
                    title: "Chyba při získávání umístění",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path
                );
            }
        }
    }
}
