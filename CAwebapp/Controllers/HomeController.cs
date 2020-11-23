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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet]  
        public IActionResult Profile()  
        {  
            Console.WriteLine("profile");
            ClaimsPrincipal currentUser = this.User;
            var userId = currentUser.FindFirst(ClaimTypes.Name).Value;
            Console.WriteLine(userId);
            //var user = JsonConvert.DeserializeObject<UserInformation>(Convert.ToString(TempData["Profile"])); 
        
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            string getUserEndpoint = "/user/" + userId;
            Console.WriteLine("Getting users");
            var userResponse = _httpClient.GetAsync(getUserEndpoint).Result;
           

            // if we can fetch user lets try to fetch certificates
            if (userResponse.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                var responseBody = userResponse.Content.ReadAsStringAsync().Result;
                ViewBag.User = JsonConvert.DeserializeObject<UserInformation>(responseBody); 

                Console.WriteLine("Getting user certificates");
                string getUserCertEndpoint = "/user/" + userId + "/certificates";
                var certResponse = _httpClient.GetAsync(getUserCertEndpoint).Result;

                var certResponseBody = certResponse.Content.ReadAsStringAsync().Result;

                var certificates = JsonConvert.DeserializeObject<IEnumerable<UserCertificate>>(certResponseBody); 
                ViewBag.ActiveCerts = certificates.Where(x => !x.Revoked).ToList();
                ViewBag.RevokedCerts = certificates.Where(x => x.Revoked).ToList();

                return View();
            } 
            else
            {
                return RedirectToAction("Login");
            }
        } 

        // Create new certificate
        [Authorize]
        public IActionResult CreateAndDownloadCert()  
        {  
            Console.WriteLine("CreateAndDownloadCert");
            ClaimsPrincipal currentUser = this.User;
            var userId = currentUser.FindFirst(ClaimTypes.Name).Value;
            var user = new UserInformation() {UserId = userId};

            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); 
            string endpoint = "/user/" + userId + "/certificates"; 
            var response = _httpClient.PostAsync(endpoint, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                var certResponseBody = response.Content.ReadAsStringAsync().Result;
                var certificates = JsonConvert.DeserializeObject<CreateCertResponse>(certResponseBody); 
                Console.WriteLine("Certificate created! " + certificates.cid);
                string downloadEndpoint = "/user/" + userId + "/certificates/download/" + certificates.cid;
                var fileResponse = _httpClient.GetAsync(downloadEndpoint).Result;

                var fileContent = fileResponse.Content.ReadAsStringAsync().Result;
                var fileName = String.Format("{0}.pfx", certificates.cid);

                return File(Encoding.UTF8.GetBytes(fileContent), "APPLICATION/binary", fileName);
            }

            TempData["Profile"] = JsonConvert.SerializeObject(user);
            return RedirectToAction("Profile");
        }

        [Authorize]
        public IActionResult RevokeCert(string cid)
        {
            Console.WriteLine("RevokeCert");
            ClaimsPrincipal currentUser = this.User;
            var userId = currentUser.FindFirst(ClaimTypes.Name).Value;
            var user = new UserInformation() {UserId = userId};
            
            StringContent content = new StringContent("", Encoding.UTF8, "application/json"); 
            string revokeEndpoint = "/user/" + userId + "/certificates/" + cid + "/revoke"; 
            var response = _httpClient.PutAsync(revokeEndpoint, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                Console.WriteLine("Revoking certificate successfull");
            }
            TempData["Profile"] = JsonConvert.SerializeObject(user);
            Console.WriteLine(cid);
            return RedirectToAction("Profile");
        }

        // Update user information
        [Authorize]
        [HttpPost]  
        public IActionResult Profile(UserUpdate user)  
        {  
            ClaimsPrincipal currentUser = this.User;
            var userId = currentUser.FindFirst(ClaimTypes.Name).Value;
            user.UserId = userId;
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); 
            string endpoint = "/user";  
            var response = _httpClient.PutAsync(endpoint, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                Console.WriteLine("Updating user successfull");
            }
            TempData["Profile"] = JsonConvert.SerializeObject(user); 
            return RedirectToAction("Profile");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> Login(string userId, string password)  
        {  
            var user = new {UserId=userId, Password=password};
     
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");  
            string endpoint = "/auth/login";  

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)  
            {  
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userId)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                        new ClaimsPrincipal(claimsIdentity));

                TempData["Profile"] = JsonConvert.SerializeObject(user); 
                return RedirectToAction("Profile");  
            }  
            else  
            {  
                ModelState.Clear();  
                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");  
                return View();  
            }       
        }

        [HttpPost]  
        public async Task<IActionResult> Logout()  
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
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
