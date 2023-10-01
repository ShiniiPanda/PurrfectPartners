using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;

namespace PurrfectPartners.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;
        private readonly UserManager<User> _userManager;

        public StaffController(ILogger<HomeController> logger, PurrfectPartnersContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return View(appointments);
        }

        public async Task<IActionResult> ManageCustomers()
        {
            var users = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Customer));
            return View(users.ToList());
        }

        public async Task<IActionResult> ManageStaff()
        {
            var users = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Staff));
            return View(users.ToList());
        }

        public async Task<IActionResult> ManageServices()
        {
            var services = await _context.TrainingServices.Include(t => t.Animals).ToListAsync();
            return View(services);
        }

        public async Task<IActionResult> ManageAnimals()
        {
            var animals = await _context.Animals.AsNoTracking().ToListAsync();
            return View(animals);
        }

        [HttpPost]
        public async Task<ActionResult> AddAnimal(string name)
        {
            if (name != null && name.Length > 1)
            {
                name = name.ToLower();
                string processedName = $"{char.ToUpper(name[0])}{name[1..]}";
                var newAnimal = new Animal { Name = processedName };
                _context.Animals.Add(newAnimal);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Added ({processedName}) to animals list!";
                return RedirectToAction("ManageAnimals");
            }
            TempData["StatusMessage"] = $"Error: New Animal Name must be atleast 1 character long!";
            return RedirectToAction("ManageAnimals");
        }

        public async Task<ActionResult> MakeAdmin(string email)
        {
            var user = await _userManager.Users.Where(u => u.UserName == email).FirstOrDefaultAsync();
            if (user == null)
            {
                TempData["StatusMessage"] = "Error: Unable to find user with this email!";
                return RedirectToAction("ManageStaff");
            }
            var isCustomer = await _userManager.IsInRoleAsync(user, nameof(UserRole.Customer));
            var isStaff = await _userManager.IsInRoleAsync(user, nameof(UserRole.Staff));
            if (isStaff)
            {
                TempData["StatusMessage"] = "Error: This user is already a staff member!";
                return RedirectToAction("ManageStaff");
            }
            else
            {
                if (isCustomer)
                {
                    await _userManager.RemoveFromRoleAsync(user, nameof(UserRole.Customer));
                    await _userManager.AddToRoleAsync(user, nameof(UserRole.Staff));
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, nameof(UserRole.Staff));
                }
                ViewBag.StatusMessage = $"User ({email}) has become a staff member!";
                return RedirectToAction("ManageStaff");
            }
        }
    }
}
