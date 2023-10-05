using Microsoft.AspNetCore.Mvc;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;
using PurrfectPartners.Models;
using System.Diagnostics;

namespace PurrfectPartners.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;

        public HomeController(ILogger<HomeController> logger, PurrfectPartnersContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Animals()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Feedback()
        {
            return View();
        }

        public async Task<IActionResult> AddFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedback.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["FeedbackStatus"] = "Your feedback has been successfully sent to our team!";
                return RedirectToAction("Feedback");
            }
            TempData["FeedbackStatus"] = "Error: Please enter your email address and message!";
            return RedirectToAction("Feedback");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}