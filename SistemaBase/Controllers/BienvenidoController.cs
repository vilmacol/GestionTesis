using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaBase.Controllers
{
    //[Authorize]
    public class BienvenidoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
