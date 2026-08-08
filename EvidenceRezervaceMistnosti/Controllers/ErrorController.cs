using EvidenceRezervaceMistnosti.Models.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EvidenceRezervaceMistnosti.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {
        private static readonly string[] ApiRoutePrefixes =
        [
            "/reservations",
            "/rooms",
            "/locations",
            "/equipment"
        ];

        private readonly ILogger<ErrorController> _logger;
        private readonly IStringLocalizer<Trans> _localizer;

        public ErrorController(
            ILogger<ErrorController> logger,
            IStringLocalizer<Trans> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [Route("error")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            string originalPath = exceptionFeature?.Path ?? Request.Path;

            _logger.LogError(
                exceptionFeature?.Error,
                "Neočekávaná chyba při zpracování požadavku {path}",
                originalPath);

            if (IsApiRequest(originalPath))
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: _localizer["An unexpected error occurred"].Value,
                    detail: _localizer["The request could not be processed. Please try again later."].Value,
                    instance: originalPath
                );
            }

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View("CstmError", new CstmErrorViewModel
            {
                Title = "An unexpected error occurred",
                Description = "The request could not be processed. Please try again later."
            });
        }

        [Route("error/{statusCode:int}")]
        public IActionResult StatusCodeError(int statusCode)
        {
            IStatusCodeReExecuteFeature? statusFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            string originalPath = statusFeature?.OriginalPath ?? Request.Path;
            (string title, string description) = GetStatusMessage(statusCode);

            _logger.LogWarning(
                "Požadavek na {path} skončil stavovým kódem {statusCode}",
                originalPath,
                statusCode);

            if (IsApiRequest(originalPath))
            {
                return Problem(
                    statusCode: statusCode,
                    title: _localizer[title].Value,
                    detail: _localizer[description].Value,
                    instance: originalPath
                );
            }

            Response.StatusCode = statusCode;
            return View("CstmError", new CstmErrorViewModel
            {
                Title = title,
                Description = description
            });
        }

        private bool IsApiRequest(string path)
        {
            return ApiRoutePrefixes.Any(prefix =>
                       path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) ||
                   Request.Headers.Accept
                       .ToString()
                       .Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        private static (string Title, string Description) GetStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => (
                    "Bad request",
                    "Check the entered information and submit the request again."),
                StatusCodes.Status403Forbidden => (
                    "Access denied",
                    "You do not have permission to perform this action."),
                StatusCodes.Status404NotFound => (
                    "Page not found",
                    "Check the address or return to the reservation overview."),
                _ => (
                    "The request could not be processed",
                    "Try the request again or return to the home page.")
            };
        }
    }
}
