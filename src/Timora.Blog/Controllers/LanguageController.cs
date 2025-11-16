using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Timora.Blog.Models.ViewModels;

namespace Timora.Blog.Controllers
{
    [Route("[controller]")]
    public class LanguageController : Controller
    {
        [HttpPost("Set")]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(culture))
                culture = "tr-TR";

            // Validate culture
            var supportedCultures = LanguageStrings.GetSupportedCultures();
            if (!supportedCultures.Contains(culture))
                culture = "tr-TR";

            // Set cookie using the same cookie name as CookieRequestCultureProvider
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Path = "/"
                }
            );

            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            return Redirect(returnUrl);
        }
    }
}

