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
using System;
using Serilog;

namespace CAcore.Controllers
{
    [Route("auth")]
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

        [HttpPost("login")]
        public ActionResult <bool> Login(UserCredentialsDto credDto) 
        {
            if (!ModelState.IsValid || credDto == null)
            {
                Log.Information("User login - Invalid model");
                return new BadRequestObjectResult(new { Message = "Login failed", IsLogin = false});
            }

            var dbUser = _repository.GetUserByUserId(credDto.UserId);

            if (dbUser == null)
            {
                Log.Information(string.Format("User login - Invalid username: {0}", credDto.UserId));
                return Unauthorized(new { Message = "Invalid username or password", IsLogin = false });	            
            }

            var hashedPasword = _userHelper.GetHashedPassword(credDto.Password);
            
            if (hashedPasword != dbUser.Password)
            {
                Log.Information(string.Format("User login - Invalid password: {0}", dbUser.UserId));
                return Unauthorized(new { Message = "Invalid username or password", IsLogin = false });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, dbUser.Email),
                new Claim(ClaimTypes.Name, dbUser.UserId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                    new ClaimsPrincipal(claimsIdentity));

            // TODO: remove/change when we have decided our url
            // Source: https://dotnetstories.com/blog/How-to-enable-CORS-for-POST-requests-on-a-single-endpoint-in-ASPNET-Core-en-7186478980?lang=en
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8080");
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" );
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            
            return Ok(new { Message = "Successful login", IsLogin = true });
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