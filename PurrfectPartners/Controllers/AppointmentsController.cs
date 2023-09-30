using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;

namespace PurrfectPartners.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;
        private readonly UserManager<User> _userManager;

        public AppointmentsController(ILogger<HomeController> logger, PurrfectPartnersContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async  Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound($"Unable to load user information!");
            }
            var completedAppointments = await _context.Appointments
                .Where(a => a.UserId == userId && a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.ReservationDate)
                .ThenBy(a => a.Status)
                .AsNoTracking()
                .ToListAsync();

            return View(completedAppointments);
        }

        public async Task<IActionResult> Create()
        {
            var services = await GetServices();
            var listedServices = new List<SelectListItem>();
            foreach (var service in services)
            {
                listedServices.Add(new SelectListItem(service.Name, service.Id.ToString()));
            }
            ViewBag.Services = listedServices;
            return View();
        }

        private async Task<IEnumerable<TrainingService>> GetServices()
        {
            return await _context.TrainingServices.AsNoTracking().ToListAsync();
        }

    }
}
