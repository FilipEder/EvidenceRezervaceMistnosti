using EvidenceRezervaceMistnosti.DTO.Response;
using EvidenceRezervaceMistnosti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvidenceRezervaceMistnosti.API
{
    [Route("equipment")]
    [ApiController]
    public class EquipmentApiController : ControllerBase
    {
        private readonly ILogger<EquipmentApiController> _logger;
        private readonly ReservationContext _ctx;
        public EquipmentApiController(ILogger<EquipmentApiController> logger, ReservationContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentResponse>>> Get()
        {
            try
            {
                List<EquipmentResponse> response = await _ctx.Equipment
                    .Where(e => e.IsActive)
                    .Select(e => new EquipmentResponse
                    {
                        EquipmentId = e.EquipmentId,
                        Name = e.Name,
                        IsActive = e.IsActive
                    })
                    .ToListAsync();

                if(response.Count == 0)
                {
                    _logger.LogWarning("Nebylo nalezeno žádné vybavení.");
                    return Problem(
                        type: "",
                        title: "Nebylo nalezeno žádné vybavení",
                        detail: "Nebylo nalezeno žádné aktivní vybavení.",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.Request.Path
                    );
                }

                _logger.LogInformation("Získání vybavení proběhlo úspěšně.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání vybavení");
                return Problem(
                    type: "",
                    title: "Chyba při získávání vybavení",
                    detail: "Zkuste to později, pokud by problém nadále trval, kontaktujte administrátora",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path
                );
            }
        }
    }
}
