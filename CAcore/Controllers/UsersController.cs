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

namespace CAcore.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICAcoreRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserHelper _userHelper;

        public UsersController(ICAcoreRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _userHelper= new UserHelper();
        }

        [HttpGet]
        public ActionResult <IEnumerable<UserReadDto>> GetAllUsers()
        {
            var users = _repository.GetAllUsers();

            // ALLOWS ALL ORIGINS! TODO: remove/change when we have decided our url
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
        }

        [HttpGet("{userId}", Name="GetUserByUserId")]
        public ActionResult <UserReadDto> GetUserByUserId(string userId)
        {
            var user = _repository.GetUserByUserId(userId);
            if(user != null)
            {
                // ALLOWS ALL ORIGINS! TODO: remove/change when we have decided our url
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return Ok(_mapper.Map<UserReadDto>(user));
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult <UserReadDto> CreateUser(UserCreateDto userCreateDto)
        {
            var userModel = _mapper.Map<User>(userCreateDto);
            _repository.CreateUser(userModel);
            
            if (_repository.SaveChanges())
            {
                var userReadDto = _mapper.Map<UserReadDto>(userModel);
                return CreatedAtRoute(nameof(GetUserByUserId), new { UserID = userReadDto.UserId }, userReadDto);
            }
            return BadRequest();
        }

        [HttpPut("{userId}")]
        public ActionResult <UserReadDto> UpdateUser(string userId, UserUpdateDto userUpdateDto)
        {
            Console.WriteLine("bla");
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

            // ALLOWS ALL ORIGINS! TODO: remove/change when we have decided our url
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(string userId)
        {
            var userModel = _repository.GetUserByUserId(userId);
            if (userModel == null)
            {
                return NotFound();
            }
            _repository.DeleteUser(userModel);
            _repository.SaveChanges();
            return NoContent();
        }
    }
}