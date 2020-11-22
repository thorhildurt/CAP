using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CAwebapp.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net;

namespace CAwebapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly String apiBaseUrl;
        private HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
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

        // This is the starting point, if the user is not authentticated it is redirected to the login page
        [HttpGet]  
        public async Task<IActionResult> Index()
        {
            string endpoint = "/user";
            using (var Response = await _httpClient.GetAsync(endpoint))  
            {  
                if (Response.StatusCode == System.Net.HttpStatusCode.OK)  
                {  
                    TempData["Profile"] = JsonConvert.SerializeObject(Response.Content);  
                    return RedirectToAction("Profile");  
                }  
                else  
                {  
                    ModelState.Clear();  
                    ModelState.AddModelError(string.Empty, "User is not authenticated");  
                    return RedirectToAction("Login");
                }  
            }  
        }

        [HttpGet]  
        public async Task<IActionResult> Profile()  
        {  
            Console.WriteLine("profile");
            UserInformation user = null;
            var authCookie = HttpContext.Request.Cookies[".AspNetCore.Cookies"];
            
            string endpoint = "/user";
            using (var Response = await _httpClient.GetAsync(endpoint))  
            {  
                if (Response.StatusCode == System.Net.HttpStatusCode.OK)  
                {  
                    user = JsonConvert.DeserializeObject<UserInformation>(Response.Content.ToString());  
                    return View(user);
                } 
            }
            
            Console.WriteLine("get user did not work :(");
            //user = JsonConvert.DeserializeObject<UserInformation>(Convert.ToString(TempData["Profile"]));  
            return View(user);  
        } 

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  
        public IActionResult Login(string userId, string password)  
        {  
            var user = new {UserId=userId, Password=password};
     
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");  
            string endpoint = "/auth/login";  

            var response = _httpClient.PostAsync(endpoint, content).Result;
            
            var res = response.Content.ReadAsStringAsync();

            Console.WriteLine(response.Headers);
            var test = HttpContext.Request.Headers;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                TempData["Profile"] = JsonConvert.SerializeObject(user); 
                var getUser = _httpClient.GetAsync(endpoint).Result;
                Console.WriteLine(getUser.Headers);
                return RedirectToAction("Profile");  
            }  
            else  
            {  
                ModelState.Clear();  
                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");  
                return View();  
            }       
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
