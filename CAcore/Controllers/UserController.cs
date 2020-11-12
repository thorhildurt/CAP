using System.Collections.Generic;
using CAcore.Models;
using CAcore.Dtos;
using CAcore.Data;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using System;
using CAcore.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CAcore.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICAcoreRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        private const String claimNameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        public UserController(ICAcoreRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _userHelper= new UserHelper();
        }

        [HttpGet(Name="GetLoggedInUser")]
        public ActionResult <UserReadDto> GetLoggedInUser()
        {
            // Get the id of the logged in user, the id is located in claim identity in the authenticatiton cookie
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimNameType)?.Value;
            var user = _repository.GetUserByUserId(userId);
            if(user != null)
            {
                // TODO: remove/change when we have decided our url
                Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8080/");
                return Ok(_mapper.Map<UserReadDto>(user));
            }
            return NotFound();
        }

        [HttpPut(Name="UpdateLoggedInUser")]
        public ActionResult <UserReadDto> UpdateLoggedInUser(UserUpdateDto userUpdateDto)
        {
            // Get the id of the logged in user, the id is located in claim identity in the authenticatiton cookie
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimNameType)?.Value;
  
            var userModel = _repository.GetUserByUserId(userId);
            if (userModel == null)
            {
                return NotFound();
            }
    
            if (!String.IsNullOrEmpty(userUpdateDto.Password) && !String.IsNullOrEmpty(userUpdateDto.NewPassword)) 
            {   
                string PasswordHash = _userHelper.GetHashedPassword(userUpdateDto.Password);

                // TODO: Maybe refactor once login is implemented
                if (PasswordHash != userModel.Password)
                {
                    return Unauthorized();
                }
            }
            else
            {
                userUpdateDto.NewPassword = String.Empty;
                userUpdateDto.Password = userModel.Password;
            }

            _mapper.Map(userUpdateDto, userModel);
            _repository.UpdateUser(userModel, userUpdateDto.NewPassword);
            _repository.SaveChanges();

            // TODO: remove/change when we have decided our url
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8080");
            return NoContent();
        }
    }
}