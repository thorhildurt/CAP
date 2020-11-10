using System.Collections.Generic;
using CAcore.Models;
using CAcore.Dtos;
using CAcore.Data;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using CAcore.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CAcore.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICAcoreRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        public LoginController(ICAcoreRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _userHelper = new UserHelper();
        }

        [HttpPost]
        public ActionResult <bool> Login(UserCredentialsDto credDto) 
        {
            if (!ModelState.IsValid || credDto == null)
            {
                return new BadRequestObjectResult(new { Message = "Login failed" });
            }

            var dbUser = _repository.GetUserByUserId(credDto.UserId);

            if (dbUser == null)
            {
                return Unauthorized();	            
            }

            var hashedPasword = _userHelper.GetHashedPassword(credDto.Password);
            
            if (hashedPasword != dbUser.Password)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, dbUser.Email),
                new Claim(ClaimTypes.Name, dbUser.UserId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                    new ClaimsPrincipal(claimsIdentity));

            HttpContext.Response.Cookies.Append("Userid", dbUser.UserId);

            // ALLOWS ALL ORIGINS! TODO: remove/change when we have decided our url
            // Source: https://dotnetstories.com/blog/How-to-enable-CORS-for-POST-requests-on-a-single-endpoint-in-ASPNET-Core-en-7186478980?lang=en
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8080");
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" );
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            
            return Ok(new { status = true });
        }

        [HttpPost]
        [Route("logout")]
        public ActionResult <bool> Logout() 
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);	            
            return Ok();
        }
    }
}