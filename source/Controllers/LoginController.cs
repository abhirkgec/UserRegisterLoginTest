using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Auth;
using UserApi.Helpers;
using UserApi.Model;
using UserApi.Service;
using UserApi.Services;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        public LoginController(ICosmosDbService cosmosDbService, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _cosmosDbService = cosmosDbService;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }
        [Route("Register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (ModelState.IsValid) // As of now I have done basic validation but it can be enhanced
                {
                    var existingUser = await _cosmosDbService.GetItemByUserNameAsync(user.UserName);
                    if (existingUser==null)
                    {
                        user.id = Guid.NewGuid().ToString();
                        await _cosmosDbService.AddItemAsync(user);
                        //sending welcome email{using userName as email}
                        //await Utility.SendEmail(_appSettings.SendGridKey, _appSettings.FromEmail, _appSettings.FromName, user.UserName, user.Fullname, _appSettings.WelComeEmailSubject, _appSettings.WelcomeEmailBody);

                        // removing Static and adding abstraction dependency for Email utility
                        await _emailService.SendEmail(user.UserName, user.Fullname);
                        return StatusCode(StatusCodes.Status201Created, new ObjectResult(new { Value = "User Created successfully!", StatusCode = StatusCodes.Status201Created }));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ObjectResult(new { Value = "Resource already exists!", StatusCode = StatusCodes.Status409Conflict }));
                    }
                    
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ObjectResult(new { Value = "Bad request!", StatusCode = StatusCodes.Status400BadRequest }));
                }
                
            }
            catch (Microsoft.Azure.Cosmos.CosmosException ex) 
            {
                return null; //TODO:: implement logger
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ObjectResult(new { Value = "Internal error!", StatusCode = StatusCodes.Status500InternalServerError }));

            }
        }

        [Route("Login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            AuthenticateResponse response = new AuthenticateResponse();
            if (!string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
            {
                var result = await _cosmosDbService.Authenticate(loginModel.UserName, loginModel.Password);
                if (result != null)
                {
                    response.JwtToken = _jwtUtils.GenerateToken(result);
                    response.AuthSuccess = true;
                    response.FullName = result.Fullname;
                    response.Username = result.UserName;
                    return Ok(response);
                }
                else
                {
                    response.AuthSuccess = false;
                    return StatusCode(StatusCodes.Status401Unauthorized, new ObjectResult(new { Value = "Credentials are not correct!", StatusCode = StatusCodes.Status401Unauthorized }));
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ObjectResult(new { Value = "bad request!", StatusCode = StatusCodes.Status400BadRequest }));
            }
            
        }

        
        [Route("Dashboard")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext != null)
            {
                var user = (User)HttpContext.Items["User"];
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ObjectResult(new { Value = "Unauthorised request!", StatusCode = StatusCodes.Status401Unauthorized }));
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ObjectResult(new { Value = "Unauthorised request!", StatusCode = StatusCodes.Status401Unauthorized }));
            }
            
            
            

        }
    }
}
