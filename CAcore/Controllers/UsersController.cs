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
        private readonly MockCAcoreRepo _reposiotry = new MockCAcoreRepo();

        [HttpGet]
        public ActionResult <IEnumerable<User>> GetAllUsers()
        {
            var users = _reposiotry.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult <User> GetUserById(int id)
        {
            var user = _reposiotry.GetUserById(id);
            return Ok(user);
        }
    }
}