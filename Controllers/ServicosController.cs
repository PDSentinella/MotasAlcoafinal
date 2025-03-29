using Microsoft.AspNetCore.Mvc;

namespace MotasAlcoafinal.Controllers
{
    public class ServicosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
