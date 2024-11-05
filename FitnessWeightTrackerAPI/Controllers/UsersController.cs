﻿using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Services;

namespace FitnessWeightTrackerAPI.Controllers
{
    //[ValidateModel]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService service)
        {
            _userService = service;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUsers(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> LoginUsers(UserLoginDTO login)
        {
            User user = await _userService.LoginUserAsync(login);
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            var jwtToken = _userService.GenerateUserJWTToken(user);

            return CreatedAtAction(nameof(LoginUsers), new Dictionary<string, string> { { "jwtBearer", jwtToken } });
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUsers(RegistrationUserDTO user)
        {
            var registeredUser = await _userService.RegisterUserAsync(user);

            if (registeredUser == null)
            {
                return BadRequest("User with this email/username already exists");
            }
            return CreatedAtAction(nameof(GetUsers), new { id = registeredUser.Id });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var isDeleted = await _userService.DeleteUserAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
