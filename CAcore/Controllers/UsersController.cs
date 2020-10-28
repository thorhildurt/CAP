using System.Collections.Generic;
using CAcore.Models;
using CAcore.Data;
using Microsoft.AspNetCore.Mvc;

namespace CAcore.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ICAcoreRepo _repository;

        public UsersController(ICAcoreRepo repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult <IEnumerable<User>> GetAllUsers()
        {
            var users = _repository.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult <User> GetUserById(int id)
        {
            var user = _repository.GetUserById(id);
            return Ok(user);
        }
    }
}