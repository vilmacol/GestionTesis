using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Models;
using System.Security.Claims;

namespace SistemaBase.Filters
{
    public class AccesoPantallaFilter : IAsyncAuthorizationFilter
    {
        private readonly UAADbContext _context;
        private readonly string _nomForma;

        public AccesoPantallaFilter(UAADbContext context, string nomForma)
        {
            _context = context;
            _nomForma = nomForma;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            var grupo = user.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(grupo))
            {
                context.Result = new RedirectToActionResult("ErrorAcceso", "Login", null);
                return;
            }

            if (grupo.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var tieneAcceso = await _context.GruposUsuarios
                .AsNoTracking()
                .Where(g => g.IdGrupo == grupo)
                .SelectMany(g => g.Formas)
                .AnyAsync(f => f.NomForma == _nomForma);

            if (!tieneAcceso)
            {
                context.Result = new RedirectToActionResult("ErrorAcceso", "Login", null);
            }
        }
    }
}
