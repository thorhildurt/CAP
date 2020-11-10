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
        public ActionResult <bool> CheckCredentials(UserCredentialsDto credDto) 
        {
            var user = _repository.GetUserByUserId(credDto.UserId);

            // ALLOWS ALL ORIGINS! TODO: remove/change when we have decided our url
            // Source: https://dotnetstories.com/blog/How-to-enable-CORS-for-POST-requests-on-a-single-endpoint-in-ASPNET-Core-en-7186478980?lang=en
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" );
            Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

            if (user != null && user.Password == credDto.Password) 
            {
                return Ok(new {status = true});
            } 
            else 
            {
                return Ok(new {status = false});
            }
        }
    }
}