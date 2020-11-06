using System.Collections.Generic;
using CAcore.Models;
using CAcore.Dtos;
using CAcore.Data;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace CAcore.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ICAcoreRepo _repository;
        private readonly IMapper _mapper;

        public LoginController(ICAcoreRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [HttpPost]
        public ActionResult <bool> CheckCredentials(UserCredentialsDto credDto) {
            var user = _repository.GetUserByEmail(credDto.Email);
            if (user != null && user.Password == credDto.Password) {
                return Ok(true);
            } else return Ok(false);
        }
    }
}