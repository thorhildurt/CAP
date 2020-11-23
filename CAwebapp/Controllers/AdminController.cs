using CAwebapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MvcMovie.Controllers
{
    public class AdminController : Controller
    {
        
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        private readonly String apiBaseUrl;
        private HttpClient _httpClient;

        public AdminController(ILogger<AdminController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            apiBaseUrl = _configuration["WebAPIBaseUrl"];

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.UseCookies = true;
            clientHandler.CookieContainer = new CookieContainer();
                
            _httpClient = new HttpClient(clientHandler);
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            
            MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }



        // GET: /Admin/

        [HttpGet]
        public IActionResult Index()
        {
             return RedirectToAction("Profile"); 
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        [HttpGet]  
        public async Task<IActionResult> Profile()  
        {
            string endpoint = "/admin";
            using (var Response = await _httpClient.GetAsync(endpoint))  
            {  
                // if (Response.StatusCode == System.Net.HttpStatusCode.OK)  

                // {   
                    var responseBody = Response.Content.ReadAsStringAsync().Result;
                    ViewBag.Status = JsonConvert.DeserializeObject<CertificatesStatus>(responseBody); 
                     
                // }  
                // else  
                // {  
                //     ModelState.Clear();  
                //     ModelState.AddModelError(string.Empty, "User is not authenticated");  
                //     return RedirectToAction("Login");
                // }  
                return View();
            } 
        }
    }
}