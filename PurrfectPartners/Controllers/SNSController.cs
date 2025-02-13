﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PurrfectPartners.Areas.Identity.Data;
using PurrfectPartners.Data;
using PurrfectPartners.Models;
using System.Net;
using System.Text.Json;

namespace PurrfectPartners.Controllers
{
    public class SNSController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly PurrfectPartnersContext _context;
        private readonly UserManager<User> _userManager;
        private static readonly string EndpointName = "newsletter";

        public SNSController(ILogger<HomeController> logger, PurrfectPartnersContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> PublishMessage(SNSRequestModel request)
        {
            if (request.Action != "message") {
                TempData["StatusMessage"] = "Error: Unable to publish message to newsletter!";
                return RedirectToAction("Newsletter", "Staff");
            }
            if (request.Subject == null || request.Message == null) {
                TempData["StatusMessage"] = "Error: Subject and Message can't be empty!";
                return RedirectToAction("Newsletter", "Staff");
            }
            request.AWSKeys = GetAWSConnectionStrings();
            var json = JsonSerializer.Serialize(request);
            var client = new HttpClient();
            var endpoint = _configuration.GetValue<string>("AWS:API") + EndpointName;
            var httpResponse = await client.PostAsync(endpoint, new StringContent(json));
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                var unescapedString = JsonSerializer.Deserialize<string>(responseString);
                var snsResponse = JsonSerializer.Deserialize<SNSResponseModel>(unescapedString!);
                if (snsResponse != null && snsResponse.Status == 200)
                {
                    TempData["StatusMessage"] = $"Successfully broadcasted the message with title {request.Subject}";
                    return RedirectToAction("Newsletter", "Staff");
                }
                TempData["StatusMessage"] = $"Error: Failed to broadcast message. SNS Error";
            } else
            {
                TempData["StatusMessage"] = $"Error: Failed to send request to API Gateway. Status Code: {httpResponse.StatusCode.ToString()}";
            }
            return RedirectToAction("Newsletter", "Staff");
        }

        [Authorize]
        public async Task<IActionResult> Newsletter()
        {
            var email = _userManager.GetUserName(User);
            if (email != null)
            {
                var subscriptionStatusRequest = new SNSRequestModel()
                {
                    AWSKeys = GetAWSConnectionStrings(),
                    Action = "status",
                    Email = email
                };
                var client = new HttpClient();
                var endpoint = _configuration.GetValue<string>("AWS:API") + EndpointName;
                var json = JsonSerializer.Serialize(subscriptionStatusRequest);
                var httpResponse = await client.PostAsync(endpoint, new StringContent(json));
                if (httpResponse.IsSuccessStatusCode == true)
                {
                    var responseString = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError(responseString);
                    var unescapedString = JsonSerializer.Deserialize<string>(responseString);
                    var snsResponse = JsonSerializer.Deserialize<SNSResponseModel>(unescapedString!);
                    if (snsResponse!.Status == 200)
                    {
                        ViewBag.NewsletterStatus = "Active";
                    }
                    else if (snsResponse!.Status == 204)
                    {
                        ViewBag.NewsletterStatus = "Inactive";
                    }
                    else
                    {
                        ViewBag.NewsletterStatus = "Unable to Fetch";
                    }
                }
                else
                {
                    ViewBag.NewsletterStatus = "Unable to Fetch";
                }
            }
            return View("~/Views/Profile/Newsletter.cshtml");
        }

        [Authorize]
        public async Task<IActionResult> Subscribe()
        {
            var email = _userManager.GetUserName(User);
            if (email != null)
            {
                var subscriptionStatusRequest = new SNSRequestModel()
                {
                    AWSKeys = GetAWSConnectionStrings(),
                    Action = "subscribe",
                    Email = email
                };
                var client = new HttpClient();
                var endpoint = _configuration.GetValue<string>("AWS:API") + EndpointName;
                var json = JsonSerializer.Serialize(subscriptionStatusRequest);
                var httpResponse = await client.PostAsync(endpoint, new StringContent(json));
                if (httpResponse.IsSuccessStatusCode == true)
                {
                    var responseString = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError(responseString);
                    var unescapedString = JsonSerializer.Deserialize<string>(responseString);
                    var snsResponse = JsonSerializer.Deserialize<SNSResponseModel>(unescapedString!);
                    if (snsResponse!.Status == 200)
                    {
                        TempData["StatusMessage"] = "Success, please check your email addres to confirm subscription!";
                    }
                    else
                    {
                        TempData["StatusMessage"] = "Error: Failed to subscribe to newsletter!";
                    }
                }
                else
                {
                    TempData["StatusMessage"] = "Error: Failed to subscribe to newsletter!";
                }
            }
            return RedirectToAction("Newsletter", "SNS");
        }

        [Authorize]
        public async Task<IActionResult> Unsubscribe()
        {
            var email = _userManager.GetUserName(User);
            if (email != null)
            {
                var subscriptionStatusRequest = new SNSRequestModel()
                {
                    AWSKeys = GetAWSConnectionStrings(),
                    Action = "unsubscribe",
                    Email = email
                };
                var client = new HttpClient();
                var endpoint = _configuration.GetValue<string>("AWS:API") + EndpointName;
                var json = JsonSerializer.Serialize(subscriptionStatusRequest);
                var httpResponse = await client.PostAsync(endpoint, new StringContent(json));
                if (httpResponse.IsSuccessStatusCode == true)
                {
                    var responseString = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError(responseString);
                    var unescapedString = JsonSerializer.Deserialize<string>(responseString);
                    var snsResponse = JsonSerializer.Deserialize<SNSResponseModel>(unescapedString!);
                    if (snsResponse!.Status == 200)
                    {
                        TempData["StatusMessage"] = "Success, you've been unsubscribed from the Purrfect Parnter's newsletter!";
                    }
                    else if (snsResponse!.Status == 204)
                    {
                        TempData["StatusMessage"] = "Error: Unable to unsubscribe as subscription record was not found!";
                    }
                    else
                    {
                        TempData["StatusMessage"] = "Error: Subscription Status is pending confirmation, please check your email address to either confirm or cancel subscription!";
                    }
                }
                else
                {
                    TempData["StatusMessage"] = "Error: Failed to reach subscription server.";
                }
            }
            return RedirectToAction("Newsletter", "SNS");
        }

        private List<string> GetAWSConnectionStrings()
        {
            var result = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                result.Add(_configuration.GetValue<string>($"AWS:Key{i}")!);
            }
            return result;
        }

    }
}
