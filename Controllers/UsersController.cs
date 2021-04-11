using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using globelinkapi.Helpers;
using globelinkapi.Models;
using globelinkapi.services;
using Microsoft.AspNetCore.Mvc;
using globelinkapi.Entities;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace globelinkapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("insUser")]
        public IActionResult insUser(UsersRequest model)
        {
            var user = HttpContext.Items["User"];
            if (user == null)
            {
                return BadRequest(new { message = "unauthorize insert data" });
            }
            //model.createdBy = ((User)user).Id.ToString();
            //String updatedBy = ((User)user).Id.ToString();
            var response = _userService.insUser(model);
            return Ok(response);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var user = HttpContext.Items["User"];
            if (user == null)
            {
                return BadRequest(new { message = "unauthorize retrive data" });
            }
            var users = _userService.GetAll();
            return Ok(users);
        }
        [HttpPost("updUser")]
        public IActionResult updUser(UsersRequest model)
        {
            var user = HttpContext.Items["User"];
            if (user == null)
            {
                return BadRequest(new { message = "unauthorize insert data" });
            }
            //model.createdBy = ((User)user).Id.ToString();
            //String updatedBy = ((User)user).Id.ToString();
            var response = _userService.updUser(model);
            return Ok(response);
        }
    }
}