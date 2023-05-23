using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Repository;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost(Name = "AuthenticateUser")]
        public async Task<ActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            if (request == null)
                return BadRequest();

            if (_userRepository.IsValidUser(request.UserName, request.Password))
            {
                var response = new AuthenticateUserResult { Success = true, ErrorMessage = string.Empty };

                return Ok(response);
            }

            return Unauthorized(new AuthenticateUserResult { Success = false, ErrorMessage = "Invalid User!" });
        }
    }
}