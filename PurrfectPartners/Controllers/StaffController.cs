using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;
using PurrfectPartners.Models;

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

        public async Task<IActionResult> Appointment(string id)
        {
            var appointmentId = new Guid(id);
            var appointmentModel = await _context.Appointments
                .Where(a => a.Id == appointmentId)
                .Select(a => new StaffAppointmentDetailsModel(a)
                {
                    AnimalName = a.Animal.Name,
                    ServiceName = a.TrainingService!.Name,
                    ServicePrice = a.TrainingService.DefaultPrice, 
                    UserName = a.User.Name,
                    UserEmail = a.User.Email,
                    UserPhone = a.User.PhoneNumber,
                    UserAddress = a.User.Address
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (appointmentModel == null)
            {
                TempData["StatusMessage"] = $"Error: Unable to find appointment with id ({id})";
                return RedirectToAction("Appointments");
            }
            return View(appointmentModel);
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
            var services = await _context.TrainingServices.AsNoTracking().ToListAsync();
            ManageServicesModel ViewModel = new();
            ViewModel.Services = services;
            var animals = await _context.Animals.Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() })
                .AsNoTracking()
                .ToListAsync();
            ViewBag.Animals = animals;
            return View(ViewModel);
        }

        public IActionResult Newsletter()
        {
            return View();
        }

        public async Task<ActionResult> EditService(int id)
        {
            var service = await _context.TrainingServices.Where(t => t.Id == id)
                .Include(t => t.JoinedAnimals)
                .ThenInclude(ja => ja.Animal)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (service == null)
            {
                return RedirectToAction("ManageServices");
            }
            return View(service);
        }

        [HttpPost]
        public async Task<ActionResult> ModifyService(TrainingService model)
        {
            _context.Entry(model).State = EntityState.Modified;
            foreach (var joinedAnimal in model.JoinedAnimals)
            {
                _context.AnimalServices.Entry(joinedAnimal).Property(ja => ja.StartingPrice).IsModified = true;
            }
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = $"Successfully updated the {model.Name} service!";
            return RedirectToAction("EditService", new { id = model.Id });
        }

        public async Task<ActionResult> DeleteService(int id)
        {
            var service = await _context.TrainingServices.FindAsync(id);
            if (service == null)
            {
                TempData["StatusMessage"] = "Error: Failed to delete server. Service does not exist";
                return RedirectToAction("ManageServices");
            }
            _context.TrainingServices.Remove(service);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = $"Service {service.Name} has been removed!";
            return RedirectToAction("ManageServices");
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

        public async Task<ActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["StatusMessage"] = "Error: Unable to delete animal record";
                return RedirectToAction("ManageAnimals");
            }
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "Successfully removed animal record!";
            return RedirectToAction("ManageAnimals");
        }

        [HttpPost]
        public async Task<ActionResult> AddService(SingleServiceModel newService)
        {
            if (ModelState.IsValid)
            {
                var service = new TrainingService
                {
                    Name = newService.Name,
                    DefaultPrice = newService.DefaultPrice,
                    Description = newService.Description
                };
                foreach(var animalId in newService.AnimalIds)
                {
                    service.JoinedAnimals.Add(new()
                    {
                        TrainingServiceId = service.Id,
                        AnimalId = int.Parse(animalId),
                        StartingPrice = service.DefaultPrice
                    });
                }
                _context.Add(service);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Successfully added new service!";
                return RedirectToAction("ManageServices");
            }
            TempData["StatusMessage"] = "Error: Unable to add new service.";
            return RedirectToAction("ManageServices");
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

        public async Task<ActionResult> ChangeAppointmentStatus(string id, int status, string returnAction)
        {
            var appointmentId = new Guid(id);
            var appointment = await _context.Appointments.Where(a => a.Id == appointmentId).FirstOrDefaultAsync();
            if (appointment == null) return RedirectToAction("Appointments");
            appointment.Status = (AppointmentStatus)status;
            _context.Entry(appointment).Property(a => a.Status).IsModified = true;
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "Successfully approved appointment!";
            if (returnAction == null) returnAction = "Appointments";
            if (returnAction.Equals("Appointment")) return RedirectToAction(returnAction, new { id });
            return RedirectToAction(returnAction);
        }
    }
}
