using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EvidenceRezervaceMistnosti.Controllers
{
    [Route("localization")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LocalizationController : Controller
    {
        private static readonly HashSet<string> SupportedCultures =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "cs-CZ",
                "en-US",
                "de-DE"
            };

        [HttpPost("set-language")]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string? returnUrl)
        {
            if (!SupportedCultures.Contains(culture))
            {
                culture = "cs-CZ";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                });

            var safeReturnUrl = returnUrl is not null && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : "/dashboard/room";

            return LocalRedirect(safeReturnUrl);
        }
    }
}
