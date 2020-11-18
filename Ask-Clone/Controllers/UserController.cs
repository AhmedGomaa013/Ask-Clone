using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ask_Clone.Jwt.Services;
using Ask_Clone.Models;
using Ask_Clone.Models.Entities;
using Ask_Clone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ask_Clone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public partial class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IQuestionsRepository _questionsRepository;
        private readonly IUserRepository _userRepository;
        private readonly JWTCreator _jwtCreator;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IQuestionsRepository questionsRepository,
            IUserRepository userRepository,
            JWTCreator jwtCreator,
            ILogger<UserController> logger
            )
        {
            _userManager = userManager;
            _questionsRepository = questionsRepository;
            _userRepository = userRepository;
            _jwtCreator = jwtCreator;
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpPost("Register")]
        //Post " api/User/Register
        public   async Task<object> Register (UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ApplicationUser = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };

                    var result = await _userManager.CreateAsync(ApplicationUser, model.Password);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: User Format isn't right from Register");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        //Post => api/User/Login
        public async Task<ActionResult> Login(LoginUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if ((user != null) && (await _userManager.CheckPasswordAsync(user, model.Password)))
                    {
                        var token = _jwtCreator.GenerateToken(user.UserName);

                        user.RefreshToken = GenerateRefreshToken();
                        
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            AppendCookies(token, user.RefreshToken);

                            _logger.LogInformation($"DateTime: {DateTime.Now} -- User {user.UserName} has logged in");

                            return Ok();
                        }
                        else
                        {
                            _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Couldn't update the refresh token of into database");
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Username or password is incorrect." });
                    }
                }
                else
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Login Format isn't right from Login");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }

        [AllowAnonymous]
        [HttpHead("RefreshToken/{username}")]
        //Head => api/User/RefreshToken/username
        public async Task<ActionResult> RefreshToken(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var refreshToken = Request.Cookies["RefreshToken"];
                
                if((user != null) && (!string.IsNullOrEmpty(refreshToken)) && (user.RefreshToken == refreshToken))
                {
                    var token = _jwtCreator.GenerateToken(username);
                    Response.Cookies.Append("Token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.Now.AddHours(1)
                    });

                    return Ok();
                }
                else
                {
                    DeleteCookies();

                    _logger.LogInformation($"DateTime: {DateTime.Now} -- User {user.UserName} has forced to log out due to bad refresh token");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }
        
        [HttpGet("Logout")]
        //Get => api/User/Logout
        public ActionResult Logout()
        {
            try
            {
                DeleteCookies();

                _logger.LogInformation($"DateTime: {DateTime.Now} -- User {User.Claims.First(o => o.Type == "UserName").Value} has logged out");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }

        [HttpPost("ChangePassword")]
        //Post => api/User/ChangePassword
        public async Task<ActionResult> ChangePassword(PasswordsViewModel model)
        {
            try
            {
                var username = User.Claims.First(o => o.Type == "UserName").Value;
                var user = await _userManager.FindByNameAsync(username);
                if (user == null) return StatusCode(StatusCodes.Status404NotFound);

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    DeleteCookies();

                    _logger.LogInformation($"DateTime: {DateTime.Now} -- User {username} has changed the password");
                    _logger.LogInformation($"DateTime: {DateTime.Now} -- User {username} has forced to log out due to password change");
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }

        [HttpGet("Search")]
        //Get => api/User/Search?username=
        public async Task<ActionResult> Search([FromQuery]string username)
        {
            try
            {
                var authorizedUsername = User.Claims.First(o => o.Type == "UserName").Value;
                var authorizedUser = await _userManager.FindByNameAsync(authorizedUsername);
                if (authorizedUser == null) return StatusCode(StatusCodes.Status404NotFound);

                var users = _userRepository.GetAllUsersByName(username);
                if (users == null) return StatusCode(StatusCodes.Status404NotFound);

                List<UserInfoViewModel> returnUsers = new List<UserInfoViewModel>();

                foreach (var item in users)
                {
                    if (authorizedUser.UserName == item.UserName) continue;

                    UserInfoViewModel user = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
                    
                    if (_userRepository.GetFollowByUsers(item, authorizedUser) != null) user.IsFollowed = true;
                    else user.IsFollowed = false;

                    returnUsers.Add(user);
                }

                return Ok(returnUsers);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpGet("Home/Questions")]
        //Get => api/User/Home/Questions
        public async Task<ActionResult> GetHomeQuestions()
        {
            try
            {
                var username = User.Claims.First(o => o.Type == "UserName").Value;
                var user = await _userManager.FindByNameAsync(username);

                var following = _userRepository.GetFollowing(user);
                following.Add(user);

                List<QuestionsViewModel> returnQuestions = new List<QuestionsViewModel>();

                foreach (var item in following)
                {
                    var questions = _questionsRepository.GetAllAnsweredQuestionsByUser(item.UserName);
                    foreach (var question in questions)
                    {
                        var returnQuestion = new QuestionsViewModel()
                        {
                            QuestionId = question.QuestionId,
                            Answer = question.Answer,
                            Question = question.Question,
                            QuestionTo = question.QuestionTo.UserName,
                            Time = question.Time,
                            IsAnswered = question.IsAnswered
                        };

                        if (question.QuestionFrom != null) returnQuestion.QuestionFrom = question.QuestionFrom.UserName;
                        returnQuestions.Add(returnQuestion);
                    }
                }

                return Ok(returnQuestions);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }



        /// <summary>
        /// Append Access Token and Refresh Token into httponly Cookies
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        private void AppendCookies(string token, string refreshToken)
        {
            CookieOptions options = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddMonths(3)
            };
            Response.Cookies.Append("Token", token, options);
            Response.Cookies.Append("RefreshToken", refreshToken, options);
            
            Response.Cookies.Append("UNL", "LIT", new CookieOptions()
            { Expires = DateTime.Now.AddMonths(3), Secure = true, HttpOnly = false });
        }


        private void DeleteCookies()
        {
            Response.Cookies.Delete("Token");
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Delete("UNL");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}