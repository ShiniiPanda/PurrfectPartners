using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;
using System.ComponentModel.DataAnnotations;

namespace PurrfectPartners.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;
        private readonly UserManager<User> _userManager;

        public AppointmentsController(ILogger<HomeController> logger, PurrfectPartnersContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAppointment(AppointmentSubmissionModel newAppointmentModel, IFormFile AnimalImage)
        {
            if (ModelState.IsValid)
            {
                var connectionStrings = GetAWSConnectionStrings();
                var awsS3Client = new AmazonS3Client(connectionStrings[0], connectionStrings[1], connectionStrings[2], RegionEndpoint.USEast1);
                string imageName = string.Empty;
                bool ImageUploaded = false;
                if (AnimalImage != null)
                {
                    // Exceeds 5 MB
                    if (AnimalImage.Length > 5242880) {
                        return BadRequest("Image cannot exceed 5 MB in size, please apply compression or upload a smaller image.");
                    }
                    var fileExtension = Path.GetExtension(AnimalImage.FileName);
                    imageName = Guid.NewGuid().ToString() + fileExtension;
                    try
                    {
                        PutObjectRequest uploadAnimalImage = new PutObjectRequest
                        {
                            InputStream = AnimalImage.OpenReadStream(),
                            BucketName = StandardValues.S3BucketName,
                            Key = imageName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        await awsS3Client.PutObjectAsync(uploadAnimalImage);
                        ImageUploaded = true;

                    } catch (Exception ex)
                    {
                        _logger.LogError($"Unable to upload image to S3 {ex.Message}");
                        ImageUploaded = false;
                    }
                }
                var userId = _userManager.GetUserId(User);
                if (userId == null) return BadRequest("Unable to load user information");
                Appointment newAppointment = new();
                newAppointment.UserId = userId;
                if (ImageUploaded) newAppointment.AnimalImage = imageName;
                newAppointment.BookingDate = DateTime.UtcNow;
                newAppointment.ReservationDate = newAppointmentModel.ReservationDate;
                newAppointment.TrainingServiceId = int.Parse(newAppointmentModel.ServiceId);
                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Appointment(string id)
        {
            var appointmentId = new Guid(id);
            var appointment = await _context.Appointments.Include(a => a.TrainingService)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
            return View(appointment);
        }

        private List<string> GetAWSConnectionStrings()
        {
            var result = new List<string>();
            for(int i = 1; i <= 3; i++)
            {
                result.Add(_configuration.GetValue<string>($"AWS:Key{i}")!);
            }
            return result;
        }

    }

    public class AppointmentSubmissionModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; }

        [Required]
        public string ServiceId { get; set; } = null!;
    }
}
