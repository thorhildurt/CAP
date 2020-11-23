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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Serilog;

namespace CAcore.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICAcoreRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        public UserController(ICAcoreRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _userHelper= new UserHelper();
        }

        [HttpGet("{uid}", Name="GetLoggedInUser")]
        public ActionResult <UserReadDto> GetLoggedInUser(String uid)
        {
            // Get the id of the logged in user. The id is located in claim identity in the authenticatiton cookie
            //ClaimsPrincipal currentUser = this.User;
            //var userId = currentUser.FindFirst(ClaimTypes.Name).Value;

            var user = _repository.GetUserByUserId(uid);
            if(user != null)
            {
                // TODO: remove/change when we have decided our url
                Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:3001/");
                return Ok(_mapper.Map<UserReadDto>(user));
            }
            return NotFound();
        }

        [HttpPut(Name="UpdateLoggedInUser")]
        public ActionResult <UserReadDto> UpdateLoggedInUser(UserUpdateDto userUpdateDto)
        {
            // Get the id of the logged in user. The id is located in claim identity in the authenticatiton cookie
            ClaimsPrincipal currentUser = this.User;
            var userId = currentUser.FindFirst(ClaimTypes.Name).Value;
            userUpdateDto.UserId = userId;

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
                    Log.Information(string.Format("Update passsword - Invalid old password: {0}", userId));
                    return Unauthorized();
                }
                Log.Information(string.Format("Update passsword - password updated: {0}", userId));
            }
            else
            {
                userUpdateDto.NewPassword = String.Empty;
                userUpdateDto.Password = userModel.Password;
            }

            userModel.FirstName = !String.IsNullOrEmpty(userUpdateDto.FirstName) ? userUpdateDto.FirstName : userModel.FirstName;
            userModel.LastName = !String.IsNullOrEmpty(userUpdateDto.LastName) ? userUpdateDto.LastName: userModel.LastName;
            _repository.UpdateUser(userModel, userUpdateDto.NewPassword);
            _repository.SaveChanges();

            // TODO: remove/change when we have decided our url
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3001");
            return NoContent();
        }
    }
}