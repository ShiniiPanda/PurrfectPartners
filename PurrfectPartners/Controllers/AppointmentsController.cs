using Microsoft.AspNetCore.Mvc;
using PurrfectPartners.Data;

namespace PurrfectPartners.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;

        public AppointmentsController(ILogger<HomeController> logger, PurrfectPartnersContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
